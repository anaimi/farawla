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
	/// Interaction logic for Slider.xaml
	/// </summary>
	public partial class HueSlider : UserControl
	{
		public Action ColorSelectionChanged { get; set; }
		
		public double Hue { get; set; }

		public HueSlider()
		{
			InitializeComponent();
			
			Hue = 1;
		}
		
		#region mouse down/move
		private void SelectorMouseDown(object sender, MouseButtonEventArgs e)
		{
			MoveSlider(e.GetPosition(Rainbow).Y);
		}

		private void SelectorMouseMove(object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				MoveSlider(e.GetPosition(Rainbow).Y);
			}
		}
		#endregion
		
		private void MoveSlider(double distance)
		{
			// fix distance
			distance = distance - Selector.ActualHeight / 2;
			
			SetHue(distance / HueSliderControl.Height);
			
			UpdateSelectorPosition();

			if (ColorSelectionChanged != null)
				ColorSelectionChanged();
		}

		public HueSlider SetHue(double hue)
		{
			Hue = hue;
			
			if (Hue < 0)
				Hue = 0;
			else if (Hue > 1)
				Hue = 1;
			
			return this;
		}

		public HueSlider UpdateSelectorPosition()
		{
			Selector.SetValue(Canvas.TopProperty, Hue * HueSliderControl.Height);

			return this;
		}
	}
}
