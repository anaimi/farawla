using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Farawla.Core;
using Newtonsoft.Json;
using System.Windows.Forms;
using Farawla.Features;
using System.Diagnostics;

namespace Farawla.Utilities
{
	public static class JsonHelper
	{
		public static T Load<T>(string filenameOrPath)
		{
			if (!Path.IsPathRooted(filenameOrPath))
			{
				filenameOrPath = Settings.ExecDir + filenameOrPath;
			}
			
			try
			{
				var obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(filenameOrPath));

				return obj;
			}
			catch (JsonReaderException exception)
			{
				var message =
					"Error while parsing JSON object. Line " + exception.LineNumber + ", Column " + exception.LinePosition + ".\n" +
					filenameOrPath + "\n\n" + 
					"Fix it and try again.";
				
				Notifier.Show(message);
				
				Process.GetCurrentProcess().Kill();
			}

			throw new Exception("Exiting after parse error...");
		}
	}
}
