using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Farawla.Core.Language;
using System.Linq;
using System.IO;
using Farawla.Core.TabContext;
using Farawla.Features;
using System.Windows.Input;
using Microsoft.Win32;
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
		public List<Tab> CurrentTabs { get; private set; }
		public Languages Languages { get; private set; }
		public MainWindow MainWindow { get; private set; }
		public Tab ActiveTab { get; set; }

		public event Action OnStart;
		public event Action OnExit;
		public event Action OnResize;
		public event Action OnActiveTabChanged;
		public event Action<Tab> OnTabCreated;
		public event Action<string> OnProjectOpened;
		public event Action<string[]> OnFileDropped;
		public event Action<EditorSegment> OnContextLanguageChanged;
		
		public static void Initialize(MainWindow instance)
		{
			_current = new Controller();

			_current.MainWindow = instance;
			_current.Widgets = new List<IWidget>();
			_current.CurrentTabs = new List<Tab>();
			_current.Keyboard = new KeyboardObserver();
			_current.Languages = new Languages();
			
			// load languages
			_current.Languages.LoadLanguages();
			
			// maximize window if it was maximized
			if (Settings.Instance.IsWindowMaximized)
				_current.MainWindow.WindowState = WindowState.Maximized;
			else
				_current.MainWindow.WindowState = WindowState.Normal;
			
			// assign active window change event
			_current.MainWindow.Tab.SelectionChanged += (s, e) => {
				var item = _current.MainWindow.Tab.SelectedItem as TabItem;
				
				// set active tab
				if (item != null)
				{
					_current.ActiveTab = item.Tag as Tab;
				}
				
				// notify active tab
				if (_current.ActiveTab != null)
				{
					_current.ActiveTab.MadeActive();
				}
				
				// notify active tab changed listeners
				if (_current.OnActiveTabChanged != null)
				{
					_current.OnActiveTabChanged();
				}

				// notify context listeners
				if (_current.ActiveTab != null)
				{
					_current.ActiveSegmentChanged(new EditorSegment(_current.ActiveTab));
				}
			};

			// open files that were open
			if (Settings.Instance.OpenTabs.Count > 0)
			{
				foreach (var tab in Settings.Instance.OpenTabs)
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
			if (CurrentTabs.Any(t => t.DocumentPath == path && !t.IsNewDocument))
			{
				CurrentTabs.First(t => t.DocumentPath == path).MakeActive(true);
				ActiveTab.TabItem.Focus();
				return;
			}
			
			// remove empty & unsaved "new" tab if found
			if (CurrentTabs.Count == 1 && CurrentTabs[0].IsNewDocument && CurrentTabs[0].Editor.Text == "" && !path.IsBlank())
			{
				MainWindow.Tab.Items.Remove(CurrentTabs[0].TabItem);
				CurrentTabs.Remove(CurrentTabs[0]);
			}
			
			// validate index
			if (index < 0)
				index = 0;
			else if (index > MainWindow.Tab.Items.Count)
				index = MainWindow.Tab.Items.Count;

			// create and add new window
			var tab = new Tab(path);
			CurrentTabs.Add(tab);
			MainWindow.Tab.Items.Insert(index, tab.TabItem);

			// inform observers
			if (OnTabCreated != null)
				OnTabCreated(tab);

			// select/show last added item
			tab.MakeActive(true);
			
			TabCountUpdated();
		}
		
		public void Start()
		{	
			#region Bind keyboard shortcuts
			
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.O, BrowseFile);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.S, () => ActiveTab.Save(false));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.S, () => ActiveTab.Save(true));
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.F4, CloseActiveTab);
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.N, () => CreateNewTab(""));
			Keyboard.AddBinding(KeyCombination.Ctrl, Key.T, () => CreateNewTab(""));
			Keyboard.AddBinding(KeyCombination.Ctrl | KeyCombination.Shift, Key.T, OpenLastClosedTab);		
			
			// keybindings for ctrl+#
			Keyboard.AddBinding(KeyCombination.Ctrl, KeyboardNumberNavigation, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9);
			Keyboard.AddBinding(KeyCombination.Ctrl, KeyboardNumberNavigation, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9);
			//Keyboard.AddBinding(KeyCombination.Ctrl, KeyboardArrowNavigation, Key.Left, Key.Right);			
			
			#endregion
			
			// inform tab creation observers, since tabs are created before allowing observers to subscribe
			if (OnTabCreated != null)
				CurrentTabs.ForEach(OnTabCreated);
			
			// inform onstart observers
			if (OnStart != null)
				OnStart();

			// inform segment changed observers
			if (OnContextLanguageChanged != null && ActiveTab != null && ActiveTab.ActiveLanguageSegment != null)
				OnContextLanguageChanged(ActiveTab.ActiveLanguageSegment);
			
			// check command-line args
			if (App.Current.Properties["Argument0"] != null)
				CreateNewTab(App.Current.Properties["Argument0"].ToString());
			
			// open readme if first time
			if (Settings.Instance.IsFirstTime)
			{
				var path = Settings.ExecDir + "README";
				
				if (File.Exists(path))
					CreateNewTab(path);
			}
				
		}

		public void Closing(CancelEventArgs args)
		{
			// ask to save unsaved documents
			foreach (var tab in CurrentTabs)
			{
				if (!tab.PromptToSave())
				{
					args.Cancel = true;
					return;
				}
			}
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
			Settings.Instance.IsFirstTime = false;
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

			MainWindow.Title = Path.GetFileName(path);
		}

		public void FileDropped(string[] files)
		{
			if (OnFileDropped != null)
				OnFileDropped(files);
		}
		
		public void ActiveSegmentChanged(EditorSegment segment)
		{
			if (OnContextLanguageChanged != null)
				OnContextLanguageChanged(segment);
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
				
				if (!File.Exists(tab.Path))
					return;
				
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
			
			CurrentTabs[index].MakeActive(true);
		}

		private void KeyboardArrowNavigation(Key key)
		{
			if (CurrentTabs.Count == 0)
				return;
			
			if (key == Key.Left)
			{
				var target = CurrentTabs.FirstOrDefault(t => t.Index == ActiveTab.Index - 1);
				
				if (target == null)
					return;
				
				target.MakeActive(true);
			}
			else if (key == Key.Right)
			{
				var target = CurrentTabs.FirstOrDefault(t => t.Index == ActiveTab.Index + 1);

				if (target == null)
					return;

				target.MakeActive(true);
			}
		}
		
		public T GetWidget<T>()
		{
			return (T)Widgets.First(w => w is T);
		}
	}
}