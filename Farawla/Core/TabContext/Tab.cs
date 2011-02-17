using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Farawla.Core.Language;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
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
	public partial class Tab
	{
		public string Name { get; set; }
		public string DocumentPath { get; set; }
		public bool IsSaved { get; private set; }
		public bool IsShowingCompletionWindow { get; set; }

		public LanguageMeta Language { get; set; }
		public TabItem TabItem { get; private set; }
		public TextEditor Editor { get; private set; }
		public BlockHighlighter BlockHighlighter { get; private set; }
		public DocumentHighlighter DocumentHighlighter { get; private set; }

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

		public Tab(string path)
		{
			// set name and path
			if (path.IsBlank())
			{
				IsSaved = false;
				Name = "new";
				DocumentPath = string.Empty;
				Language = Controller.Current.Languages.GetDefaultLanguage();
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
				Language = Controller.Current.Languages.GetLanguageByExtension(path.Substring(path.LastIndexOf('.') + 1));
			}

			// arrange
			matchingTokensBackground = Theme.Instance.MatchingTokensBackground.ToColor();
			
			// editor
			Editor = new TextEditor();
			Editor.FontSize = 13;
			Editor.ShowLineNumbers = true;
			Editor.Options.EnableEmailHyperlinks = false;
			Editor.Options.EnableHyperlinks = false;
			Editor.Options.ShowBoxForControlCharacters = true;
			//Editor.Options.ConvertTabsToSpaces = true;
			Editor.Background = new SolidColorBrush(Theme.Instance.Background.ToColor());
			Editor.Foreground = new SolidColorBrush(Theme.Instance.Foreground.ToColor());
			Editor.FontFamily = new FontFamily(Theme.Instance.FontFamily);
			Editor.TextChanged += TextChanged;
			Editor.TextArea.SelectionChanged += SelectionChanged;
			Editor.TextArea.Caret.PositionChanged += CaretOffsetChanged;
			Editor.TextArea.GotFocus += EditorGotFocus;
			
			// renderer
			BlockHighlighter = new BlockHighlighter(Editor);
			Editor.TextArea.TextView.BackgroundRenderers.Add(BlockHighlighter);
			
			// load?
			if (!path.IsBlank())
			{
				Editor.Load(path);		
			}

			// tab
			TabItem = new TabItem();
			TabItem.Tag = this;
			TabItem.Content = Editor;
			
			// tab header
			var header = new TextBlock();
			header.IsHitTestVisible = false;
			header.Text = Name;
			
			if (!path.IsBlank())
				header.ToolTip = path;
			
			TabItem.Header = header;
			
			// tab header double click
			TabItem.MouseDoubleClick += (s, e) => {
				if (!(e.OriginalSource is TextView))
					Close();
			};
			
			// syntax highlighter
			if (Language.HasSyntax)
			{
				//HighlightingManager.Instance.RegisterHighlighting(Language.Name, Language.Associations.ToArray(), Language.Syntax.GetHighlighter());
				
				var ruleset = Language.Syntax.GetHighlighter();
				Editor.SyntaxHighlighting = ruleset;
				DocumentHighlighter = new DocumentHighlighter(Editor.Document, ruleset.MainRuleSet);
			}
			
			// assign completion window
			completionItems = new List<CompletionWindowItem>();
		}

		public void MakeActive()
		{
			var index = Controller.Current.CurrentTabs.IndexOf(this);
			
			Controller.Current.MainWindow.Tab.SelectedIndex = index;

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
				Notifier.Show("Error saving '" + DocumentPath + "':\n" + e.Message);
			}

			IsSaved = true;
			MarkWindowAsSaved();
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
		
		public void TextChanged()
		{
			// mark as unsaved?
			if (IsSaved || IsNewDocument)
			{
				IsSaved = false;
				MarkWindowAsUnsaved();
			}
		}

		public void MarkWindowAsUnsaved()
		{
			TabItem.Header = Name + "*";
		}

		public void MarkWindowAsSaved()
		{
			TabItem.Header = Name;
		}
		
		public void Rename(string newPath)
		{
			Name = Path.GetFileName(newPath);
			DocumentPath = newPath;

			TabItem.Header = Name;
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
		
		public List<string> GetCurrentSegmentNames()
		{
			var offset = Editor.CaretOffset;
			var line = DocumentHighlighter.HighlightLine(Editor.Document.GetLineByOffset(offset));
			
			return line.Sections
						.Where(s => s.Offset <= offset && s.Offset + s.Length >= offset)
						.Select(s => s.Color.Name)
						.ToList();
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

		public void CompletionRequestInsertion(TextCompositionEventArgs e, bool isDelimiterText)
		{
			if (completionWindow != null && completionWindow.IsVisible && isDelimiterText)
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
	}
}