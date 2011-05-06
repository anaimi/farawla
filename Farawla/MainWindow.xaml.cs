using System.Windows;
using Farawla.Core;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Farawla
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			Style = (Style)Resources["GlassStyle"];
			
			Bootstrapper.Initialize(this);

			Loaded += (s, e) => Controller.Current.Start();
			Closing += (s, e) => Controller.Current.Closing(e);
			Closed += (s, e) => Controller.Current.Exit();
			SizeChanged += (s, e) => Controller.Current.Resize();
			
			MouseMove += ChangeSidebarVisibility;
			Drop += (s, e) => Controller.Current.FileDropped((string[])e.Data.GetData(DataFormats.FileDrop));
			
			// root grid double click to maximize or minimize
			RootGrid.MouseLeftButtonDown += (s, e) => {
				if (e.ClickCount >= 2 && e.MouseDevice.DirectlyOver is TabPanel)
				{
					if (WindowState == WindowState.Maximized)
					{
						WindowState = WindowState.Normal;
					}
					else
					{
						WindowState = WindowState.Maximized;
					}
				}                              	
			};
			
			// root grid to move
			RootGrid.MouseMove += (s, e) => {
				if (e.LeftButton != MouseButtonState.Pressed)
					return;
				
				if (!(e.MouseDevice.DirectlyOver is TabPanel))
					return;
				
				DragMove();
			};
		}
		
		private void ChangeSidebarVisibility(object sender, MouseEventArgs e)
		{
			if (Sidebar.DontHideSidebar)
				return;
			
			var distance = e.GetPosition(Sidebar).X;
			
			if (distance > -20)
			{
				Sidebar.Opacity = 1;
			}
			else if (distance > -300)
			{
				var percentage = distance / 300 * -1;
				Sidebar.Opacity = (1 - percentage);
			}
			else
			{
				Sidebar.Opacity = 0;
			}
		}

		protected override void OnSourceInitialized(System.EventArgs e)
		{
			base.OnSourceInitialized(e);
			
			Sidebar.UpdateWidgetSize();
		}
	}
}
