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
using Farawla.Utilities;

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
		public List<FileItem> ExpandedNodes { get; set; }
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
			ExpandedNodes = new List<FileItem>();
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
				items.Add(CreateFileItem(dir, true));
			}

			foreach (var file in Directory.GetFiles(path))
			{
				items.Add(CreateFileItem(file, false));
			}

			return items;
		}

		private FileItem CreateFileItem(string path, bool isDirectory)
		{
			var item = new FileItem();

			item.Path = path;
			item.IsDirectory = isDirectory;
			item.Header = Path.GetFileName(path);
			
			// assign event handlers
			if (isDirectory)
			{
				item.Expanded += (s, e) => { e.Handled = true; ExpandItem(item); };
				item.Collapsed += (s, e) => { e.Handled = true; CollapseItem(item); };
				
				item.Items.Add(new FileItem {Header = "loading..."});
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

			// build context menu
			item.ContextMenu = new ContextMenu();
			item.ContextMenu.Items.Add(CreateMenuItem("Rename", () => RenameFile(item)));
			item.ContextMenu.Items.Add(CreateMenuItem("Delete", () => DeleteFile(item)));
			
			return item;
		}
		
		private MenuItem CreateMenuItem(string label, Action click)
		{
			var item = new MenuItem();

			item.Header = label;
			item.Click += (s, e) => click();

			return item;
		}

		private void RenameFile(FileItem item)
		{
			var input = new ModalInputBox("Rename", "Rename", item.Path, item.FileName, (canceled, inputStr) => {
				if (!canceled)
				{
					item.Rename(inputStr);
				}
			});

			input.ShowDialog();
		}
		
		private void DeleteFile(FileItem item)
		{
			var result = MessageBox.Show("Move '" + Path.GetFileName(item.Path) + "' to Recycle Bin?", item.Path, MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				FileOperationAPIWrapper.MoveToRecycleBin(item.Path);
				DelayedAction.Invoke(500, () => RefreshProjectClicked(null, null));
			}
		}

		private void ExpandItem(FileItem node)
		{
			node.Items.Clear();
			
			foreach(var item in GetSubItems(node.Path))
				node.Items.Add(item);

			if (!ExpandedNodes.Any(f => f.Path == node.Path))
				ExpandedNodes.Add(node);
		}
		
		private void CollapseItem(FileItem node)
		{
			var oldNode = ExpandedNodes.FirstOrDefault(f => f.Path == node.Path);
			
			if (oldNode != null)
				ExpandedNodes.Remove(oldNode);
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
				OpenProjectsMenu.Items.Add(CreateMenuItem(path, () => OpenProject(path))); // After a long session of debugging, it was discorvered that using "project" instead of "path" will result in Access Closure error.
			}
			
			OpenProjectsMenu.IsOpen = true;
		}

		private void RefreshProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenProject(CurrentProjectPath);
			
			ExpandExpandedNodes(Files.Items);
		}
		
		private void ExpandExpandedNodes(ItemCollection items)
		{
			foreach(var item in items)
			{
				if (!(item is FileItem))
					continue;

				var file = item as FileItem;
				
				if (ExpandedNodes.Any(f => f.Path == file.Path))
				{
					file.IsExpanded = true;
					
					ExpandExpandedNodes(file.Items);
				}
			}
		}
	}
	
	public class FileItem : TreeViewItem
	{
		public bool IsDirectory { get; set; }
		public string Path { get; set; }
		
		public string FileName
		{
			get { return System.IO.Path.GetFileName(Path); }
		}
		
		public void Rename(string newName)
		{
			var newPath = System.IO.Path.GetDirectoryName(Path) + "\\" + newName;
			
			if (newPath == Path)
				return;
			
			if (IsDirectory)
			{
				Directory.Move(Path, newPath);
			}
			else
			{
				File.Move(Path, newPath);
			}

			var tab = Controller.Current.CurrentTabs.Where(t => t.DocumentPath == Path).FirstOrDefault();
			
			if (tab != null)
				tab.Rename(newPath);

			Path = newPath;
			Header = FileName;
		}
	}
}
