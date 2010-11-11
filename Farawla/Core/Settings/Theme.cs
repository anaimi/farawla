using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Farawla.Features;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Farawla.Core
{
	public class Theme
	{
		public const string DEFAULT_EDITOR_MATCHING_TOKENS_BACKGROUND = "#FFFFFF00";
		public const string DEFAULT_EDITOR_MATCHING_BRACKETS_BACKGROUND = "#FFFFEE78";
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
						Notifier.Show("The directory '" + DIRECTORY_NAME + "', which is supposed to include the themes, does not exists. I'll create it, but you'll have to load it with themes (and activate one of the themes).");
						Directory.CreateDirectory(DIRECTORY_NAME);

						_instance = new Theme();
					}
					else if (!File.Exists(DIRECTORY_NAME + "\\" + Settings.Instance.ThemeName))
					{
						Notifier.Show("The theme '" + Settings.Instance.ThemeName + "' does not exist in the folder '" + DIRECTORY_NAME + "'");
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
		
		public string MatchingTokensBackground { get; set; }
		public string MatchingBracketsBackground { get; set; }
		public string Background { get; set; }
		public string Foreground { get; set; }
		public string FontFamily { get; set; }
		
		public Dictionary<string, string> SyntaxColors { get; set; }
		
		private ImageSource objectIcon;
		private ImageSource functionIcon;
		private ImageSource snippetIcon;
		
		public Theme()
		{
			MatchingTokensBackground = DEFAULT_EDITOR_MATCHING_TOKENS_BACKGROUND;
			MatchingBracketsBackground = DEFAULT_EDITOR_MATCHING_BRACKETS_BACKGROUND;
			Background = DEFAULT_EDITOR_BACKGROUND;
			Foreground = DEFAULT_EDITOR_FOREGROUND;
			FontFamily = DEFAULT_EDITOR_FONT_FAMILY;
			
			SyntaxColors = new Dictionary<string, string>();

			objectIcon = LoadIcon("object.png");
			functionIcon = LoadIcon("function.png");
			snippetIcon = LoadIcon("snippet.png");
		}
		
		private ImageSource LoadIcon(string name)
		{
			var path = Settings.ExecDir + "themes/" + name;
			
			if (!File.Exists(path))
				return null;

			var source = new BitmapImage();
			
			source.BeginInit();
			source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
			source.EndInit();

			return source;
		}
		
		public ImageSource GetObjectIcon()
		{
			return objectIcon;
		}

		public ImageSource GetFunctionIcon()
		{
			return functionIcon;
		}

		public ImageSource GetSnippetIcon()
		{
			return snippetIcon;
		}
		
		public System.Windows.Media.Color GetColor(string key)
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
}
