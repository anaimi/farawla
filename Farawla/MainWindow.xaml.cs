using System.Windows;
using Farawla.Core;
using System.Windows.Input;

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
			
			RootGrid.MouseLeftButtonDown += (s, e) => {
				if (e.ClickCount >= 2)
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
	}
}
