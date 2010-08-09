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
			Controller.Current.Features.Add(Controller.Current.MainWindow.Sidebar.FileExplorer);
			Controller.Current.Features.Add(Controller.Current.MainWindow.Sidebar.ColorPicker);
		}
	}
}