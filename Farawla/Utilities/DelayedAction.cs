using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Timers;

namespace Farawla.Utilities
{
	public class DelayedAction
	{
		public static void Invoke(int delayMS, Action action)
		{
			var timer = new DispatcherTimer();

			timer.Interval = TimeSpan.FromMilliseconds(delayMS);
			
			timer.Tick += (s, e) => {
				if (action != null)
					action();
				
				timer.Stop();
			};
			
			timer.Start();
		}
	}
	
	public class WindowDelayAction
	{
		private Timer timer;
		private Action action;

		public WindowDelayAction(int delayMS, Action action)
		{
			timer = new Timer();
			
			timer.Interval = delayMS;
			timer.Elapsed += TimerTick;
			
			this.action = action;
		}
		
		public void Start()
		{
			if (timer.Enabled)
				timer.Stop();
			
			timer.Start();
		}

		private void TimerTick(object sender, ElapsedEventArgs e)
		{
			if (action != null)
				action();
			
			timer.Stop();
		}
	}
}
