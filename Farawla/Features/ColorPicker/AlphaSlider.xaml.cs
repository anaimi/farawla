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

namespace Farawla.Features.ColorPicker
{
	/// <summary>
	/// Interaction logic for AlphaSlider.xaml
	/// </summary>
	public partial class AlphaSlider : UserControl
	{
		public Action ColorSelectionChanged { get; set; }
		
		public double Alpha { get; private set; }
		
		public AlphaSlider()
		{
			InitializeComponent();
			
			Alpha = 1;
		}
		
		#region mouse down/move
		private void SelectorMouseDown(object sender, MouseButtonEventArgs e)
		{
			MoveSlider(e.GetPosition(AlphaGradiant).Y);
		}

		private void SelectorMouseMove(object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				MoveSlider(e.GetPosition(AlphaGradiant).Y);
			}
		}
		#endregion

		private void MoveSlider(double distance)
		{
			// fix distance
			distance = distance - Selector.ActualHeight / 2;

			SetAlpha(1 - distance / AlphaSliderControl.Height);
			
			UpdateSliderPosition();

			if (ColorSelectionChanged != null)
				ColorSelectionChanged();
		}
		
		public AlphaSlider SetAlpha(double alpha)
		{
			Alpha = alpha;
			
			if (Alpha < 0)
				Alpha = 0;
			else if (Alpha > 1)
				Alpha = 1;
			
			return this;
		}
		
		public AlphaSlider UpdateSliderPosition()
		{
			Selector.SetValue(Canvas.TopProperty, (1 - Alpha) * AlphaSliderControl.Height);
			
			return this;
		}
	}
}
