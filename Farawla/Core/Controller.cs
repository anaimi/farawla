using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Linq;
using System.IO;
using Farawla.Features;

namespace Farawla.Core
{
	public class Controller
	{
		#region instance
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

		public List<IFeature> Features = new List<IFeature>();
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
			// if already open, make active
			if (CurrentTabs.Any(t => t.DocumentPath == path))
			{
				CurrentTabs.First(t => t.DocumentPath == path).MakeActive();
				ActiveTab.Tab.Focus();
				return;
			}
			
			// create and add new window
			var window = new WindowTab(path);
			CurrentTabs.Add(window);
			MainWindow.Tab.Items.Add(window.Tab);
			
			// select/show las added item
			window.MakeActive();
			
			// inform observers
			TabCountUpdated();
		}
		
		public void OnStart()
		{
			Features.ForEach(f => f.OnStart());
		}

		public void OnExit()
		{
			// save opened tabs
			Settings.Instance.OpenTabs = new List<string>();
			foreach(var tab in CurrentTabs.Where(t => !t.DocumentPath.IsBlank()))
				Settings.Instance.OpenTabs.Add(tab.DocumentPath);
			
			// inform widgets
			Features.ForEach(f => f.OnExit());
			
			// save settings
			Settings.Instance.Save();
			
			// DIE!
			Application.Current.Shutdown();
		}

		public void OnResize()
		{
			// inform features
			Features.ForEach(f => f.OnResize());
			
			#region resize widgets
			// update widgets
			var workspace = MainWindow.SidebarContainer.ActualHeight;
			var widgets = Features.Where(f => f is UserControl).Select(f => f as UserControl);
			
			// reduce workspace, by enumerating the MaxHeights of non-Stretchable widgets
			foreach(var widget in widgets.Where(w => w.VerticalContentAlignment != VerticalAlignment.Stretch))
			{
				workspace -= widget.MaxHeight;
			}
			
			// set the Height on Stretchable components
			workspace = workspace / widgets.Where(w => w.VerticalContentAlignment == VerticalAlignment.Stretch).Count();
			foreach(var widget in widgets.Where(w => w.VerticalContentAlignment == VerticalAlignment.Stretch))
			{
				widget.Height = workspace;
			}
			#endregion
			
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
	}

	public class WindowTab
	{
		public string Name { get; set; }
		public string DocumentPath { get; set; }

		public TabItem Tab { get; private set; }
		public TextEditor Editor { get; private set; }
		
		public WindowTab(string path)
		{
			var extension = path.Substring(path.LastIndexOf('.') + 1);
			var language = Controller.Current.Languages.GetLanguage(extension);
			
			Name = path.IsBlank() ? "new" : path.Substring(path.LastIndexOf('\\') + 1);
			DocumentPath = path;
			
			// editor
			Editor = new TextEditor();
			Editor.FontFamily = new FontFamily(language.FontFamily);
			Editor.FontSize = 13;
			Editor.ShowLineNumbers = true;
			Editor.Background = new SolidColorBrush(language.Background.ToColor());
			Editor.Foreground = new SolidColorBrush(language.Foreground.ToColor());
			
			// load?
			if (!path.IsBlank())
				Editor.Load(path);
			
			// tab
			Tab = new TabItem();
			Tab.Header = Name;
			Tab.Content = Editor;
			
			// syntax highlighter
			if (language.Name != "Default")
			{
				HighlightingManager.Instance.RegisterHighlighting(language.Name, language.Associations.ToArray(), language.GetHighlighter());
				Editor.SyntaxHighlighting = language.GetHighlighter();
			}
		}
		
		public void MakeActive()
		{
			var index = Controller.Current.CurrentTabs.IndexOf(this);
			Controller.Current.MainWindow.Tab.SelectedIndex = index;
		}
		
		public void Close()
		{
			Controller.Current.MainWindow.Tab.Items.Remove(Tab);
			Controller.Current.CurrentTabs.Remove(this);
			
			Controller.Current.TabCountUpdated();
		}
	}
}