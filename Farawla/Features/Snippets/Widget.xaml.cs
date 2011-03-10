using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Farawla.Core;
using Farawla.Core.Sidebar;
using System.IO;
using System.Windows;
using Farawla.Core.TabContext;
using Farawla.Utilities;
using Newtonsoft.Json;

namespace Farawla.Features.Snippets
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public Dictionary<string, SnippetGroup> Snippets { get; set; }
		public SnippetGroup ActiveGroup { get; set; }
		public string ActiveGroupName;
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Snippets");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;
			
			// initialize
			Snippets = new Dictionary<string, SnippetGroup>();
			
			// assign events
			Controller.Current.OnContextLanguageChanged += ContextLanguageChanged;
			Controller.Current.OnTabCreated += TabCreated;
		}
		
		private void TabCreated(Tab tab)
		{
			tab.Editor.TextArea.KeyUp += (s, e) => OnTextEntered(tab, e);
		}

		private void ContextLanguageChanged(EditorSegment segment)
		{
			if (ActiveGroupName == segment.SyntaxName)
				return;
			
			ShowSnippets(segment.SyntaxName);
		}

		private void OnTextEntered(Tab tab, KeyEventArgs e)
		{
			if (ActiveGroup == null || e.Key != Key.Tab)
				return;

			var line = tab.Editor.Document.GetLineByOffset(tab.Editor.CaretOffset);
			var length = tab.Editor.CaretOffset - line.Offset - 1;
			
			if (length <= 0)
				return;
			
			var text = tab.Editor.Document.GetText(line.Offset, length);
			var snippet = ActiveGroup.Snippets.FirstOrDefault(s => text.EndsWith(s.Trigger));
			
			if (snippet == null)
				return;

			tab.Editor.Document.Remove(tab.Editor.CaretOffset - (snippet.Trigger.Length + 1), snippet.Trigger.Length + 1);
			
			ApplySnippet(snippet);
			
			e.Handled = true;
		}
		
		private void PopulateSnippets(string name, string path)
		{
			var panel = new StackPanel();

			#region Populate list
			
			var list = JsonHelper.Load<List<Snippet>>(path);
			
			if (list == null)
			{
				Snippets.Add(name, null);
				return;
			}
			
			#endregion
			
			#region Populate panel's children
			
			foreach(var item in list)
			{
				var button = new SnippetButton(item, ApplySnippet);
				
				panel.Children.Add(button);
			}
			
			#endregion
			
			Snippets.Add(name, new SnippetGroup { Panel = panel, Snippets = list });
		}
		
		private void ShowSnippets(string languageName)
		{
			var language = Controller.Current.Languages.GetLanguageByName(languageName);

			if (language.IsDefault)
			{
				ShowNoSnippets();
				return;
			}

			// not cached? populate it
			if (!Snippets.ContainsKey(language.Name))
			{
				var path = language.Directory + "\\snippets.js";

				if (File.Exists(path))
				{
					PopulateSnippets(language.Name, path);
				}
				else
				{
					Snippets.Add(language.Name, new SnippetGroup());
				}
			}

			// get the cached panel
			var snippets = Snippets[language.Name];

			// hide it if null
			if (snippets == null || snippets.Snippets.Count == 0)
			{
				ShowNoSnippets();

				ActiveGroup = snippets;
				ActiveGroupName = language.Name;
			}

			// show it otherwise
			else
			{
				ActiveGroup = snippets;
				ActiveGroupName = language.Name;
				
				NoSnippets.Visibility = Visibility.Collapsed;
				Container.Visibility = Visibility.Visible;

				Container.Child = ActiveGroup.Panel;
			}
		}
		
		private void ShowNoSnippets()
		{
			ActiveGroup = null;
			NoSnippets.Visibility = Visibility.Visible;
			Container.Visibility = Visibility.Collapsed;
		}
		
		private void ApplySnippet(Snippet snippet)
		{
			var editor = Controller.Current.ActiveTab.Editor;
			
			#region Indent body
			var lineNumber = editor.Document.GetLineByOffset(editor.CaretOffset);
			var lineText = editor.Document.GetText(lineNumber);
			var formatedBody = snippet.Body;
			
			foreach(var c in lineText)
			{
				if (c == '\t')
				{
					formatedBody = formatedBody.Replace("\n", "\n\t");
					
				}
				else if (c == ' ')
				{
					formatedBody = formatedBody.Replace("\n", "\n ");					
				}
				else
				{
					break;
				}

			}
			#endregion
			
			// get caret position
			var caret = -1;
			if (formatedBody.Contains("$0"))
			{
				var index = formatedBody.IndexOf("$0");
				caret = editor.CaretOffset + index;
				formatedBody = formatedBody.Remove(index, 2);
			}
			
			editor.Document.Insert(editor.CaretOffset, formatedBody);
			
			// set caret
			if (caret != -1)
			{
				editor.CaretOffset = caret;
			}
		}
	}
	
	public class SnippetGroup
	{
		public List<Snippet> Snippets { get; set; }
		public StackPanel Panel { get; set; }

		public SnippetGroup()
		{
			Snippets = new List<Snippet>();
		}
	}
	
	public class Snippet
	{
		public string Name { get; set; }
		public string Body { get; set; }
		public string Trigger { get; set; }
	}
}
