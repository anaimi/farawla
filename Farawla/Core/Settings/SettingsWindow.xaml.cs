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
using Orientation=System.Windows.Controls.Orientation;
using Path=System.IO.Path;
using Farawla.Utilities;

namespace Farawla.Core
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		private const string PROG_ID = "Farawla";
		
		public Theme Theme { get; private set; }
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
			Theme = Theme.Instance;
			Settings = Settings.Instance;
			
			InitializeComponent();
			
			// cancel all closing events
			Closing += (s, e) => {
				Hide();
				e.Cancel = true;
			};
			
			// close on escape
			KeyDown += (s, e) => {
				if (e.Key == Key.Escape)
					Close();
			};
			
			// close on click
			CloseDialogText.MouseDown += (s, e) => Close();

			// populate themes
			PopulateThemesList();
			
			// populate file associations
			SettingsTab.SelectionChanged += (s, e) => {
				if (SettingsTab.SelectedItem == FileAssociationsTab && FileAssociationList.Items.Count == 0)
				{
					PopulateFileAssociations();
				}
			};
			
			// never highlight a list item in file association list
			FileAssociationList.SelectionChanged += (s, e) => { FileAssociationList.SelectedIndex = -1; };
		}
		
		public new void ShowDialog()
		{
			base.ShowDialog();
		}
		
		#region File Association tab

		private void PopulateFileAssociations()
		{
			// arrange
			var associations = new List<FileAssociation>();
			
			// get all values
			foreach(var lang in Controller.Current.Languages.Items)
				foreach(var asso in lang.Associations)
					associations.Add(new FileAssociation { Extension = "." + asso, Language = lang.Name });
			
			// add default value
			associations.Add(new FileAssociation { Extension = ".txt", Language = "Default"});
			
			// build them
			foreach(var association in associations.OrderBy(a => a.Extension))
			{
				var item = CreateFileAssociationItem(association);

				FileAssociationList.Items.Add(item);
			}
		}
		
		private ListBoxItem CreateFileAssociationItem(FileAssociation association)
		{
			var item = new ListBoxItem();
			
			// get color for language name
			var languageColor = Theme.Instance.TextWidgetColor.ToColor();
			languageColor.A = 20;
			
			// build item
			var container = new StackPanel { Orientation = Orientation.Horizontal };
			var checkbox = new System.Windows.Controls.CheckBox {Content = association.Extension, Margin = new Thickness(0, 0, 10, 0)};
			container.Children.Add(checkbox);
			container.Children.Add(new TextBlock {Text = association.Language, Foreground = new SolidColorBrush(languageColor)});
			item.Content = container;
			
			// checked?
			if (FileTypeAssociation.GetAssociationProgId(association.Extension) == PROG_ID)
			{
				checkbox.IsChecked = true;
			}
			
			// connect events
			checkbox.Click += (s, e) => FileAssociationChanged(checkbox.IsChecked.Value, association);
			
			return item;
		}
		
		private void FileAssociationChanged(bool enabled, FileAssociation association)
		{
			if (enabled)
			{
				FileTypeAssociation.Associate(association.Extension, PROG_ID, "", Settings.ExecPath, Settings.ExecPath);
			}
			else
			{
				FileTypeAssociation.RemoveAssociation(association.Extension);
			}
		}
		
		class FileAssociation
		{
			public string Extension { get; set; }
			public string Language { get; set; }
		}
		
		#endregion

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

		private void ShowFilesStartingWithDotChanged(object sender, RoutedEventArgs e)
		{
			var manager = Controller.Current.Widgets.FirstOrDefault(w => w is Features.Projects.Widget) as Features.Projects.Widget;
			
			manager.RefreshProject();
		}
	}
}