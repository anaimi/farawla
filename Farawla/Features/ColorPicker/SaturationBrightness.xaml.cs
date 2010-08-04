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
using System.Diagnostics;

namespace Farawla.Features.ColorPicker
{
	/// <summary>
	/// Interaction logic for Saturation.xaml
	/// </summary>
	public partial class SaturationBrightness : UserControl
	{
		public Action ColorSelectionChanged { get; set; }
		
		public double Saturation { get; private set; }
		public double Brightness { get; private set; }
		
		public SaturationBrightness()
		{
			InitializeComponent();
			
			Saturation = 0.5;
			Brightness = 0.5;
		}

		#region shades mouse move/down
		private void ShadesMouseMove(object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				MoveSelector(e.GetPosition(Shades));
			}
		}

		private void ShadesMouseDown(object sender, MouseButtonEventArgs e)
		{
			MoveSelector(e.GetPosition(Shades));
		}
		#endregion

		private void MoveSelector(Point position)
		{
			// get fixed values
			var x = position.X - Selector.Width / 2;
			var y = position.Y - Selector.Height / 2;
			
			// set saturation and brightness
			SetSaturation(x / Shades.ActualWidth);
			SetBrightness(1 - y / Shades.ActualHeight);
			
			UpdateSelectorPosition();

			if (ColorSelectionChanged != null)
				ColorSelectionChanged();
		}

		public SaturationBrightness SetSampleColor(Color color)
		{
			SampleColor.Fill = new SolidColorBrush(color);
			
			return this;
		}

		public SaturationBrightness SetSaturation(double saturation)
		{
			Saturation = saturation;

			if (Saturation < 0)
				Saturation = 0;
			else if (Saturation > 1)
				Saturation = 1;
			
			return this;
		}

		public SaturationBrightness SetBrightness(double brightness)
		{
			Brightness = brightness;

			if (Brightness < 0)
				Brightness = 0;
			else if (Brightness > 1)
				Brightness = 1;
			
			return this;
		}
		
		public SaturationBrightness UpdateSelectorPosition()
		{
			Selector.SetValue(Canvas.LeftProperty, Saturation * SampleColor.Width);
			Selector.SetValue(Canvas.TopProperty, (1 - Brightness) * WhiteGradient.Height);
			
			return this;
		}
	}
}
