using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Farawla.Core.Language;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Input;
using DrawingContext=System.Windows.Media.DrawingContext;
using FontFamily=System.Windows.Media.FontFamily;
using ImageSource=System.Windows.Media.ImageSource;
using System.Windows.Markup;
using Farawla.Utilities;

namespace Farawla.Core
{
	public partial class WindowTab
	{
		public string Name { get; set; }
		public string DocumentPath { get; set; }
		public bool IsSaved { get; private set; }
		public bool IsShowingCompletionWindow { get; set; }

		public LanguageMeta Language { get; set; }
		public TabItem Tab { get; private set; }
		public TextEditor Editor { get; private set; }
		public BlockHighlighter BlockHighlighter { get; private set; }

		public bool IsNewDocument
		{
			get { return DocumentPath.IsBlank(); }
		}
		public int Index
		{
			get { return Controller.Current.MainWindow.Tab.Items.IndexOf(Tab); }
		}

		private Color matchingTokensBackground;
		private List<CompletionWindowItem> completionItems;
		private CompletionWindow completionWindow;

		public WindowTab(string path)
		{
			// arrange
			matchingTokensBackground = Theme.Instance.MatchingTokensBackground.ToColor();
			
			// get language
			Language = Controller.Current.Languages.GetLanguage(path.Substring(path.LastIndexOf('.') + 1));

			// set name and path
			if (path.IsBlank())
			{
				IsSaved = false;
				Name = "new";
				DocumentPath = string.Empty;
			}
			else
			{
				IsSaved = true;
				Name = Path.GetFileName(path);
				DocumentPath = path;
			}

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
			Editor.TextArea.TextEntered += TextEntered;
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
			Tab = new TabItem();
			Tab.Header = Name;
			Tab.Content = Editor;
			
			// syntax highlighter
			if (Language.HasSyntax)
			{
				HighlightingManager.Instance.RegisterHighlighting(Language.Name, Language.Associations.ToArray(), Language.Syntax.GetHighlighter());
				Editor.SyntaxHighlighting = Language.Syntax.GetHighlighter();
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

			Editor.Save(DocumentPath);

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
			Tab.Header = Name + "*";
		}

		public void MarkWindowAsSaved()
		{
			Tab.Header = Name;
		}
		
		public void Rename(string newPath)
		{
			Name = Path.GetFileName(newPath);
			DocumentPath = newPath;

			Tab.Header = Name;
		}
		
		public void Close()
		{
			// save or ignore?
			if (!PromptToSave())
				return;

			// add path to closed tabs, keep last ten tabs
			if (!IsNewDocument)
			{
				if (Settings.Instance.ClosedTabs.Count > 0 && Settings.Instance.ClosedTabs[0].Path != DocumentPath)
				{
					Settings.Instance.ClosedTabs.Insert(0, new ClosedTabs(DocumentPath, Index));
					Settings.Instance.ClosedTabs = Settings.Instance.ClosedTabs.Take(10).ToList();
				}
			}

			// remove tab
			Controller.Current.MainWindow.Tab.Items.Remove(Tab);
			Controller.Current.CurrentTabs.Remove(this);

			// update count (also open a new tab if tab count is zero)
			Controller.Current.TabCountUpdated();
		}

		private void TextEntered(object sender, TextCompositionEventArgs args)
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
			if (completionWindow != null && (completionWindow.IsActive || isDelimiterText))
				completionWindow.CompletionList.RequestInsertion(e);
		}
		
		public void ShowCompletionWindow(int offset)
		{
			if (IsShowingCompletionWindow)
				return;
			
			IsShowingCompletionWindow = true;

			completionWindow = new CompletionWindow(Editor.TextArea);

			completionWindow.Background = new SolidColorBrush(Colors.Purple);
			completionWindow.StartOffset = offset;
			
			foreach (var item in completionItems)
				completionWindow.CompletionList.CompletionData.Add(item);

			completionWindow.Show();
			
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
	
	public enum CompletionItemType
	{
		Object,
		Function,
		Snippet
	}
	
	public class CompletionWindowItem : ICompletionData
	{
		public string Owner { get; private set; }
		public string Text { get; private set; }
		public object Description { get; private set; }
		public object Content { get; private set; }
		public CompletionItemType Type { get; private set; }
		public ImageSource Image { get; private set; }
		
		public CompletionWindowItem(CompletionItemType type, string owner, string text, string description)
		{
			Owner = owner;
			Text = text;
			Type = type;

			switch (type)
			{
				case CompletionItemType.Object:
					Image = Theme.Instance.GetObjectIcon();
					break;
				
				case CompletionItemType.Function:
					Image = Theme.Instance.GetFunctionIcon();
					break;
				
				case CompletionItemType.Snippet:
					Image = Theme.Instance.GetSnippetIcon();
					break;
			}
			
			Content = text; // use this property for fancy UI
			Description = description;
		}
		
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			var value = Text;
			
			if (Type == CompletionItemType.Function)
			{
				var index = value.IndexOf('(');
				
				if (index > 0)
					value = value.Substring(0, index);
			}

			textArea.Document.Replace(completionSegment, value);
		}
		
		#region Equal & GetHashCode
		
		public override bool Equals(object obj)
		{
			if (!(obj is CompletionWindowItem))
				return false;

			var other = obj as CompletionWindowItem;
			
			return other.Owner == Owner && other.Text == Text;
		}

		public override int GetHashCode()
		{
			return (Text + Owner).GetHashCode();
		}
		
		#endregion
	}

