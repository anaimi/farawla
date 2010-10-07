using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Farawla.Core;
using Path=System.IO.Path;
using Farawla.Core.Sidebar;

namespace Farawla.Features.Projects
{
	public partial class Widget : IWidget
	{
		#region Widget: File Explorer
		public string WidgetName { get { return "Projects"; } }
		public bool Expandable { get { return true; } }
		public double WidgetHeight { get { return -1; } }
		public BarButton SidebarButton { get; set; }
		#endregion

		public WidgetSettings Settings { get; set; }
		
		public string CurrentProjectPath { get; set; }
		public string LastOpenProject
		{
			get
			{
				return Settings["LastOpenProject"];
			}
			set
			{
				Settings["LastOpenProject"] = value;
			}
		}
		public IEnumerable<string> PreviouslyOpenedProjects
		{
			get
			{
				return Settings["PreviouslyOpenedProjects"].Split(',').ToArray();
			}
		}
		
		public Widget()
		{
			InitializeComponent();

			Loaded += (s, e) => OnLoaded();
			Settings = Core.Settings.Instance.GetWidgetSettings("Projects");
		}

		public void OnLoaded()
		{
			Controller.Current.OnFileDropped += (paths) => {
				foreach(var path in paths)
				{
					if (File.Exists(path))
					{
						Controller.Current.CreateNewTab(path);
						continue;
					}
					else if (!Directory.Exists(path))
					{
						continue;
					}
					else
					{
						OpenProject(path);
					}
				}
			};
			
			OpenProject(LastOpenProject);
		}

		public void OnClick()
		{
			
		}
		
		private void OpenProject(string path)
		{
			if (path.IsBlank() || !Directory.Exists(path))
			{
				NoOpenProject.Visibility = Visibility.Visible;
				ProjectBox.Visibility = Visibility.Collapsed;

				if (SidebarButton != null)
					SidebarButton.SetLabel("Projects");
			}
			else
			{
				AddPreviouslyOpenedProject(path);
				Files.Items.Clear();
				
				NoOpenProject.Visibility = Visibility.Collapsed;
				ProjectBox.Visibility = Visibility.Visible;

				foreach(var item in GetSubItems(path))
					Files.Items.Add(item);

				if (SidebarButton != null)
					SidebarButton.SetLabel(Path.GetFileName(path));
				
				CurrentProjectPath = path;
			}
			
		}
		
		private List<TreeViewItem> GetSubItems(string path)
		{
			var items = new List<TreeViewItem>();
			
			foreach (var dir in Directory.GetDirectories(path))
			{
				items.Add(CreateItem(dir, true));
			}

			foreach (var file in Directory.GetFiles(path))
			{
				items.Add(CreateItem(file, false));
			}

			return items;
		}
		
		private TreeViewItem CreateItem(string path, bool isDirectory)
		{
			var item = new TreeViewItem();

			item.Header = Path.GetFileName(path);

			if (isDirectory)
			{
				item.Expanded += (s, e) => { e.Handled = true; ExpandItem(item, path); };
				item.Items.Add(new TreeViewItem {Header = "loading..."});
			}
			else
			{
				item.Expanded += (s, e) => {
					if (File.Exists(path))
					{
						Controller.Current.CreateNewTab(path);
					}
					else
					{
						Notifier.Notifier.Instance.Show("File not found - consider refreshing the project");
					}
				};
			}
			
			return item;
		}

		private void ExpandItem(TreeViewItem node, string path)
		{
			node.Items.Clear();
			
			foreach(var item in GetSubItems(path))
				node.Items.Add(item);
		}

		private void AddPreviouslyOpenedProject(string path)
		{
			var projects = PreviouslyOpenedProjects.Where(p => p != path).ToArray();
			
			if (projects.Count() > 0)
			{
				Settings["PreviouslyOpenedProjects"] = string.Join(",", projects) + "," + path;
			}
			else
			{
				Settings["PreviouslyOpenedProjects"] = path;
			}
		}

		private void OpenProjectsClicked(object sender, RoutedEventArgs e)
		{
			var projects = PreviouslyOpenedProjects.Where(p => Directory.Exists(p)).ToList();

			if (projects.Count() == 0)
			{
				Notifier.Notifier.Instance.Show("Nothing found in your projects history");
				return;
			}

			projects.Reverse();
			OpenProjectsMenu.Items.Clear();
			
			foreach (var project in projects)
			{
				var path = project;
				var item = new MenuItem();
				item.Header = path;
				
				item.Click += (s, e2) => {
					OpenProject(path); // using "project" instead of "path" will result in Access Closure error.
				};
				
				OpenProjectsMenu.Items.Add(item);
			}
			
			OpenProjectsMenu.IsOpen = true;
		}

		private void RefreshProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenProject(CurrentProjectPath);
		}
	}
}
