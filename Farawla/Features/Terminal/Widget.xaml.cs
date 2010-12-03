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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Farawla.Core.Sidebar;
using Farawla.Core;

namespace Farawla.Features.Terminal
{
	/// <summary>
	/// Interaction logic for Widget.xaml
	/// </summary>
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Terminal");
			SidebarButton.ShowInSidebar = false;
			
			// connect to events
			Controller.Current.OnStart += OnStart;
			Controller.Current.OnResize += OnResize;
		}
		
		private void OnStart()
		{
			Controller.Current.MainWindow.RootGrid.Children.Add(this);

			Visibility = Visibility.Visible;
		}
		
		private void OnResize()
		{
			Height = Controller.Current.MainWindow.ActualHeight / 3;
		}

	}
}
