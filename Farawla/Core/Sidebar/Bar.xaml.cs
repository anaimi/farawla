using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Farawla.Core.Sidebar
{
	public partial class Bar
	{
		private const double BUTTON_HEIGHT = 26;
		
		public bool DontHideSidebar { get; set; }
		
		public Bar()
		{
			InitializeComponent();

			Loaded += (s, e) => OnLoad();
		}

		public void OnLoad()
		{
			foreach (var button in Controller.Current.Widgets.Select(w => w.SidebarButton))
			{
				var control = AddWidget(button);

				Container.Children.Add(control);
			}
		}

		private UIElement AddWidget(BarButton button)
		{
			// initialize grid
			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BUTTON_HEIGHT) });

			// set properties for widget
			if (button.IsExpandable)
			{
				grid.RowDefinitions.Add(new RowDefinition());

				button.Control.Margin = button.IsExpanded ? new Thickness(5) : new Thickness(5,0,5,0);
				button.Control.SetValue(Grid.RowProperty, 1);

				grid.Children.Add(button.Control);
			}

			// add button
			grid.Children.Add(button);
			
			// update bar on collapse
			button.OnExpand += () => OnButtonExpand(button);
			//button.OnCollapse += UpdateWidgetSize;

			return grid;
		}
		
		public void OnButtonExpand(BarButton button)
		{
			// temporarily, emitate an accordion's behaviour
			
			foreach(var btn in Controller.Current.Widgets.Where(w => w != button.Widget && w.SidebarButton.IsExpanded).Select(b => b.SidebarButton))
				btn.CollapseWidget();
		}

		public void UpdateWidgetSize()
		{
			var workspace = OuterBorder.ActualHeight - OuterBorder.Padding.Top - OuterBorder.Padding.Bottom;
			var buttons = Controller.Current.Widgets.Select(w => w.SidebarButton);

			// reduce workspace, by enumerating the MaxHeights of non-Stretchable widgets
			foreach (var button in buttons.Where(b => !b.IsStretchable && b.IsExpandable && b.IsExpanded))
			{
				button.Control.Height = button.WidgetHeight;
				workspace -= button.WidgetHeight + BUTTON_HEIGHT + 10;
			}

			// reduce workspace for each non-Expandable button
			workspace -= BUTTON_HEIGHT * buttons.Where(b => !b.IsExpandable || !b.IsExpanded).Count();

			// set the Height on Stretchable components
			var stretchables = buttons.Where(b => b.IsStretchable && b.IsExpanded);
			var count = stretchables.Count();

			workspace = (workspace / count) - ((BUTTON_HEIGHT + 10) * count);

			foreach (var button in buttons.Where(b => b.IsStretchable))
			{
				button.WidgetHeight = workspace;
				
				if (button.IsExpanded)
					button.Control.Height = workspace;
			}
		}
	}
}
