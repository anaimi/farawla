using Farawla.Core.Sidebar;
namespace Farawla.Features
{
	public interface IWidget
	{
		string WidgetName { get; }
		bool Expandable { get; }
		double WidgetHeight { get; }
		BarButton SidebarButton { get; set; }
		
		void OnClick();
	}
}