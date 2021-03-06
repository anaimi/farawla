﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Farawla.Features;
using Farawla.Utilities;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Farawla.Core
{
	public class Theme
	{
		public const string DEFAULT_EDITOR_MATCHING_TOKENS_BACKGROUND = "#FFFFFF00";
		public const string DEFAULT_EDITOR_MATCHING_BRACKETS_BACKGROUND = "#FFFFEE78";
		
		public const string DEFAULT_EDITOR_SHOW_TAB_COLOR = "#FFCCCCCC";
		public const string DEFAULT_EDITOR_SHOW_SPACE_COLOR = "#FFAAAAAA";
		public const string DEFAULT_EDITOR_LINE_OF_CARET = "#22FFFF00";
		
		public const string DEFAULT_PRIMARY_WIDGET_COLOR = "#44606060";
		public const string DEFAULT_SECONDARY_WIDGET_COLOR = "#99C0C0C0";
		public const string DEFAULT_TEXT_WIDGET_COLOR = "#FFFFFFFF";

		public const string DEFAULT_EDITOR_BACKGROUND = "#FFFFFFFF";
		public const string DEFAULT_EDITOR_FOREGROUND = "#FF000000";
		public const string DEFAULT_EDITOR_FONT_FAMILY = "Courier New";

		public const string DEFAULT_WINDOW_TAB_SELECTED_COLOR = "#FF00FF00";
		public const string DEFAULT_WINDOW_TAB_HOVER_COLOR = "#FFFF0000";
		public const string DEFAULT_WINDOW_TAB_INACTIVE_COLOR = "#FF0000FF";
		public const string DEFAULT_WINDOW_TAB_SELECTED_CAPTION_COLOR = "#FFFFFFFF";
		public const string DEFAULT_WINDOW_TAB_INACTIVE_CAPTION_COLOR = "#FFAAAAAA";
		public const string DEFAULT_WINDOW_TAB_TOOLBAR_COLOR = "#FF00FF00";
		
		public const string DIRECTORY_NAME = "themes";
		
		#region Instance
		
		private static Theme _instance;
		public static Theme Instance
		{
			get
			{
				if (_instance == null)
				{
					var path = Settings.ExecDir + DIRECTORY_NAME + "\\" + Settings.Instance.ThemeName + "\\theme.js";
					
					if (!File.Exists(path))
					{
						Notifier.Show("The theme '" + Settings.Instance.ThemeName + "' does not exist in the folder '" + DIRECTORY_NAME + "'");
						
						_instance = new Theme();
					}
					else
					{
						_instance = JsonHelper.Load<Theme>(path) ?? new Theme();
						
					}
				}

				return _instance;
			}
		}
		
		#endregion
		
		public string MatchingTokensBackground { get; set; }
		public string MatchingBracketsBackground { get; set; }
		
		public string ShowTabColor { get; set; }
		public string ShowSpaceColor { get; set; }

		public string PrimaryWidgetColor { get; set; }
		public string SecondaryWidgetColor { get; set; }
		public string TextWidgetColor { get; set; }
		
		public string CompletionWindowBackground { get; set; }
		public string CompletionWindowForeground { get; set; }

		public bool HighlightLineOfCaret { get; set; }
		public string LineOfCaretColor { get; set; }
		
		public string Background { get; set; }
		public string Foreground { get; set; }
		public string FontFamily { get; set; }

		public string WindowTabSelectedColor { get; set; }
		public string WindowTabHoverColor { get; set; }
		public string WindowTabInactiveColor { get; set; }
		public string WindowTabSelectedCaptionColor { get; set; }
		public string WindowTabInactiveCaptionColor { get; set; }
		public string WindowTabToolbarColor { get; set; }
		
		public Dictionary<string, string> SyntaxColors { get; set; }
		
		public ImageSource ObjectIcon { get; private set; }
		public ImageSource FunctionIcon { get; private set; }
		public ImageSource SnippetIcon { get; private set; }
		
		public ImageSource BackgroundImage { get; private set; }
		public Rect BackgroundImageViewport { get; private set; }
		
		public Theme()
		{
			// default values
			MatchingTokensBackground = DEFAULT_EDITOR_MATCHING_TOKENS_BACKGROUND;
			MatchingBracketsBackground = DEFAULT_EDITOR_MATCHING_BRACKETS_BACKGROUND;
			
			ShowTabColor = DEFAULT_EDITOR_SHOW_TAB_COLOR;
			ShowSpaceColor = DEFAULT_EDITOR_SHOW_SPACE_COLOR;
			LineOfCaretColor = DEFAULT_EDITOR_LINE_OF_CARET;

			PrimaryWidgetColor = DEFAULT_PRIMARY_WIDGET_COLOR;
			SecondaryWidgetColor = DEFAULT_SECONDARY_WIDGET_COLOR;
			TextWidgetColor = DEFAULT_TEXT_WIDGET_COLOR;
			
			Background = DEFAULT_EDITOR_BACKGROUND;
			Foreground = DEFAULT_EDITOR_FOREGROUND;
			FontFamily = DEFAULT_EDITOR_FONT_FAMILY;

			WindowTabSelectedColor = DEFAULT_WINDOW_TAB_SELECTED_COLOR;
			WindowTabHoverColor = DEFAULT_WINDOW_TAB_HOVER_COLOR;
			WindowTabInactiveColor = DEFAULT_WINDOW_TAB_INACTIVE_COLOR;
			WindowTabSelectedCaptionColor = DEFAULT_WINDOW_TAB_SELECTED_CAPTION_COLOR;
			WindowTabInactiveCaptionColor = DEFAULT_WINDOW_TAB_INACTIVE_CAPTION_COLOR;
			WindowTabToolbarColor = DEFAULT_WINDOW_TAB_TOOLBAR_COLOR;
			
			// colors
			SyntaxColors = new Dictionary<string, string>();

			// icons
			ObjectIcon = LoadIcon("object.png");
			FunctionIcon = LoadIcon("function.png");
			SnippetIcon = LoadIcon("snippet.png");
			
			// background dimensions
			BackgroundImage = LoadIcon("background.png");
			if (BackgroundImage != null)
				BackgroundImageViewport = new Rect(0, 0, BackgroundImage.Width, BackgroundImage.Height);
		}
		
		private ImageSource LoadIcon(string name)
		{
			var path = Settings.ExecDir + "themes\\" + Settings.Instance.ThemeName + "\\" + name;
			
			if (!File.Exists(path))
				return null;

			var source = new BitmapImage();
			
			source.BeginInit();
			source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
			source.EndInit();

			return source;
		}
		
		public Color GetColor(string key)
		{
			if (SyntaxColors.ContainsKey(key))
				return SyntaxColors[key].ToColor();
			
			if (key.Contains(" ") && key != " ")
			{
				// replace double space, if found
				while(key.Contains("  "))
					key.Replace("  ", " ");
				
				return GetColor(key.Substring(0, key.LastIndexOf(' ')));
			}

			return Foreground.ToColor();
		}
	}

	public class ThemeColorConverter : MarkupExtension, IValueConverter
	{
		public static SolidColorBrush DefaultColor = new SolidColorBrush(Colors.Orange);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var colorName = parameter as string;

			if (colorName == null)
				return DefaultColor;

			return GetColor(colorName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
		
		public static Brush GetColor(string colorNameAndOpacity)
		{
			double opacity = 1.0;
			Brush color;
			
			// get opacity if specified
			if (colorNameAndOpacity.Contains(','))
			{
				opacity = colorNameAndOpacity.Extract(",", "").ToDouble();
				colorNameAndOpacity = colorNameAndOpacity.Extract("", ",");
			}

			// get color by name from instance
			var colorInfo = Theme.Instance.GetType().GetProperty(colorNameAndOpacity);
			var colorStr = colorInfo.GetValue(Theme.Instance, new object[0]) as string;

			if (colorStr == null)
				return DefaultColor;

			// gradient?
			if (colorStr.Contains(","))
			{
				var parts = colorStr.Split(',');
				color = new LinearGradientBrush(parts[0].ToColor(), parts[1].ToColor(), 90);
			}
			else
			{
				color = new SolidColorBrush(colorStr.ToColor());
			}


			// apply opacity
			if (opacity < 1)
			{
				color.Opacity = opacity;
			}

			return color;
		}
	}
	
	public class ThemeImageConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var imageName = parameter as string;
			var path = Settings.ExecDir + Theme.DIRECTORY_NAME + "\\" + Settings.Instance.ThemeName + "\\" + imageName;
			
			if (!File.Exists(path))
				return null;

			return new BitmapImage(new Uri(path));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
