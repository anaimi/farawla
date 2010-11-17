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
using Farawla.Features;

namespace Farawla.Core.Sidebar
{
	public partial class BarButton
	{
		public event Action OnClick;
		public event Action OnExpand;
		public event Action OnCollapse;
		
		public IWidget Widget { get; private set; }
		public UserControl Control { get; private set; }
		
		public double WidgetHeight { get; set; }
		
		public bool IsExpandable { get; set; }
		public bool IsStretchable { get; set; }
		public bool IsExpanded { get; private set; }
		
		private bool isUserControl;
		
		public BarButton(IWidget widget, string name)
		{
			Widget = widget;
			InitializeComponent();
			
			// assign values
			isUserControl = widget is UserControl;
			Control = widget as UserControl;
			SetLabel(name);

			// default is collapsed
			IsExpanded = false;
			
			// defaults for control
			if (isUserControl)
			{
				Control.Height = 0;
				Control.Visibility = Visibility.Collapsed;
			}
			
			// assign events
			MouseLeftButtonDown += (s, e) => ButtonClicked();
		}
		
		public void SetLabel(string label)
		{
			Label.Text = label;
		}
		
		public void ButtonClicked()
		{
			if (OnClick != null)
				OnClick();

			if (!isUserControl)
				return;

			if (Math.Round(Control.Height, 2) == 0)
			{
				ExpandWidget(null);
			}
			else
			{
				CollapseWidget();
			}
		}

		public void CollapseWidget()
		{
			if (OnCollapse != null)
				OnCollapse();
			
			Control.VerticalSlide(0, 10, () =>
			{
				Control.Margin = new Thickness(Control.Margin.Left, 0, Control.Margin.Right, 0);
				Control.Visibility = Visibility.Collapsed;
			});

			IsExpanded = false;
		}

		public void ExpandWidget(Action callback)
		{
			Control.Visibility = Visibility.Visible;
			
			if (OnExpand != null)
				OnExpand();

			Control.Margin = new Thickness(Control.Margin.Left, 5, Control.Margin.Right, 5);
			Control.VerticalSlide(WidgetHeight, 10, callback);

			IsExpanded = true;
		}

		public void ExpadedByDefault()
		{
			IsExpanded = true;
			Loaded += (s, e) => ExpandWidget(null);
		}
	}
}
