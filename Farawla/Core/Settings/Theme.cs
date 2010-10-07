﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Farawla.Features.Notifier;
using Newtonsoft.Json;

namespace Farawla.Core
{
	public class Theme
	{
		public const string DEFAULT_EDITOR_BACKGROUND = "#FFFFFFFF";
		public const string DEFAULT_EDITOR_FOREGROUND = "#FF000000";
		public const string DEFAULT_EDITOR_FONT_FAMILY = "Courier New";
		
		public const string DIRECTORY_NAME = "themes";
		
		#region Instance
		private static Theme _instance;
		public static Theme Instance
		{
			get
			{
				if (_instance == null)
				{
					if (!Directory.Exists(DIRECTORY_NAME))
					{
						Notifier.Instance.Show("The directory '" + DIRECTORY_NAME + "', which is supposed to include the themes, does not exists. I'll create it, but you'll have to load it with themes (and activate one of the themes).");
						Directory.CreateDirectory(DIRECTORY_NAME);

						_instance = new Theme();
					}
					else if (!File.Exists(DIRECTORY_NAME + "\\" + Settings.Instance.ThemeName))
					{
						Notifier.Instance.Show("The theme '" + Settings.Instance.ThemeName + "' does not exist in the folder '" + DIRECTORY_NAME + "'");
						_instance = new Theme();
					}
					else
					{
						_instance = JsonConvert.DeserializeObject<Theme>(File.ReadAllText(DIRECTORY_NAME + "\\" + Settings.Instance.ThemeName)) ?? new Theme();
					}
				}

				return _instance;
			}
		}
		#endregion
		
		public string Background { get; set; }
		public string Foreground { get; set; }
		public string FontFamily { get; set; }
		
		public Dictionary<string, string> SyntaxColors { get; set; }

		public Theme()
		{
			Background = DEFAULT_EDITOR_BACKGROUND;
			Foreground = DEFAULT_EDITOR_FOREGROUND;
			FontFamily = DEFAULT_EDITOR_FONT_FAMILY;
			
			SyntaxColors = new Dictionary<string, string>();
		}
		
		public Color GetColor(string key)
		{
			if (SyntaxColors.ContainsKey(key))
				return SyntaxColors[key].ToColor();

			return Foreground.ToColor();
		}
	}
}