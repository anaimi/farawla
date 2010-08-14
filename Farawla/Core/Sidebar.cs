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
using System.Windows.Media.Effects;

namespace Farawla.Core
{
	public partial class Sidebar : UserControl
	{
		private const double BUTTON_HEIGHT = 26;

		public Border OuterBorder { get; set; }
		public StackPanel Container { get; set; }

		public Sidebar()
		{
			// initialize OuterBorder
			OuterBorder = new Border()
			{
				Background = new SolidColorBrush("#66666666".ToColor()),
				CornerRadius = new CornerRadius(4),
				Padding = new Thickness(0, 5, 0, 5)
			};

			// initialize Container
			Container = new StackPanel()
			{
				VerticalAlignment = VerticalAlignment.Stretch,
			};

			// assign components
			OuterBorder.Child = Container;
			Content = OuterBorder;

			// assign events
			Loaded += (s, e) => OnLoad();
		}

		public void OnLoad()
		{
			foreach (var widget in Controller.Current.Features.Where(w => w is UserControl))
			{
				Container.Children.Add(AddWidget(widget as UserControl));
			}
		}
		
		private UIElement AddWidget(UserControl widget)
		{
			var height = widget.Height;
			var feature = widget as IFeature;
			
			// initialize grid
			var grid = new Grid();
			var contentRow = new RowDefinition();
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BUTTON_HEIGHT) });
			grid.RowDefinitions.Add(contentRow);
			
			// initialize button
			var button = new Border {
				Background = new SolidColorBrush("#60606060".ToColor()),
				Padding = new Thickness(5),
				Child = new TextBlock {
					Foreground = new SolidColorBrush("#FFFFFFFF".ToColor()),
					Text = feature.WidgetName,
					Effect = new DropShadowEffect {
						Opacity = 0.5,
						ShadowDepth = 9,
						Direction = 542,
						BlurRadius = 9
					}
				},
				Cursor = Cursors.Hand
			};
			
			// set properties for widget
			widget.Margin = new Thickness(5);
			widget.SetValue(Grid.RowProperty, 1);
			
			// set properties for button
			button.SetValue(Grid.RowProperty, 0);
			button.MouseEnter += (s, e) => button.Background = new SolidColorBrush("#FF606060".ToColor());
			button.MouseLeave += (s, e) => button.Background = new SolidColorBrush(Colors.Transparent);
			button.MouseLeftButtonDown += (s, e) => {
				if (Math.Round(widget.Height, 2) == 0)
				{
					ExpandWidget(widget, height);
				}
				else
				{
					CollapseWidget(widget);
				}
			};
			
			// rounded corners for button?
			//if (Container.Children.Count == 0)
			//    button.CornerRadius = new CornerRadius(4, 4, 0, 0);
			
			// add components
			grid.Children.Add(button);
			grid.Children.Add(widget);
			
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
			// update widgets
			var workspace = Controller.Current.MainWindow.ActualHeight - OuterBorder.Padding.Top - OuterBorder.Padding.Bottom;
			var widgets = Controller.Current.Features.Where(f => f is UserControl).Select(f => f as UserControl);

			// reduce workspace, by enumerating the MaxHeights of non-Stretchable widgets
			foreach (var widget in widgets.Where(w => w.VerticalContentAlignment != VerticalAlignment.Stretch))
			{
				workspace -= BUTTON_HEIGHT;
				workspace -= widget.Height;
			}

			// set the Height on Stretchable components
			var count = widgets.Where(w => w.VerticalContentAlignment == VerticalAlignment.Stretch).Count();
			workspace = workspace / count - BUTTON_HEIGHT / count;
			foreach (var widget in widgets.Where(w => w.VerticalContentAlignment == VerticalAlignment.Stretch))
			{
				widget.Height = workspace;
			}
		}
	}
}