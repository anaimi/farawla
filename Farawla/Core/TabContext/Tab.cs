using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Farawla.Core.Language;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Input;
using FontFamily=System.Windows.Media.FontFamily;
using Farawla.Utilities;
using Farawla.Features;

namespace Farawla.Core.TabContext
{
	public class Tab
	{
		public string Name { get; set; }
		public string DocumentPath { get; set; }
		public EditorSegment ActiveLanguageSegment { get; private set; }
		public bool IsSaved { get; private set; }
		public bool IsShowingCompletionWindow { get; set; }

		public LanguageMeta Language { get; set; }
		public ExtendedTabItem TabItem { get; private set; }
		public ExtendedTabHeader TabHeader { get; private set; }
		public TextEditor Editor { get; private set; }
		public BlockHighlighter BlockHighlighter { get; private set; }
		public IHighlighter DocumentHighlighter { get; private set; }

		public bool IsNewDocument
		{
			get { return DocumentPath.IsBlank(); }
		}
		public int Index
		{
			get { return Controller.Current.MainWindow.Tab.Items.IndexOf(TabItem); }
		}

		private Color matchingTokensBackground;
		private List<CompletionWindowItem> completionItems;
		private CompletionWindow completionWindow;
		private ColorPreviewRenderer colorPreview;

		public Tab(string path)
		{
			// arrange
			matchingTokensBackground = Theme.Instance.MatchingTokensBackground.ToColor();
			
			#region Set Name and DocumentPath
			
			if (path.IsBlank())
			{
				IsSaved = false;
				Name = "new";
				DocumentPath = string.Empty;
			}
			else
			{
				if (!File.Exists(path))
				{
					// Notifier.Show("Attempted to open a file that doesn't exist");
					return;
				}
				
				IsSaved = true;
				Name = Path.GetFileName(path);
				DocumentPath = path;
			}
			
			#endregion
			
			#region Create editor
			
			Editor = new TextEditor();
			Editor.FontSize = 13;
			Editor.ShowLineNumbers = true;
			Editor.Options.EnableEmailHyperlinks = false;
			Editor.Options.EnableHyperlinks = false;
			Editor.Options.ShowBoxForControlCharacters = true;
			//Editor.Options.ConvertTabsToSpaces = true;
			Editor.Background = ThemeColorConverter.GetColor("Background");
			Editor.Foreground = ThemeColorConverter.GetColor("Foreground");
			Editor.FontFamily = new FontFamily(Theme.Instance.FontFamily);
			Editor.TextArea.PreviewKeyDown += EditorKeyDown;
			Editor.TextChanged += TextChanged;
			Editor.TextArea.SelectionChanged += SelectionChanged;
			Editor.TextArea.Caret.PositionChanged += CaretOffsetChanged;
			Editor.TextArea.GotFocus += EditorGotFocus;
			Editor.Encoding = System.Text.Encoding.GetEncoding("ASCII");
			
			#endregion
			
			#region Create context menu for editor

			var menu = new ContextMenu();

			menu.Items.Add(ContextMenuHelper.CreateManuItem("New Tab", "CTRL+T", () => Controller.Current.CreateNewTab("")));
			menu.Items.Add(ContextMenuHelper.CreateManuItem("Open Previous Tab", "CTRL+SHIFT+T", Controller.Current.OpenLastClosedTab));
			menu.Items.Add(ContextMenuHelper.CreateManuItem("Browse File", "CTRL+O", Controller.Current.BrowseFile));
			menu.Items.Add(new Separator());
			menu.Items.Add(ContextMenuHelper.CreateManuItem("Quick Jump", "CTRL+,", () => Controller.Current.GetWidget<Features.Projects.Widget>().Jump.ShowBox()));
			menu.Items.Add(ContextMenuHelper.CreateManuItem("Stats & Encoding", "CTRL+ALT (hold)", () => Controller.Current.GetWidget<Features.Stats.Widget>().ShowStats()));
			menu.Items.Add(new Separator());
			menu.Items.Add(GetChangleLanguageContextMenuItem());
			menu.Items.Add(ContextMenuHelper.CreateManuItem("Close Tab", "CTRL+F4", Controller.Current.CloseActiveTab));

			Editor.ContextMenu = menu;
			
			#endregion
			
			// renderer
			BlockHighlighter = new BlockHighlighter(this);
			Editor.TextArea.TextView.BackgroundRenderers.Add(BlockHighlighter);
			colorPreview = new ColorPreviewRenderer(this);
			Editor.TextArea.TextView.BackgroundRenderers.Add(colorPreview);
			
			// load language
			LoadLanguageHighlighterAndSyntax(null);

			// tab item
			TabHeader = new ExtendedTabHeader(this); // always before TabItem
			TabItem = new ExtendedTabItem(this);			
			
			// assign completion window
			completionItems = new List<CompletionWindowItem>();

			// load? (always last)
			if (!path.IsBlank())
				Editor.Load(path);
		}

