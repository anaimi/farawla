using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using Path=System.IO.Path;

namespace Farawla.Core
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public Settings Settings { get; private set; }
		
		#region Instance
		private static SettingsWindow _instance;
		public static SettingsWindow Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SettingsWindow();
				}
				
				return _instance;
			}
		}
		#endregion
		
		public SettingsWindow()
		{
			Settings = Settings.Instance;
			
			InitializeComponent();
			
			PopulateThemesList();
			
			Closing += (s, e) => {
				Hide();
				Controller.Current.HideOverlay();
				e.Cancel = true;
			};
			
			KeyDown += (s, e) => {
				if (e.Key == Key.Escape)
					Close();
			};
		}
		
		private void PopulateThemesList()
		{
			var path = Settings.ExecDir + Theme.DIRECTORY_NAME;
			
			if (!Directory.Exists(path))
				return;
			
			foreach(var dir in Directory.GetDirectories(path))
			{
				var name = Path.GetFileName(dir);
				var option = new ComboBoxItem();

				option.Content = name;

				if (name == Settings.Instance.ThemeName)
					option.IsSelected = true;
				
				option.Selected += (s, e) => {
					Settings.Instance.ThemeName = name;
				};
				
				cbThemes.Items.Add(option);
			}
		}

		private void ShowTabsAndSpacesChanged(object sender, RoutedEventArgs e)
		{
			foreach(var tab in Controller.Current.CurrentTabs)
			{
				tab.BlockHighlighter.Redraw();
			}
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ShowFilesStartingWithDotChanged(object sender, RoutedEventArgs e)
		{
			var manager = Controller.Current.Widgets.FirstOrDefault(w => w is Features.Projects.Widget) as Features.Projects.Widget;
			
			manager.RefreshProject();
		}
	}
}