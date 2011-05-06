using System.Collections.Generic;
using Farawla.Features;

namespace Farawla.Core
{
	public class Bootstrapper
	{
		public static void Initialize(MainWindow instance)
		{
			Controller.Initialize(instance); // ALWAYS first thing to execute

			PopulateFeatureList();
		}
		
		public static void PopulateFeatureList()
		{
			Controller.Current.Widgets.Add(new Features.Projects.Widget());
			Controller.Current.Widgets.Add(new Features.Search.Widget());
			Controller.Current.Widgets.Add(new Features.Snippets.Widget());
			Controller.Current.Widgets.Add(new Features.Completion.Widget());
			Controller.Current.Widgets.Add(new Features.ColorPicker.Widget());
			Controller.Current.Widgets.Add(new Features.Stats.Widget());
			//Controller.Current.Widgets.Add(new Features.Terminal.Widget());
			Controller.Current.Widgets.Add(Settings.Instance);
		}
	}
}