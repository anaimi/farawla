using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Farawla.Core.Language;
using ICSharpCode.AvalonEdit;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Linq;
using System.IO;
using Farawla.Features;
using System.Windows.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel;

namespace Farawla.Core
{
	public class Controller
	{
		#region Instance
		private static Controller _current;
		public static Controller Current
		{
			get
			{
				if (_current == null)
					throw new Exception("You must initialize the controller before using it");

				return _current;
			}
		}
		#endregion

		public List<IWidget> Widgets { get; private set; }
		public KeyboardObserver Keyboard { get; private set; }
		public List<WindowTab> CurrentTabs { get; private set; }
		public Languages Languages { get; private set; }
		public MainWindow MainWindow { get; private set; }
		public WindowTab ActiveTab
		{
			get
			{
				if (MainWindow.Tab.SelectedIndex == -1)
					return null;
				
				foreach(var tab in CurrentTabs)
					if (tab.Tab == MainWindow.Tab.SelectedItem as TabItem)
						return tab;
				
				return null;
			}
		}
		public string SelectedText
		{
			get
			{
				return "This is selected text"; // TODO
			}
			set
			{
				throw new Exception("settings selected text is not implemented");
			}
		}

		public static void Initialize(MainWindow instance)
		{
			_current = new Controller();

			_current.MainWindow = instance;
			_current.Widgets = new List<IWidget>();
			_current.CurrentTabs = new List<WindowTab>();
			_current.Keyboard = new KeyboardObserver();
			_current.Languages = new Languages();
			
			// maximize window if it was maximized
			if (Settings.Instance.IsWindowMaximized)
				_current.MainWindow.WindowState = WindowState.Maximized;
			else
				_current.MainWindow.WindowState = WindowState.Normal;
			
			// open files that were open
			if (Settings.Instance.OpenTabs.Count > 0)
			{
				foreach(var tab in Settings.Instance.OpenTabs)
					if (File.Exists(tab))
						_current.CreateNewTab(tab);
			}
			
			// never show zero tabs
			if (_current.CurrentTabs.Count == 0)
				_current.TabCountUpdated();
		}
		
		public void CreateNewTab(string path)
		{
			CreateNewTab(path, MainWindow.Tab.Items.Count);
		}
		
		public void CreateNewTab(string path, int index)
		{
			// if already open, make active
			if (CurrentTabs.Any(t => t.DocumentPath == path))
			{
				CurrentTabs.First(t => t.DocumentPath == path).MakeActive();
				ActiveTab.Tab.Focus();
				return;
			}
			
			// validate index
			if (index < 0)
				index = 0;
			else if (index > MainWindow.Tab.Items.Count)
				index = MainWindow.Tab.Items.Count;

			// create and add new window
			var window = new WindowTab(path);
			CurrentTabs.Add(window);
			MainWindow.Tab.Items.Insert(index, window.Tab);

			// select/show las added item
			window.MakeActive();

			// inform observers
			TabCountUpdated();
		}
		
		public void OnStart()
		{
			#region Bind keyboard shortcuts
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.O, BrowseFile);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.S, () => ActiveTab.Save(false));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.S, () => ActiveTab.Save(true));
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.F4, CloseActiveTab);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.T, () => CreateNewTab(""));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.T, OpenLastClosedTab);
			#endregion
			
			Widgets.ForEach(f => f.OnStart());
		}

		public void OnExit()
		{
			// save opened tabs
			Settings.Instance.OpenTabs = new List<string>();
			foreach(var tab in CurrentTabs.Where(t => !t.IsNewDocument).OrderBy(t => t.Index))
				Settings.Instance.OpenTabs.Add(tab.DocumentPath);
			
			// inform widgets
			Widgets.ForEach(f => f.OnExit());
			
			// save settings
			Settings.Instance.Save();
			
			// DIE!
			System.Windows.Application.Current.Shutdown();
		}

		public void OnResize()
		{
			// inform Widgets
			Widgets.ForEach(f => f.OnResize());
			
			// update sidebar
			MainWindow.Sidebar.UpdateWidgetSize();
			
			// save current state
			Settings.Instance.IsWindowMaximized = MainWindow.WindowState == WindowState.Maximized;
		}
		
		public void TabCountUpdated()
		{
			if (CurrentTabs.Count == 0)
			{
				CreateNewTab("");
			}
		}

		public void BrowseFile()
		{
			var dialog = new OpenFileDialog();

			if (dialog.ShowDialog().Value)
			{
				Controller.Current.CreateNewTab(dialog.FileName);
			}
		}

		public void CloseActiveTab()
		{
			Controller.Current.ActiveTab.Close();
		}
		
		public void OpenLastClosedTab()
		{
			if (Settings.Instance.ClosedTabs.Count > 0)
			{
				// open it
				var tab = Settings.Instance.ClosedTabs.First();
				CreateNewTab(tab.Path, tab.Index);
				
				// make it active
				MainWindow.Tab.SelectedIndex = tab.Index;
				
				// update list of last closed tabs
				Settings.Instance.ClosedTabs = Settings.Instance.ClosedTabs.Skip(1).ToList();
			}
		}

		#region Overlay Show/Hide
		public void ShowOverlay()
		{
			MainWindow.Overlay.Visibility = Visibility.Visible;
		}

		public void HideOverlay()
		{
			MainWindow.Overlay.Visibility = Visibility.Collapsed;
		}
		#endregion
	}
}