		private void EditorKeyDown(object sender, KeyEventArgs e)
		{
			var ctrl = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
			var keyD = Keyboard.IsKeyDown(Key.D);
			
			if (ctrl && keyD)
			{
				var line = Editor.Document.GetLineByOffset(Editor.CaretOffset);
				var text = Editor.Document.GetText(line.Offset, line.Length);
				
				Editor.Document.Insert(line.EndOffset, "\n" + text);
				
				e.Handled = true;
			}
		}

		public void LoadLanguageHighlighterAndSyntax(LanguageMeta lang)
		{
			// forced language, via context menu
			if (lang != null)
			{
				Language = lang;
			}
			// new tab
			else if (DocumentPath.IsBlank())
			{
				Language = Controller.Current.Languages.GetDefaultLanguage();
				return;
			}
			// language via rename or save
			else
			{
				var language = Controller.Current.Languages.GetLanguageByExtension(DocumentPath.Substring(DocumentPath.LastIndexOf('.') + 1));

				if (Language != null && Language.Name == language.Name)
					return;

				Language = language;
			}
			
			// syntax highlighter
			if (Language.HasSyntax || Language.IsDefault)
			{
				var ruleset = Language.Syntax.GetHighlighter();
				
				Editor.SyntaxHighlighting = ruleset;
				DocumentHighlighter = Editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;

				CaretOffsetChanged(null, null); // to inform language context change
			}
		}
		
		public MenuItem GetChangleLanguageContextMenuItem()
		{
			var languages = ContextMenuHelper.CreateManuItem("Change Language", "", null);

			languages.Items.Add(ContextMenuHelper.CreateManuItem("Default", "", () => LoadLanguageHighlighterAndSyntax(Controller.Current.Languages.GetDefaultLanguage())));

			languages.Items.Add(new Separator());
			
			foreach (var lang in Controller.Current.Languages.Items)
			{
				var language = lang;
				languages.Items.Add(ContextMenuHelper.CreateManuItem(lang.Name, "", () => LoadLanguageHighlighterAndSyntax(language)));
			}

			return languages;
		}

		public void MakeActive(bool giveFocus)
		{
			var index = Controller.Current.CurrentTabs.IndexOf(this);
			
			Controller.Current.MainWindow.Tab.SelectedIndex = index;
			
			if (giveFocus)
				DelayedAction.Invoke(250, () => Editor.Focus());
		}
		
		public void MadeActive()
		{
			for(var i = 0; i < Controller.Current.CurrentTabs.Count; i++)
			{
				Panel.SetZIndex(Controller.Current.CurrentTabs[i].TabItem, i);
			}
			
			Panel.SetZIndex(TabItem, Controller.Current.CurrentTabs.Count);
			
			DelayedAction.Invoke(250, () => Editor.Focus());
		}

