using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class Bar
	{
		private const double BUTTON_HEIGHT = 26;
		
		public Bar()
		{
			InitializeComponent();

			Loaded += (s, e) => OnLoad();
		}

		public void OnLoad()
		{
			foreach (var widget in Controller.Current.Widgets)
			{
				var control = AddWidget(widget);

				Container.Children.Add(control);
			}
		}

		private UIElement AddWidget(IWidget widget)
		{
			var isControl = widget is UserControl;
			var control = widget as UserControl;

			var height = isControl ? control.Height : 0;

			// initialize grid
			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BUTTON_HEIGHT) });

			// set properties for widget
			if (isControl)
			{
				grid.RowDefinitions.Add(new RowDefinition());

				control.Margin = new Thickness(5);
				control.SetValue(Grid.RowProperty, 1);

				grid.Children.Add(control);
			}

			// initialize button
			var button = new BarButton(widget.WidgetName, () =>
			{

				widget.OnClick();

				if (!isControl)
					return;

				if (Math.Round(control.Height, 2) == 0)
				{
					ExpandWidget(control, height);
				}
				else
				{
					CollapseWidget(control);
				}
			});

			// add button
			grid.Children.Add(button);
			
			// assign button to widget
			widget.SidebarButton = button;

			return grid;
		}

		private void CollapseWidget(UserControl widget)
		{
			widget.VerticalSlide(0, 10, () => {
				widget.Margin = new Thickness(widget.Margin.Left, 0, widget.Margin.Right, 0);
			});
		}

		private void ExpandWidget(UserControl widget, double height)
		{
			widget.Margin = new Thickness(widget.Margin.Left, 5, widget.Margin.Right, 5);
			widget.VerticalSlide(height, 10);
		}

		public void UpdateWidgetSize()
		{
			var workspace = OuterBorder.ActualHeight - OuterBorder.Padding.Top - OuterBorder.Padding.Bottom;
			var widgets = Controller.Current.Widgets.Where(f => f is UserControl).Select(f => f as UserControl);

			// reduce workspace, by enumerating the MaxHeights of non-Stretchable widgets
			foreach (var widget in widgets.Where(w => w.VerticalContentAlignment != VerticalAlignment.Stretch))
			{
				widget.Height = (widget as IWidget).WidgetHeight;
				workspace -= widget.Height + BUTTON_HEIGHT + 10;
			}

			// reduce workspace for each non-Expandable button
			workspace -= BUTTON_HEIGHT * Controller.Current.Widgets.Count(w => !(w is UserControl));

			// set the Height on Stretchable components
			var stretchables = widgets.Where(w => w.VerticalContentAlignment == VerticalAlignment.Stretch);
			var count = stretchables.Count();

			workspace = (workspace / count) - ((BUTTON_HEIGHT + 10) * count);

			foreach (var widget in stretchables)
			{
				widget.Height = workspace;
			}
		}
	}
}
