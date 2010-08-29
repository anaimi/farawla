using System;
using System.Windows;
using System.Windows.Input;
using Farawla.Core;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;

namespace Farawla.Features.FileExplorer
{
	public partial class Widget : IWidget
	{
		#region Widget: File Explorer
		public string WidgetName { get { return "File Explorer"; } }
		public bool Expandable { get { return true; } }
		public double WidgetHeight { get { return -1; } }
		#endregion
		
		public WidgetSettings Settings { get; set; }
		public List<FileItem> ExpandedDirectories { get; set; }
		
		public Widget()
		{
			InitializeComponent();
			Settings = Farawla.Core.Settings.Instance.GetWidgetSettings("FileExplorer");

			ExpandedDirectories = new List<FileItem>();

			OpenInitialFileList(Settings["Path"]);
		}
		
		public void OnStart()
		{
			
		}
		
		public void OnExit()
		{
			
		}
		
		public void OnResize()
		{
			
		}
		
		private void OpenInitialFileList(string path)
		{
			// add drives
			var index = 0;
			foreach (var drive in Directory.GetLogicalDrives())
			{
				var item = CreateItem(new FileItem(drive, FileType.Drive));
				list.AddItem(index++, null, item);
			}
			
			// build path parts
			var parts = path.Split('\\').Where(p => p != string.Empty);
			
			// open each part
			var newPath = string.Empty;
			foreach(var part in parts)
			{
				newPath += part + "\\";
				
				var item = GetListBoxItemByPath(newPath);
				OnItemClick(item);
			}
		}
		
		public void PopulateFileList(ListBoxItem parent, string path)
		{
			var index = 0;
			
			if (parent != null)
			{
				index = list.GetIndex(parent) + 1;
			}

			try
			{
				// add directories
				foreach (var dir in Directory.GetDirectories(path))
				{
					var item = CreateItem(new FileItem(dir, FileType.Directory));
					list.AddItem(index++, parent, item);
				}

				// add files
				foreach (var file in Directory.GetFiles(path))
				{
					var item = CreateItem(new FileItem(file, FileType.File));
					list.AddItem(index++, parent, item);
				}
			}
			catch (Exception e)
			{
				Notifier.Notifier.Instance.Show(e.Message);
			}
		}

		private void OnItemClick(ListBoxItem item)
		{
			var file = item.Tag as FileItem;
			
			if (file.Type == FileType.File)
			{
				Controller.Current.CreateNewTab(file.Path);
			}
			else
			{
				if (ExpandedDirectories.Contains(file))
				{
					CollapseItem(item);
				}
				else
				{
					Settings["Path"] = file.Path;
					ExpandItem(item);
				}
			}
		}
		
		private void ExpandItem(ListBoxItem item)
		{
			var file = item.Tag as FileItem;
			
			ExpandedDirectories.Add(file);
			PopulateFileList(item, file.Path);
		}
		
		private void CollapseItem(ListBoxItem item)
		{
			var file = item.Tag as FileItem;

			list.Collapse(item);
			ExpandedDirectories.Remove(file);
		}
		
		private ListBoxItem CreateItem(FileItem file)
		{
			var prefix = new StringBuilder();
			var item = new ListBoxItem();
			
			for(var i = 0; i < file.Level; i++)
				prefix.Append("    ");
			
			item.Tag = file;
			item.Content = prefix + file.Name;
			
			if (file.Type == FileType.Drive || file.Type == FileType.Directory)
			{
				item.FontWeight = FontWeights.Bold;
				item.Foreground = new SolidColorBrush(Colors.White);
			}
			
			return item;
		}
		
		public ListBoxItem GetListBoxItemByPath(string path)
		{
			foreach(ListBoxItem item in list.Items)
			{
				var file = item.Tag as FileItem;

				if (file.Path == path || file.Path + "\\" == path)
					return item;
			}
			
			throw new Exception("Directory '" + path + "' was not found");
		}
		
		public void OnClick()
		{
		
		}
	}

	public class FileItem
	{
		public string Path { get; set; }
		public FileType Type { get; set; }
		public string Name
		{
			get
			{
				if (Type == FileType.Drive)
					return Path.Extract("", ":");
				
				return System.IO.Path.GetFileName(Path);
			}
		}
		public int Level
		{
			get
			{
				if (Type == FileType.Drive)
					return 0;
				
				return Path.Count(c => c == '\\');
			}
		}

		public FileItem(string path, FileType type)
		{
			Path = path;
			Type = type;
		}
	}
	
	public enum FileType
	{
		File,
		Directory,
		Drive
	}
}
