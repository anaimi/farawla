using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Farawla.Features.Notifier
{
	class Notifier
	{
		#region Widget: Notifier
		public string WidgetName { get { return "Notifier"; } }
		#endregion
		
		#region instance
		private static Notifier _instance;
		public static Notifier Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Notifier();

					return _instance;
			}
		}
		#endregion
		
		public void Show(string message)
		{
			MessageBox.Show(message);
		}
	}
}
