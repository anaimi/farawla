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

	public class ContentToMarginConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new Thickness(0, 0, -((ContentPresenter)value).ActualHeight, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ContentToPathConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var ps = new PathSegmentCollection(4);
			ContentPresenter cp = (ContentPresenter)value;
			double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
			double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
			ps.Add(new LineSegment(new Point(1, 0.7 * h), true));
			ps.Add(new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true));
			ps.Add(new LineSegment(new Point(w, h), true));
			ps.Add(new BezierSegment(new Point(w + 0.6 * h, h), new Point(w + h, 0), new Point(w + h * 1.3, 0), true));
			return ps;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
