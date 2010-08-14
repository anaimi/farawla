namespace Farawla.Features
{
	public interface IFeature
	{
		string WidgetName { get; }
		
		void OnStart();
		void OnExit();
		void OnResize();
	}
}