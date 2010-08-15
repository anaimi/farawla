namespace Farawla.Features
{
	public interface IWidget
	{
		string WidgetName { get; }
		bool Expandable { get; }
		double WidgetHeight { get; }
		
		void OnStart();
		void OnExit();
		void OnResize();
		
		void OnClick();
	}
}