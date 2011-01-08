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

namespace Farawla.Core.TabContext
{
	/// <summary>
	/// Interaction logic for ExtendedTabControl.xaml
	/// </summary>
	public partial class ExtendedTabControl : TabControl
	{
		public ExtendedTabControl()
		{
			InitializeComponent();
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
