using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Farawla.Core
{
	/// <summary>
	/// Interaction logic for NotifyBox.xaml
	/// </summary>
	public partial class NotifyBox : UserControl
	{
		private DispatcherTimer timer;
		
		public NotifyBox(string title, string desc, string footer, bool stick)
		{
			InitializeComponent();
			
			if (stick)
			{
				MouseDown += (s, e) => Hide();
			}
			else
			{
				timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromSeconds(3);
				timer.Tick += TimerTick;
				timer.Start();
			}

			Title.Text = title.IsBlank() ? "" : title;
			Description.Text = desc;
			Footer.Text = footer.IsBlank() ? "" : footer;
		}

		private void TimerTick(object sender, EventArgs e)
		{
			Hide();
			
			timer.Tick -= TimerTick;
			timer.Stop();
		}
		
		public void Hide()
		{
			this.VerticalSlide(0, 10, () => NotifyContainer.Instance.RemoveBox(this));
			
		}
	}
}
