using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Farawla.Core;
using System.ServiceModel;
using Farawla.Utilities;

namespace Farawla
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static Mutex mutex = new Mutex(true, "Farawla");
		
		protected override void OnStartup(StartupEventArgs e)
		{
			// named pipe configuration
			var pipeConf = new PipeConfiguration {
				Uri = "net.pipe://localhost/Pipe",
				Name = "Farawla"
			};
			
			// passed argument
			var arg = e.Args.Length > 0 ? e.Args[0] : string.Empty;
			
			// check if other instance exists
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				mutex.ReleaseMutex();

				try
				{
					var server = new PipeServer(pipeConf);

					// listen for other instances
					server.Start((action, data) =>
					{
						if (action == "open")
						{
							// open document
							Controller.Current.CreateNewTab(data);

							// activate window
							if (Settings.Instance.IsWindowMaximized)
								Controller.Current.MainWindow.WindowState = WindowState.Maximized;
							else
								Controller.Current.MainWindow.WindowState = WindowState.Normal;

							Controller.Current.MainWindow.Activate();
						}
					});
				}
				catch {  }
			}
			else
			{
				// if arg found, send to original instance
				if (!arg.IsBlank())
				{
					try
					{
						var client = new PipeClient(pipeConf);
						client.Send("open", arg);
					}
					catch {  }
				}
				
				Current.Shutdown();

				return; // just in case Shutdown is not forcing
			}

			base.OnStartup(e);

			// pass arguments if found
			if (!arg.IsBlank())
			{
				Current.Properties["Argument0"] = arg;
			}
		}
	}
}
