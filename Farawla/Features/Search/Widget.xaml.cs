using Farawla.Core.Sidebar;
using Farawla.Core;
using System.Windows.Input;

namespace Farawla.Features.Search
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		
		public Widget()
		{
			InitializeComponent();
			
			// create sidebar button
			SidebarButton = new BarButton(this, "Search");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 140;
			
			// add keyboard shortcut
			Controller.Current.Keyboard.AddBinding(KeyCombination.None, Key.F3, ShowWidgetAndGetFocus);
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.F, ShowWidgetAndGetFocus);
		}
		
		private void ShowWidgetAndGetFocus()
		{
			SidebarButton.ExpandWidget();

			Query.Focus();
		}
	}
}
