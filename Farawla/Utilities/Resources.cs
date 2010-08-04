using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Farawla.Utilities
{
	class Resources
	{
		public static Stream GetResource(string name)
		{
			var executable = System.Reflection.Assembly.GetExecutingAssembly();
			var stream = executable.GetManifestResourceStream("Farawla.Resources." + name);
			
			return stream;
		}
	}
}