		public bool Save(bool saveAs)
		{
			var dialog = new SaveFileDialog();

			if (IsNewDocument || saveAs)
			{
				if (dialog.ShowDialog().Value)
				{
					DocumentPath = dialog.FileName;
					Name = Path.GetFileName(DocumentPath);
				}
				else
				{
					return false;
				}
			}

			try
			{
				Editor.Save(DocumentPath);
			}
			catch(Exception e)
			{
				Notifier.Growl("Error saving document", e.Message, DocumentPath);
				return false;
			}

			IsSaved = true;
			TabHeader.MarkAsSaved();
			LoadLanguageHighlighterAndSyntax(null);
			
			return true;
		}

		public bool PromptToSave()
		{
			if (!IsSaved && !(IsNewDocument && Editor.Text == ""))
			{
				var result = MessageBox.Show("Do you want to save '" + Name + "'?", "Save " + Name, MessageBoxButton.YesNoCancel);

				switch (result)
				{
					case MessageBoxResult.Yes:
						return Save(false);
						
					case MessageBoxResult.No:
						return true;
						
					case MessageBoxResult.Cancel:
						return false;
				}
			}

			return true;
		}
		
		public void Rename(string newPath)
		{
			Name = Path.GetFileName(newPath);
			DocumentPath = newPath;

			TabHeader.Rename(Name);
			LoadLanguageHighlighterAndSyntax(null);
		}
		
		public void Close()
		{
			// save or ignore?
			if (!PromptToSave())
				return;

			// add path to closed tabs, keep last ten tabs
			if (!IsNewDocument)
			{
				if (Settings.Instance.ClosedTabs.Count > 0)
				{
					if (Settings.Instance.ClosedTabs[0].Path != DocumentPath)
					{
						Settings.Instance.ClosedTabs.Insert(0, new ClosedTabs(DocumentPath, Index));
						Settings.Instance.ClosedTabs = Settings.Instance.ClosedTabs.Take(10).ToList();
					}
				}
				else
				{
					Settings.Instance.ClosedTabs.Add(new ClosedTabs(DocumentPath, Index));
				}
			}

			// remove tab
			Controller.Current.MainWindow.Tab.Items.Remove(TabItem);
			Controller.Current.CurrentTabs.Remove(this);

			// update count (also open a new tab if tab count is zero)
			Controller.Current.TabCountUpdated();
		}

		public void GoToLine()
		{
			var line = Editor.Document.GetLineByOffset(Editor.CaretOffset);

			Notifier.Prompt("Go to", "You are at line " + line.LineNumber, "", (canceled, message) =>
			{
				if (canceled)
					return;
				
				if (!message.IsInteger())
				{
					Notifier.Growl("Can't go to line", "Expected a number, got " + message, "try again");
					return;
				}
				
				var dest = message.ToInteger();
				var maxLine = Editor.Document.LineCount;
				
				if (dest <= 0 || dest > maxLine)
				{
					Notifier.Growl("Can't go to line", "Requested line number is out of range", "try again");
					return;
				}
				
				Editor.TextArea.Caret.Line = dest;
				Editor.ScrollToLine(dest);
				Editor.Focus();
				
			});
		}
		
		public List<EditorSegment> GetCurrentSegments()
		{
			if (DocumentHighlighter == null)
				return new List<EditorSegment>();
			
			var offset = Editor.CaretOffset;
			var line = DocumentHighlighter.HighlightLine(Editor.TextArea.Caret.Line);
			
			return line.Sections
						.Where(s => s.Offset <= offset && s.Offset + s.Length >= offset)
						.Select(s => new EditorSegment(this, s.Color.Name, s.Offset, s.Length))
						.ToList();
		}
		
