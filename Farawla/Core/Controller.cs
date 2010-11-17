using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Farawla.Core.Language;
using System.Linq;
using System.IO;
using Farawla.Features;
using System.Windows.Input;
using Microsoft.Win32;

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
				foreach(var tab in CurrentTabs)
					if (tab.Tab == MainWindow.Tab.SelectedItem as TabItem)
						return tab;

				return CurrentTabs.Last();
			}
		}

		public event Action OnStart;
		public event Action OnExit;
		public event Action OnResize;
		public event Action OnActiveTabChanged;
		public event Action<WindowTab> OnTabCreated;
		public event Action<string> OnProjectOpened;
		public event Action<string[]> OnFileDropped;
		
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
			
			// assign active window change event
			_current.MainWindow.Tab.SelectionChanged += (s, e) => {
				if (_current.OnActiveTabChanged != null)
					_current.OnActiveTabChanged();
			};
			
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
			var tab = new WindowTab(path);
			CurrentTabs.Add(tab);
			MainWindow.Tab.Items.Insert(index, tab.Tab);

			// inform observers
			if (OnTabCreated != null)
				OnTabCreated(tab);

			// select/show last added item
			tab.MakeActive();
			
			TabCountUpdated();
		}
		
		public void Start()
		{
			#region Bind keyboard shortcuts
			
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.O, BrowseFile);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.S, () => ActiveTab.Save(false));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.S, () => ActiveTab.Save(true));
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.F4, CloseActiveTab);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.T, () => CreateNewTab(""));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.T, OpenLastClosedTab);
			
			// keybindings for ctrl+#
			Keyboard.AddBinding(KeyCombination.Ctrl, KeyboardNumberNavigation, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9);
			Keyboard.AddBinding(KeyCombination.Ctrl, KeyboardNumberNavigation, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9);
			
			#endregion
			
			// inform tab creation observers, since tabs are created before allowing observers to subscribe
			if (OnTabCreated != null)
				CurrentTabs.ForEach(OnTabCreated);
			
			if (OnStart != null)
				OnStart();
		}

		public void Exit()
		{
			// call observers
			if (OnExit != null)
				OnExit();
			
			// save opened tabs
			Settings.Instance.OpenTabs = new List<string>();
			foreach(var tab in CurrentTabs.Where(t => !t.IsNewDocument).OrderBy(t => t.Index))
				Settings.Instance.OpenTabs.Add(tab.DocumentPath);
			
			// save settings
			Settings.Instance.Save();
			
			// DIE!
			Application.Current.Shutdown();
		}

		public void Resize()
		{
			// update sidebar
			MainWindow.Sidebar.UpdateWidgetSize();
			
			// save current state
			Settings.Instance.IsWindowMaximized = MainWindow.WindowState == WindowState.Maximized;
			
			// call observers
			if (OnResize != null)
				OnResize();
		}
		
		public void ProjectOpened(string path)
		{
			if (OnProjectOpened != null)
				OnProjectOpened(path);
		}

		public void FileDropped(string[] files)
		{
			if (OnFileDropped != null)
				OnFileDropped(files);
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
		
		public void KeyboardNumberNavigation(Key key)
		{
			var index = -1;
			
			if (key == Key.D1 || key == Key.NumPad1) index = 0;
			if (key == Key.D2 || key == Key.NumPad2) index = 1;
			if (key == Key.D3 || key == Key.NumPad3) index = 2;
			if (key == Key.D4 || key == Key.NumPad4) index = 3;
			if (key == Key.D5 || key == Key.NumPad5) index = 4;
			if (key == Key.D6 || key == Key.NumPad6) index = 5;
			if (key == Key.D7 || key == Key.NumPad7) index = 6;
			if (key == Key.D8 || key == Key.NumPad8) index = 7;
			if (key == Key.D9 || key == Key.NumPad9) index = 8;
			
			if (index <= -1 || index >= CurrentTabs.Count)
				return;
			
			CurrentTabs[index].MakeActive();
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