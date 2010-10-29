using System.Collections.Generic;
using System.Windows.Controls;
using Farawla.Core;
using Farawla.Core.Sidebar;
using System.Diagnostics;
using Farawla.Core.Language;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Farawla.Features.Snippets
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public Dictionary<string, StackPanel> Snippets { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Snippets");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;
			
			// initialize
			Snippets = new Dictionary<string, StackPanel>();
			
			// assign on active window change event
			Controller.Current.OnActiveTabChanged += ActiveTabChanged;
			Controller.Current.OnStart += ActiveTabChanged;
		}

		private void ActiveTabChanged()
		{
			var tab = Controller.Current.ActiveTab;
			
			if (!tab.Language.IsDefault)
			{
				// not cached? populate it
				if (!Snippets.ContainsKey(tab.Language.Name))
				{
					var path = tab.Language.Directory;

					if (File.Exists(Core.Settings.ExecDir + "\\" + path + "\\snippets.txt"))
					{
						PopulateSnippets(tab.Language.Name, path + "\\snippets.txt");
					}
					else
					{
						Snippets.Add(tab.Language.Name, null);
					}
				}
				
				// get the cached panel
				var panel = Snippets[tab.Language.Name];
				
				// hide it if null
				if (panel == null)
				{
					ShowNoSnippets();
				}
				
				// show it otherwise
				else
				{
					ShowSnippets(Snippets[tab.Language.Name]);
				}
				
			}
			else
			{
				ShowNoSnippets();
			}
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
			
			Snippets.Add(name, panel);
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
			
			editor.Document.Insert(editor.CaretOffset, snippet.Body);
		}
	}
	
	public class Snippet
	{
		public string Name { get; set; }
		public string Body { get; set; }
		public string Trigger { get; set; }
	}
}