		public EditorSegment GetOverlappingSegment(string name, int midOffset)
		{
			var startOffset = midOffset;
			var endOffset = midOffset;
			
			// before
			while(true)
			{
				var line = DocumentHighlighter.HighlightLine(Editor.Document.GetLineByOffset(startOffset).LineNumber);

				var sections = line.Sections.Where(s => s.Color.Name.StartsWith(name) && s.Offset <= startOffset);

				if (sections.Count() == 0)
					break;

				var smallestSec = sections.FirstOrDefault(s => s.Offset == sections.Min(ss => ss.Offset));
				
				if (smallestSec.Offset >= startOffset)
					break;
				
				startOffset = smallestSec.Offset;
			}
			
			// after
			var localEndOffset = endOffset;
			while(true)
			{
				// validate index
				if (Editor.Text.Length < localEndOffset)
				{
					endOffset = Editor.Text.Length - 1;
					break;
				}

				// eat whitespace
				while (Editor.Text.Length > localEndOffset && char.IsWhiteSpace(Editor.Text[localEndOffset]))
				{
					localEndOffset++;
				}
				
				// check for segment
				var line = DocumentHighlighter.HighlightLine(Editor.Document.GetLineByOffset(localEndOffset).LineNumber);

				var sections = line.Sections.Where(s => s.Color.Name.StartsWith(name) && s.Offset <= localEndOffset && (s.Offset + s.Length) > endOffset);
				
				if (sections.Count() == 0)
					break;
				
				var largestSec = sections.FirstOrDefault(s => s.EndOffset() == sections.Max(ss => ss.EndOffset()));
				
				endOffset = largestSec.EndOffset();
				localEndOffset = endOffset + 1;				
			}

			return new EditorSegment(this, name, startOffset, endOffset - startOffset);
		}

		public void TextChanged()
		{
			// mark as unsaved?
			if (IsSaved || IsNewDocument)
			{
				IsSaved = false;
				TabHeader.MarkAsUnSaved();
			}
		}

		private void TextChanged(object sender, EventArgs e)
		{
			if (!Editor.IsLoaded)
				return;
			
			TextChanged();
		}
		
		private void SelectionChanged(object sender, EventArgs e)
		{
			#region When highlighting a token, highlight all matching tokens

			BlockHighlighter.Clear("Selection");

			if (Editor.SelectionLength == 0 || Editor.SelectedText.Any(c => !char.IsLetterOrDigit(c)))
				return;

			foreach (Match match in Regex.Matches(Editor.Text, Editor.SelectedText))
			{
				BlockHighlighter.Add("Selection", match.Index, match.Length, matchingTokensBackground);
			}

			#endregion
		}

		private void EditorGotFocus(object sender, RoutedEventArgs e)
		{
			Controller.Current.MainWindow.Sidebar.DontHideSidebar = false;
		}

		private void CaretOffsetChanged(object sender, EventArgs e)
		{
			HighlightBracketsIfNextToCaret();
			
			#region CurrentLanguageName changed

			EditorSegment segment;
			var oldSegment = ActiveLanguageSegment;
			var segments = GetCurrentSegments();

			if (segments.Where(s => s.IsSyntax).Count() == 0)
			{
				ActiveLanguageSegment = new EditorSegment(this);
			}
			else
			{
				segment = segments.FirstOrDefault(s => s.IsSyntax);

				if (segment != null && !segment.Equals(oldSegment))
				{
					segment = GetOverlappingSegment(segment.Name, segment.Offset);

					ActiveLanguageSegment = segment;
				}
			}

			if (oldSegment != null && !oldSegment.Equals(ActiveLanguageSegment))
				Controller.Current.ActiveSegmentChanged(ActiveLanguageSegment);
			
			#endregion
		}
		
		#region Completion
		
		public void ClearCompletionItems(string owner)
		{
			completionItems.RemoveAll(i => i.Owner == owner);
			
			if (completionWindow != null)
			{
				var toRemove = new List<CompletionWindowItem>();
				
				foreach(CompletionWindowItem item in completionWindow.CompletionList.CompletionData)
				{
					if (item.Owner == owner)
						toRemove.Add(item);
				}
				
				foreach(var item in toRemove)
					completionWindow.CompletionList.CompletionData.Remove(item);
			}
		}
		
		public void RemoveCompletionItem(CompletionWindowItem item)
		{
			if (completionItems.Contains(item))
				completionItems.Remove(item);
			
			if (completionWindow != null)
			{
				completionWindow.CompletionList.CompletionData.Remove(item);
			}
		}
		
		public void AddCompletionItem(CompletionWindowItem item)
		{
			completionItems.Add(item);

			if (completionWindow != null)
			{
				completionWindow.CompletionList.CompletionData.Add(item);
			}
		}
		
