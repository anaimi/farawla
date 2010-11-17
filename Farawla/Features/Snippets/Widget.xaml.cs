using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Farawla.Core;
using Farawla.Core.Sidebar;
using System.Diagnostics;
using Farawla.Core.Language;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Text;

namespace Farawla.Features.Snippets
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public Dictionary<string, SnippetGroup> Snippets { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Snippets");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;
			
			// initialize
			Snippets = new Dictionary<string, SnippetGroup>();
			
			// assign on active window change event
			Controller.Current.OnActiveTabChanged += ActiveTabChanged;
			Controller.Current.OnStart += ActiveTabChanged;
			
			// assign on text enter event
			Controller.Current.OnTabCreated += (tab) => {
				tab.Editor.TextArea.KeyUp += (s, e) => OnTextEntered(tab, e);
			};
		}

		private void ActiveTabChanged()
		{
			var tab = Controller.Current.ActiveTab;
			
			if (!tab.Language.IsDefault)
			{
				// not cached? populate it
				if (!Snippets.ContainsKey(tab.Language.Name))
				{
					var path = Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\snippets.js";

					if (File.Exists(path))
					{
						PopulateSnippets(tab.Language.Name, path);
					}
					else
					{
						Snippets.Add(tab.Language.Name, new SnippetGroup());
					}
				}
				
				// get the cached panel
				var snippets = Snippets[tab.Language.Name];
				
				// hide it if null
				if (snippets == null || snippets.Snippets.Count == 0)
				{
					ShowNoSnippets();
				}
				
				// show it otherwise
				else
				{
					ShowSnippets(Snippets[tab.Language.Name].Panel);
				}
				
			}
			else
			{
				ShowNoSnippets();
			}
		}

		private void OnTextEntered(WindowTab tab, KeyEventArgs e)
		{
			if (!Snippets.ContainsKey(tab.Language.Name) || e.Key != Key.Tab)
				return;

			var group = Snippets[tab.Language.Name];
			var line = tab.Editor.Document.GetLineByOffset(tab.Editor.CaretOffset);
			var length = tab.Editor.CaretOffset - line.Offset - 1;
			
			if (length <= 0)
				return;
			
			var text = tab.Editor.Document.GetText(line.Offset, length);
			var snippet = group.Snippets.FirstOrDefault(s => text.EndsWith(s.Trigger));
			
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
			
			var json = File.ReadAllText(path);
			var list = JsonConvert.DeserializeObject<List<Snippet>>(json);
			
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
		
		private void ShowSnippets(StackPanel panel)
		{
			NoSnippets.Visibility = Visibility.Collapsed;
			Container.Visibility = Visibility.Visible;
			
			Container.Child = panel;
		}
		
		private void ShowNoSnippets()
		{
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
