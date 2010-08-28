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
using Farawla.Core;
using System.Windows.Forms;

namespace Farawla.Features.Settings
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		#region Instance
		private static SettingsWindow _instance;
		public static SettingsWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SettingsWindow();
				
				return _instance;
			}
		}
		#endregion
		
		public SettingsWindow()
		{
			InitializeComponent();
			
			Closing += (s, e) => {
				Hide();
				Controller.Current.HideOverlay();
				e.Cancel = true;
			};
			
			KeyDown += (s, e) => {
				if (e.Key == System.Windows.Input.Key.Escape)
					Close();
			};
		}
	}
}
