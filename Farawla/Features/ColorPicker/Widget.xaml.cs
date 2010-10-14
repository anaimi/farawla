using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Farawla.Core;
using Farawla.Core.Sidebar;

namespace Farawla.Features.ColorPicker
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public WidgetSettings Settings { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Colors");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 235;
			
			// get settings
			Settings = Core.Settings.Instance.GetWidgetSettings("ColorPicker");
			
			// assign events to selectors
			HueSelector.ColorSelectionChanged += OnColorChanged;
			AlphaSelector.ColorSelectionChanged += OnColorChanged;
			SaturationSelector.ColorSelectionChanged += OnColorChanged;
			
			// assign update events to AHSB
			CurrentColorAHSB_A.KeyUp += (s, e) => UpdateColorFromAHSB();
			CurrentColorAHSB_H.KeyUp += (s, e) => UpdateColorFromAHSB();
			CurrentColorAHSB_S.KeyUp += (s, e) => UpdateColorFromAHSB();
			CurrentColorAHSB_B.KeyUp += (s, e) => UpdateColorFromAHSB();
			
			// assign update events to ARGB
			CurrentColorARGB_A.KeyUp += (s, e) => UpdateColorFromARGB();
			CurrentColorARGB_R.KeyUp += (s, e) => UpdateColorFromARGB();
			CurrentColorARGB_G.KeyUp += (s, e) => UpdateColorFromARGB();
			CurrentColorARGB_B.KeyUp += (s, e) => UpdateColorFromARGB();
			
			// assign update event to HEX
			CurrentColorHex.KeyUp += (s, e) => SetColor(CurrentColorHex.Text);
			
			// read previous color
			SetColor(Settings.KeyExists("Color") ? Settings["Color"] : "#FFFF0000");
		}
		
		public void UpdateColorFromAHSB()
		{
			if (!CurrentColorAHSB_A.Text.IsDouble() || !CurrentColorAHSB_H.Text.IsDouble() || !CurrentColorAHSB_S.Text.IsDouble() || !CurrentColorAHSB_B.Text.IsDouble())
				return;
			
			if (!CurrentColorAHSB_A.Text.ToDouble().IsBetween(0, 1) || !CurrentColorAHSB_H.Text.ToDouble().IsBetween(0, 1) || !CurrentColorAHSB_S.Text.ToDouble().IsBetween(0, 1) || !CurrentColorAHSB_B.Text.ToDouble().IsBetween(0, 1))
				return;
			
			AlphaSelector.SetAlpha(CurrentColorAHSB_A.Text.ToDouble()).UpdateSliderPosition();
			HueSelector.SetHue(CurrentColorAHSB_H.Text.ToDouble()).UpdateSelectorPosition();
			SaturationSelector.SetSaturation(CurrentColorAHSB_S.Text.ToDouble())
							  .SetBrightness(CurrentColorAHSB_B.Text.ToDouble())
							  .UpdateSelectorPosition();
			
			OnColorChanged();
		}
		
		public void UpdateColorFromARGB()
		{
			if (!CurrentColorARGB_A.Text.IsInteger() || !CurrentColorARGB_R.Text.IsInteger() || !CurrentColorARGB_G.Text.IsInteger() || !CurrentColorARGB_B.Text.IsInteger())
				return;

			if (!CurrentColorARGB_A.Text.ToInteger().IsBetween(0, 255) || !CurrentColorARGB_R.Text.ToInteger().IsBetween(0, 255) || !CurrentColorARGB_G.Text.ToInteger().IsBetween(0, 255) || !CurrentColorARGB_B.Text.ToInteger().IsBetween(0, 255))
				return;

			var color = Color.FromArgb((byte)CurrentColorARGB_A.Text.ToInteger(), (byte)CurrentColorARGB_R.Text.ToInteger(), (byte)CurrentColorARGB_G.Text.ToInteger(), (byte)CurrentColorARGB_B.Text.ToInteger());
			SetColor(color);
		}
		
		public void SetColor(string hex)
		{
			hex = CPUtilities.MakeValidColorString(hex);
			SetColor(CPUtilities.ColorFromString(hex));
		}
		
		public void SetColor(Color color)
		{
			double a = 0.0, h = 0.0, s = 0.0, b = 0.0;
			CPUtilities.AHSBFromColor(color, ref a, ref h, ref s, ref b);

			HueSelector.SetHue(h).UpdateSelectorPosition();
			SaturationSelector.SetSaturation(s).SetBrightness(b).UpdateSelectorPosition();
			AlphaSelector.SetAlpha(a).UpdateSliderPosition();

			OnColorChanged();
		}
		
		public void OnColorChanged()
		{
			// get current color
			var color = CPUtilities.ColorFromHSB(HueSelector.Hue, SaturationSelector.Saturation, SaturationSelector.Brightness);
			color.A = Convert.ToByte(AlphaSelector.Alpha * 255);
			
			// update sample and current color
			SaturationSelector.SetSampleColor(CPUtilities.ColorFromHSB(HueSelector.Hue, 1, 1));
			CurrentColor.Background = new SolidColorBrush(color);
			
			// update AHSV
			if (Keyboard.FocusedElement != CurrentColorAHSB_A)
				CurrentColorAHSB_A.Text = AlphaSelector.Alpha.Round(2).ToString();
			if (Keyboard.FocusedElement != CurrentColorAHSB_H)
				CurrentColorAHSB_H.Text = HueSelector.Hue.Round(2).ToString();
			if (Keyboard.FocusedElement != CurrentColorAHSB_S)
				CurrentColorAHSB_S.Text = SaturationSelector.Saturation.Round(2).ToString();
			if (Keyboard.FocusedElement != CurrentColorAHSB_B)
				CurrentColorAHSB_B.Text = SaturationSelector.Brightness.Round(2).ToString();
			
			// update ARGB
			CurrentColorARGB_A.Text = color.A.ToString();
			CurrentColorARGB_R.Text = color.R.ToString();
			CurrentColorARGB_G.Text = color.G.ToString();
			CurrentColorARGB_B.Text = color.B.ToString();
			
			// update Hex
			if (Keyboard.FocusedElement != CurrentColorHex)
				CurrentColorHex.Text = CPUtilities.StringFromColor(color);
			
			// remember it for next time
			Settings["Color"] = CPUtilities.StringFromColor(color);
		}
	}
}
