﻿using System;
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

namespace Farawla.Core
{
	public class WindowTab
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
			//Editor.Options.ShowTabs = true;
			//Editor.Options.ShowSpaces = true;
			Editor.Options.ShowBoxForControlCharacters = true;
			//Editor.Options.ConvertTabsToSpaces = true;
			Editor.Background = new SolidColorBrush(Theme.Instance.Background.ToColor());
			Editor.Foreground = new SolidColorBrush(Theme.Instance.Foreground.ToColor());
			Editor.FontFamily = new FontFamily(Theme.Instance.FontFamily);
			Editor.TextArea.TextEntered += (s, e) => TextEntered(e);
			Editor.TextArea.SelectionChanged += (s, e) => SelectionChanged();
			Editor.TextArea.Caret.PositionChanged += (s, e) => CaretOffsetChanged();
			
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
		
		public void TextEntered(TextCompositionEventArgs args)
		{
			if (!Editor.IsLoaded)
				return;

			TextChanged();
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
		
		public void SelectionChanged()
		{
			#region When highlighting a token, highlight all matching tokens
			
			BlockHighlighter.Clear("Selection");
			
			if (Editor.SelectionLength == 0 || Editor.SelectedText.Any(c => !char.IsLetterOrDigit(c)))
				return;
			
			foreach(Match match in Regex.Matches(Editor.Text, Editor.SelectedText))
			{
				BlockHighlighter.Add("Selection", match.Index, match.Length, matchingTokensBackground);
			}
			
			#endregion
		}

		private void CaretOffsetChanged()
		{
			HighlightBracketsIfNextToCaret();
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
			if (completionWindow != null)
				completionWindow.CompletionList.RequestInsertion(e);
		}
		
		public void ShowCompletionWindow(int offset)
		{
			if (IsShowingCompletionWindow)
				return;
			
			IsShowingCompletionWindow = true;

			completionWindow = new CompletionWindow(Editor.TextArea);

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
			textArea.Document.Replace(completionSegment, Text);
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
		
		public BlockHighlighter(TextEditor editor)
		{
			this.editor = editor;
			
			blocks = new List<HighlightedBlocks>();
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

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			textView.EnsureVisualLines();
			
			foreach(var block in blocks)
			{
				var startLocation = textView.Document.GetLocation(block.Offset);
				var endLocation = textView.Document.GetLocation(block.Offset + block.Length);

				var startPosition = textView.GetVisualPosition(new TextViewPosition(startLocation), VisualYPosition.LineTop);
				var endPosition = textView.GetVisualPosition(new TextViewPosition(endLocation), VisualYPosition.LineBottom);

				drawingContext.DrawRoundedRectangle(new SolidColorBrush(block.Color), new Pen(), new Rect(startPosition.X - editor.TextArea.TextView.ScrollOffset.X, startPosition.Y - editor.TextArea.TextView.ScrollOffset.Y, endPosition.X - startPosition.X, endPosition.Y - startPosition.Y), 3, 3);
			}
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