		public void AddCompletionItems(string owner, List<CompletionWindowItem> items)
		{
			var currentItems = completionItems.Where(i => i.Owner == owner);

			var toRemove = currentItems.Except(items).ToList();
			var toAdd = items.Except(currentItems).ToList();
			
			foreach(var item in toRemove)
				RemoveCompletionItem(item);
			
			foreach(var item in toAdd)
				AddCompletionItem(item);
		}

		public void CompletionRequestInsertion(TextCompositionEventArgs e)
		{
			if (completionWindow != null && completionWindow.IsVisible)
				completionWindow.CompletionList.RequestInsertion(e);
		}

		public void ShowCompletionWindow(int offset, string enteredText, bool isDelimiter)
		{
			if (IsShowingCompletionWindow)
				return;
			
			IsShowingCompletionWindow = true;

			// create window
			completionWindow = new CompletionWindow(Editor.TextArea);
			completionWindow.StartOffset = offset;
			
			// data
			foreach (var item in completionItems)
				completionWindow.CompletionList.CompletionData.Add(item);

			//// tooltip
			//completionWindow.ToolTip = new ToolTip() {
			//    HorizontalOffset = 20,
			//};
			
			// select text
			if (!isDelimiter)
				completionWindow.CompletionList.SelectItem(enteredText);
			
			// show		
			completionWindow.Show();
			
			// events
			completionWindow.Closed += (s, e) => {
				IsShowingCompletionWindow = false;
			};
		}
		
		public void HideCompletionWindow()
		{
			if (completionWindow != null)
				completionWindow.Close();
		}
		
		#endregion

		#region Highlight bracket pairs

		private void HighlightBracketsIfNextToCaret()
		{
			var brackets = new Dictionary<char, char> { { '(', ')' }, { '{', '}' }, { '[', ']' } };

			BlockHighlighter.Clear("Brackets");

			char begin = ' ';
			var index = 0;
			var increment = false;
			var foundBracket = false;

			var before = Editor.CaretOffset > 0 ? Editor.Text[Editor.CaretOffset - 1] : ' ';
			var after = Editor.CaretOffset < Editor.Text.Length ? Editor.Text[Editor.CaretOffset] : ' ';

			if (before.IsOneOf(')', '}', ']'))
			{
				index = Editor.CaretOffset - 1;
				begin = Editor.Text[index];

				foundBracket = true;
			}

			if (after.IsOneOf('(', '{', '['))
			{
				index = Editor.CaretOffset;
				begin = Editor.Text[index];

				foundBracket = true;
			}

			if (foundBracket)
			{
				char end;

				if (brackets.ContainsKey(begin))
				{
					end = brackets[begin];
					increment = true;
				}
				else
				{
					end = brackets.First(b => b.Value == begin).Key;
				}

				HighlightMatchingBracket(index, begin, end, increment);
			}

			BlockHighlighter.Redraw();
		}

		private void HighlightMatchingBracket(int index, char begin, char end, bool increment)
		{
			BlockHighlighter.Add("Brackets", index, 1, Theme.Instance.MatchingBracketsBackground.ToColor());

			var limit = Editor.Text.Length;
			var skip = 0;

			if (increment)
				index++;
			else
				index--;

			while (index >= 0 && index < limit)
			{
				var _char = Editor.Text[index];

				if (_char == begin)
				{
					skip++;
				}
				else if (_char == end)
				{
					if (skip > 0)
					{
						skip--;
					}
					else
					{
						BlockHighlighter.Add("Brackets", index, 1, Theme.Instance.MatchingBracketsBackground.ToColor());
						break;
					}
				}

				if (increment)
					index++;
				else
					index--;
			}
		}

		#endregion

		public override string ToString()
		{
			return Name + "(" + DocumentPath + ")";
		}
	}
	
	public static class SectionExtension
	{
		public static int EndOffset(this HighlightedSection section)
		{
			return section.Offset + section.Length;
		}
	}
}