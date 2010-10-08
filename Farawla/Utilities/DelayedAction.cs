using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Farawla.Utilities
{
	public class DelayedAction
	{
		public static void Invoke(int delayInMelliSeconds, Action action)
		{
			var timer = new DispatcherTimer();

			timer.Interval = TimeSpan.FromMilliseconds(delayInMelliSeconds);
			
			timer.Tick += (s, e) => {
				if (action != null)
					action();
				
				timer.Stop();
			};
			
			timer.Start();
		}
	}
}
