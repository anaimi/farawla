using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;

public static class FrameworkElementExtension
{	
	private static Dictionary<FrameworkElement, DispatcherTimer> timers = new Dictionary<FrameworkElement,DispatcherTimer>();

	public static FrameworkElement VerticalSlide(this FrameworkElement fe, double to, double duration, Action completed)
	{
		if (timers.ContainsKey(fe))
		{
			timers[fe].Stop();
			timers.Remove(fe);
			
			if (completed != null)
				completed();
		}

		var from = fe.ActualHeight;
		var step = Math.Abs(from - to) / duration;

		if (from > to)
			step *= -1;

		var timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(duration)
		};

		timer.Tick += (s, e) =>
		{
			if (Math.Round(fe.Height, 2) == to)
			{
				timer.Stop();
				timers.Remove(fe);
				
				if (completed != null)
					completed();
			}
			else
			{
				if (step < 0 && fe.Height < Math.Abs(step))
					fe.Height = 0;
				else
					fe.Height += step;
			}
		};

		timers.Add(fe, timer);
		timer.Start();
		
		return fe;
	}
	
	public static FrameworkElement VerticalSlide(this FrameworkElement fe, double to, double duration)
	{
		return VerticalSlide(fe, to, duration, null);
	}
	
	public static void Invoke(this FrameworkElement fe, Action action)
	{
		fe.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate {
			action();
			return null;
		}), null);
	}
}