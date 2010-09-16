using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Farawla.Core.Language;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Input;
using FontFamily=System.Windows.Media.FontFamily;

namespace Farawla.Core
{
	public class WindowTab
	{
		public string Name { get; set; }
		public string DocumentPath { get; set; }
		public bool IsSaved { get; private set; }
		public string Text { get; private set; }
		public int CaretOffset { get; private set; }

		public LanguageMeta Language { get; set; }
		public TabItem Tab { get; private set; }
		public TextEditor Editor { get; private set; }

		public bool IsNewDocument
		{
			get { return DocumentPath.IsBlank(); }
		}
		public int Index
		{
			get { return Controller.Current.MainWindow.Tab.Items.IndexOf(Tab); }
		}
		
		private BackgroundWorker completionWorker;
		private List<CompletionWindowItem> completionItems;

		public WindowTab(string path)
		{
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
			Editor.Options.ConvertTabsToSpaces = true;
			Editor.Background = new SolidColorBrush(Theme.Instance.Background.ToColor());
			Editor.Foreground = new SolidColorBrush(Theme.Instance.Foreground.ToColor());
			Editor.FontFamily = new FontFamily(Theme.Instance.FontFamily);
			Editor.TextArea.TextEntered += (s, e) => TextEntered(e);
			
			// load?
			if (!path.IsBlank())
				Editor.Load(path);

			// tab
			Tab = new TabItem();
			Tab.Header = Name;
			Tab.Content = Editor;

			// syntax highlighter
			if (Language.HasHighlighting)
			{
				HighlightingManager.Instance.RegisterHighlighting(Language.Name, Language.Associations.ToArray(), Language.Highlighting.GetHighlighter());
				Editor.SyntaxHighlighting = Language.Highlighting.GetHighlighter();
			}

			// initialize completion worker and window
			completionWorker = new BackgroundWorker();
			completionWorker.DoWork += (s, e) => PopulateAutoComplete();
		}

		public void MakeActive()
		{
			var index = Controller.Current.CurrentTabs.IndexOf(this);
			Controller.Current.MainWindow.Tab.SelectedIndex = index;
		}

		public void Save(bool saveAs)
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
					return;
				}
			}

			Editor.Save(DocumentPath);

			IsSaved = true;
			MarkWindowAsSaved();
		}

		public void PromptToSave()
		{
			if (!IsSaved && !(IsNewDocument && Editor.Text == ""))
			{
				var result = MessageBox.Show("Do you want to save '" + Name + "'?", "Save " + Name, MessageBoxButton.YesNoCancel);

				switch (result)
				{
					case MessageBoxResult.Yes:
						Save(false);
						break;

					case MessageBoxResult.No:
						// do nothing
						break;

					case MessageBoxResult.Cancel:
						return;
						break;
				}
			}
		}
		
		public void TextEntered(TextCompositionEventArgs args)
		{
			if (!Editor.IsLoaded)
				return;

			Text = Editor.Text;
			CaretOffset = Editor.CaretOffset;

			// have auto complete?
			if (Language.HasAutoComplete)
			{
				if (completionItems != null && args.Text.Length == 1 && args.Text[0] == '.')
				{
					var cw = new CompletionWindow(Editor.TextArea);
					
					foreach(var item in completionItems)
						cw.CompletionList.CompletionData.Add(item);
					
					Debug.WriteLine("Refreshed completion list: " + completionItems.Count + " items.");
					
					cw.Show();
					cw.Closed += (s, e) => cw = null;
				}
				else if (!completionWorker.IsBusy)
				{
					completionWorker.RunWorkerAsync();
				}
			}

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

		public void PopulateAutoComplete()
		{
			var identifiers = Language.AutoComplete.GetIdentifiersFromCode(Text).Where(i => i.Scope.From < CaretOffset && i.Scope.To >= CaretOffset);
			completionItems = new List<CompletionWindowItem>();

			foreach (var identifier in identifiers)
			{
				completionItems.Add(new CompletionWindowItem(identifier.Name));
			}
		}

		public void Close()
		{
			// save or ignore?
			PromptToSave();

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
	}

	public class CompletionWindowItem : ICompletionData
	{
		public CompletionWindowItem(string text)
		{
			Text = text;
		}

		public ImageSource Image
		{
			get { return null; }
		}

		public string Text { get; private set; }

		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return Text; }
		}

		public object Description
		{
			get { return "Description goes here... " + Text; }
		}

		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, this.Text);
		}
	}
}
