using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Farawla.Core;
using Path=System.IO.Path;
using Farawla.Core.Sidebar;
using Farawla.Utilities;
using System.Threading;
using System.ComponentModel;

namespace Farawla.Features.Projects
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
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
		public List<FileItem> ExpandedNodes { get; set; }
		public FileItem LastClickedFile { get; set; }
		public List<string> ProjectFiles { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Projects");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;
			SidebarButton.ExpadedByDefault();
			
			// get settings
			Settings = Core.Settings.Instance.GetWidgetSettings("Projects");
			
			// arrange
			ExpandedNodes = new List<FileItem>();

			// assign events
			Loaded += (s, e) => OnLoaded();
			Files.MouseDown += (s, e) => { LastClickedFile = null; };
			
			// create jump box
			ProjectFiles = new List<string>();
			new Jump(this);
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

			Controller.Current.Keyboard.AddBinding(KeyCombination.None, Key.F2, () =>
			{
				if (LastClickedFile != null && LastClickedFile.IsFocused)
				{
					RenameFile(LastClickedFile);
				}
			});

			Controller.Current.Keyboard.AddBinding(KeyCombination.None, Key.Delete, () =>
			{
				if (LastClickedFile != null && LastClickedFile.IsFocused)
				{
					DeleteFile(LastClickedFile);
				}
			});
			
			OpenProject(LastOpenProject);
		}
		
		public void RefreshProject()
		{
			OpenProject(CurrentProjectPath);

			ExpandExpandedNodes(Files.Items);
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
				Controller.Current.ProjectOpened(CurrentProjectPath);
			}
		}
		
		private List<TreeViewItem> GetSubItems(string path)
		{
			var items = new List<TreeViewItem>();
			
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (!Core.Settings.Instance.ShowFilesStartingWithDot && Path.GetFileName(dir).StartsWith("."))
					continue;
				
				items.Add(CreateFileItem(dir, true));
			}

			foreach (var file in Directory.GetFiles(path))
			{
				if (!Core.Settings.Instance.ShowFilesStartingWithDot && Path.GetFileName(file).StartsWith("."))
					continue;
				
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
			item.PreviewMouseDown += (s, e) => { LastClickedFile = item; };
			
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
						Notifier.Show("File not found - consider refreshing the project");
					}
				};
			}

			// build context menu
			item.ContextMenu = new ContextMenu();
			item.ContextMenu.Items.Add(CreateMenuItem("Rename", () => RenameFile(item)));
			item.ContextMenu.Items.Add(new Separator());
			item.ContextMenu.Items.Add(CreateMenuItem("Delete", () => DeleteFile(item)));
			
			if (isDirectory)
			{
				item.ContextMenu.Items.Add(new Separator());
				item.ContextMenu.Items.Add(CreateMenuItem("Create File", () => ShowCreateFile(item.Path)));
				item.ContextMenu.Items.Add(CreateMenuItem("Create Directory", () => ShowCreateDirectory(item.Path)));
			}
						
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
			Notifier.Prompt("Rename", item.Path, item.FileName, (canceled, result) => {
				if (!canceled)
				{
					item.Rename(result);
				}
			});
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

			LastOpenProject = path;
		}

		private void OpenProjectsClicked(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var menu = button.ContextMenu as ContextMenu;
			
			var projects = PreviouslyOpenedProjects.Where(p => Directory.Exists(p)).ToList();

			if (projects.Count() == 0)
			{
				Notifier.Show("Nothing found in your projects history");
				return;
			}

			projects.Reverse();
			menu.Items.Clear();
			
			foreach (var project in projects)
			{
				var path = project;
				menu.Items.Add(CreateMenuItem(path, () => OpenProject(path))); // After a long session of debugging, it was discorvered that using "project" instead of "path" will result in Access Closure error.
			}

			menu.IsOpen = true;
		}

		private void RefreshProjectClicked(object sender, RoutedEventArgs e)
		{
			RefreshProject();
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
		
		#region Create File or Directory
		
		private string GetActivePath()
		{
			if (LastClickedFile == null)
				return CurrentProjectPath;
			
			if (LastClickedFile.IsDirectory)
				return LastClickedFile.Path;

			return Path.GetDirectoryName(LastClickedFile.Path);
		}
		
		private void CreateFileOrDirectoryClicked(object sender, RoutedEventArgs e)
		{
			CreateFileButton.ContextMenu.IsOpen = true;
		}
		
		private void CreateFileClicked(object sender, RoutedEventArgs e)
		{
			var path = GetActivePath();
			
			ShowCreateFile(path);
		}
		
		private void CreateDirectoryClicked(object sender, RoutedEventArgs e)
		{
			var path = GetActivePath();

			ShowCreateDirectory(path);
		}
		
		private void ShowCreateFile(string path)
		{
			Notifier.Prompt("Create File", "Inside " + path, "", (canceled, name) =>
			{
				if (!canceled)
				{
					File.Create(path + "\\" + name);
					
					RefreshProjectClicked(null, null);
					
					DelayedAction.Invoke(250, () => Controller.Current.CreateNewTab(path + "\\" + name));
				}
			});
		}
		
		private void ShowCreateDirectory(string path)
		{
			Notifier.Prompt("Create Directory", "Inside " + path, "", (canceled, name) =>
			{
				if (!canceled)
				{
					Directory.CreateDirectory(path + "\\" + name);
					RefreshProjectClicked(null, null);
				}
			});
		}
		
		#endregion
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
