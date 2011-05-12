using System.Windows;
using System.Windows.Forms;

namespace Farawla.Core
{
	/// <summary>
	/// Interaction logic for Container.xaml
	/// </summary>
	public partial class NotifyContainer : Window
	{
		#region instance

		private static NotifyContainer _instance;
		public static NotifyContainer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new NotifyContainer();
				}

				return _instance;
			}
		}
		
		#endregion
		
		public NotifyContainer()
		{
			InitializeComponent();

			Height = Controller.Current.MainWindow.ActualHeight;
		}
		
		private void PositionWindow()
		{
			var point = Controller.Current.MainWindow.Sidebar.GetElementLocationOnLeft(Width);

			Top = point.Y;
			Left = point.X;
		}
		
		public void AddBox(NotifyBox box)
		{
			Container.Children.Add(box);
			
			// show sidebar
			Controller.Current.MainWindow.Sidebar.DontHideSidebar = true;
			Controller.Current.MainWindow.Sidebar.Opacity = 0.6;
			
			PositionWindow();
			Show();
		}
		
		public void RemoveBox(NotifyBox box)
		{
			Container.Children.Remove(box);
			
			if (Container.Children.Count == 0)
			{
				Controller.Current.MainWindow.Sidebar.DontHideSidebar = false;				
				
				Hide();
			}
		}
	}
}
