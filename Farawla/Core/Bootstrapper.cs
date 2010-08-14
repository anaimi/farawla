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
			Controller.Current.Features.Add(new Features.FileExplorer.Widget());
			Controller.Current.Features.Add(new Features.ColorPicker.Widget());
		}
	}
}