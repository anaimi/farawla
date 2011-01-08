using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Farawla.Core;
using Standard;
using System.Windows.Input;
using System.Diagnostics;

namespace Farawla
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			Bootstrapper.Initialize(this);

			Loaded += (s, e) => Controller.Current.Start();
			Closed += (e, s) => Controller.Current.Exit();
			SizeChanged += (s, e) => Controller.Current.Resize();
			
			MouseMove += ChangeSidebarVisibility;
			Drop += (s, e) => Controller.Current.FileDropped((string[])e.Data.GetData(DataFormats.FileDrop));
		}
		
		private void ChangeSidebarVisibility(object sender, MouseEventArgs e)
		{
			if (Sidebar.DontHideSidebar)
				return;
			
			var distance = e.GetPosition(Sidebar).X;
			
			if (distance > -20)
			{
				//Sidebar.Visibility = Visibility.Visible;
							
				Sidebar.Opacity = 1;
			}
			else if (distance > -300)
			{
				//Sidebar.Visibility = Visibility.Visible;
				
				var percentage = distance / 300 * -1;
				Sidebar.Opacity = (1 - percentage);
			}
			else
			{
				Sidebar.Opacity = 0;
				
				//Sidebar.Visibility = Visibility.Collapsed;
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			GlassHelper.ExtendGlassFrameComplete(this);
			GlassHelper.SetWindowThemeAttribute(this, false, false);
		}
		
	}
}
