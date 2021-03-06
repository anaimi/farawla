﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Farawla.Core;
using Farawla.Utilities;

namespace Farawla.Features.Projects
{
	/// <summary>
	/// Interaction logic for Jump.xaml
	/// </summary>
	public partial class Jump : Window
	{
		private BackgroundWorker indexer;
		private Widget projectManager;
		private List<FileListItem> projectFiles;
		private bool abortIndexing;
		
		public Jump(Widget projectManager)
		{
			InitializeComponent();

			this.projectManager = projectManager;
			projectFiles = new List<FileListItem>();
			
			// initialize indexer
			indexer = new BackgroundWorker();
			indexer.DoWork += (s, e) => IndexDirectory(projectManager.CurrentProjectPath);
			
			// bind events
			Controller.Current.OnProjectOpened += OnProjectOpened;
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.OemComma, ShowBox);
		}
		
		public void ShowBox()
		{
			if (projectManager.CurrentProjectPath.IsBlank())
			{
				Notifier.Growl("Quick Jump", "No project loaded", "Drag and drop a folder to open a project");
				return;
			}
			
			FileName.Focus();
			FileName.SelectAll();
			
			RefreshFileList();

			Show();
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Hide();
				return;
			}
			
			if (e.Key == Key.Down)
			{
				if (Files.Items.Count > Files.SelectedIndex + 1)
					Files.SelectedIndex++;

				return;
			}
			
			if (e.Key == Key.Up)
			{
				if (Files.SelectedIndex >= 1)
					Files.SelectedIndex--;

				return;
			}
			
			if (e.Key == Key.Enter)
			{
				if (Files.SelectedItem != null)
				{
					var path = (string)(Files.SelectedItem as ListBoxItem).Tag;

					Controller.Current.CreateNewTab(projectManager.CurrentProjectPath + "\\" + path);
					
					DelayedAction.Invoke(
						200, 
						() => Controller.Current.CurrentTabs.Last().MakeActive(true)
					);
				}
				
				Hide();
				return;
			}

			RefreshFileList();

			// select first item
			if (Files.Items.Count > 0)
				Files.SelectedIndex = 0;
		}
		
		private void RefreshFileList()
		{
			var query = FileName.Text.ToLower();
			var files = projectFiles.AsQueryable();
			
			Files.Items.Clear();
			
			// filter
			if (!query.IsBlank())
			{
				if (FileName.Text.Contains("\\"))
				{
					files = files.Where(f => SoftMatch(f.Path, query));
				}
				else
				{
					files = files.Where(f => SoftMatch(f.FileName, query)).OrderBy(f => f.FileName.GetLevenshteinDistance(query));
				}
			}
			
			// limit
			files = files.Take(100);
			
			// add
			foreach (var file in files)
			{
				Files.Items.Add(file.GetListBoxItem());
			}
		}
		
		private bool SoftMatch(string path, string query)
		{
			var index = 0;
			
			if (query.Length == 0 || path.Length == 0)
				return query == path;
			
			foreach(var c in path)
			{
				if (c == query[index])
				{
					index++;
					
					if (index == query.Length)
						return true;
				}
			}

			return false;
		}
		
		private void OnProjectOpened(string path)
		{
			// if the indexing thread is busy, request abortion and try again after 200 ms
			if (indexer.IsBusy)
			{
				abortIndexing = true;
				DelayedAction.Invoke(200, () => OnProjectOpened(path));
			}
			
			// not busy? continue
			else
			{
				projectFiles.Clear();
				indexer.RunWorkerAsync();
			}
		}

		private void IndexDirectory(string dir)
		{
			if (abortIndexing)
				return;
			
			foreach (var file in Directory.GetFiles(dir))
			{	
				if (abortIndexing)
					return;
				
				projectFiles.Add(new FileListItem(projectManager.CurrentProjectPath, file));
			}

			foreach (var directory in Directory.GetDirectories(dir))
			{
				if (abortIndexing)
					return;
				
				IndexDirectory(directory);
			}
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);

			Hide();
		}
	}
	
	internal class FileListItem
	{
		public string Path { get; set; }
		public string FileName { get; set; }

		public FileListItem(string projectPath, string filePath)
		{
			Path = filePath.Replace(projectPath + "\\", "");
			FileName = System.IO.Path.GetFileName(filePath).ToLower();
		}
		
		public ListBoxItem GetListBoxItem()
		{
			var item = new ListBoxItem();

			item.Tag = Path;
			item.Content = Path;
			
			return item;
		}
	}
}
