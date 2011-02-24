using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Farawla.Core;
using System.ServiceModel;
using Farawla.Utilities;
using System.IO;
using Newtonsoft.Json;
using Farawla.Features;
using System.Net;

namespace Farawla
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private const string EXCEPTION_URL = "http://localhost:4567/exception";
		
		private static Mutex mutex = new Mutex(true, "Farawla");
		
		protected override void OnStartup(StartupEventArgs e)
		{
			// unhandleed exceptions
			AppDomain.CurrentDomain.UnhandledException += ExceptionOccured;
			
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

		private void ExceptionOccured(object sender, UnhandledExceptionEventArgs e)
		{
			var answer = MessageBox.Show("An error occured - may I submit it?\nNo personal details will be included.", "KaBOOOM!", MessageBoxButton.YesNo);
			
			if (answer == MessageBoxResult.Yes)
			{
				var exception = e.ExceptionObject as Exception;

				if (exception == null)
					return;

				while (true)
				{
					if (exception.InnerException == null)
						break;

					exception = exception.InnerException;
				}

				// arrange
				var form = new NameValueCollection();
				var client = new WebClient();

				// set values
				form["version"] = "dev";
				form["os"] = Environment.OSVersion.ToString();
				form["message"] = exception.Message;
				form["trace"] = exception.StackTrace;

				// post it
				client.UploadValues(EXCEPTION_URL, "POST", form);
			}
			
			Process.GetCurrentProcess().Kill();
		}
	}
}
