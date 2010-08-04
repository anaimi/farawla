namespace Farawla.Features
{
	public interface IFeature
	{
		void OnStart();
		void OnExit();
		void OnResize();
	}
}