	public class BlockHighlighter : IBackgroundRenderer
	{
		private List<HighlightedBlocks> blocks;
		private TextEditor editor;
		
		private SolidColorBrush tabsColor;
		private SolidColorBrush spacesColor;
		
		public BlockHighlighter(TextEditor editor)
		{
			this.editor = editor;
			
			blocks = new List<HighlightedBlocks>();
			
			tabsColor = new SolidColorBrush(Theme.Instance.TabColor.ToColor());
			spacesColor = new SolidColorBrush(Theme.Instance.SpaceColor.ToColor());
		}

		public KnownLayer Layer
		{
			get { return KnownLayer.Background; }
		}
		
		public void Add(string owner, int offset, int length, Color color)
		{
			blocks.Add(new HighlightedBlocks(owner, offset, length, color));
		}

		public void Remove(string owner, int offset, int length)
		{
			var block = blocks.FirstOrDefault(b => b.Owner == owner && b.Offset == offset && b.Length == length);
			blocks.Remove(block);
		}
		
		public void ClearAll()
		{
			blocks.Clear();
		}

		public void Clear(string owner)
		{
			blocks.RemoveAll(b => b.Owner == owner);
		}

		public void Draw(TextView textView, DrawingContext ctx)
		{
			textView.EnsureVisualLines();
			
			//TODO: probably should only draw lines inside the view area (i.e. inside the fold)

			// draw tabs
			if (Settings.Instance.ShowTabsInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, "\t"))
				{
					var point = GetPositionFromOffset(textView, VisualYPosition.LineMiddle, match.Index);

					var x1 = point.X - editor.TextArea.TextView.ScrollOffset.X + 3;
					var y1 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					var x2 = point.X - editor.TextArea.TextView.ScrollOffset.X + 23;
					var y2 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 == x2)
						continue;
					
					var pen = new Pen
					{
						Brush = tabsColor,
						Thickness = 0.3
					};

					ctx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
				}
			}

			// draw spaces
			if (Settings.Instance.ShowSpacesInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, " "))
				{
					var point = GetPositionFromOffset(textView, VisualYPosition.LineMiddle, match.Index);

					var x1 = point.X - editor.TextArea.TextView.ScrollOffset.X + 2;
					var y1 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;
					
					var x2 = point.X - editor.TextArea.TextView.ScrollOffset.X + 4;
					var y2 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 == x2)
						continue;

					var pen = new Pen
					{
						Brush = spacesColor,
						Thickness = 1
					};

					ctx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
				}
			}
			
			// draw blocks
			foreach(var block in blocks)
			{
				var startPosition = GetPositionFromOffset(textView, VisualYPosition.LineTop, block.Offset);
				var endPosition = GetPositionFromOffset(textView, VisualYPosition.LineBottom, block.Offset + block.Length);

				var x = startPosition.X - editor.TextArea.TextView.ScrollOffset.X;
				var y = startPosition.Y - editor.TextArea.TextView.ScrollOffset.Y;

				var width = endPosition.X - startPosition.X;
				var height = endPosition.Y - startPosition.Y;
				
				if (width <= 0 || height <= 0)
					continue;

				ctx.DrawRoundedRectangle(new SolidColorBrush(block.Color), new Pen(), new Rect(x, y, width, height), 3, 3);
			}
		}
		
		private Point GetPositionFromOffset(TextView textView, VisualYPosition position, int offset)
		{
			var startLocation = textView.Document.GetLocation(offset);
			var point = textView.GetVisualPosition(new TextViewPosition(startLocation), position);

			return new Point(Math.Round(point.X), Math.Round(point.Y));
		}
		
		public void Redraw()
		{
			editor.TextArea.TextView.Redraw();
		}
	}

	public class HighlightedBlocks
	{
		public string Owner { get; set; }
		public int Offset { get; set; }
		public int Length { get; set; }
		public Color Color { get; set; }

		public HighlightedBlocks(string owner, int offset, int length, Color color)
		{
			Owner = owner;
			Offset = offset;
			Length = length;
			Color = color;
		}
	}
	
	
}
