using System.Windows.Documents;
using System.Collections.Generic;
using System.IO;
using Farawla.Features.Notifier;
using Newtonsoft.Json;
using System.Linq;
using Farawla.Features;
using Farawla.Features.Settings;

namespace Farawla.Core
{
	public class Settings: IWidget
	{
		#region Widget: Settings
		public string WidgetName { get { return "Settings"; } }
		public bool Expandable { get { return false; } }
		public double WidgetHeight { get { return -1; } }
		#endregion
		
		public static string ExecDir
		{
			get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\"; }
		}

		public const string DEFAULT_EDITOR_BACKGROUND = "#000000";
		public const string DEFAULT_EDITOR_FOREGROUND = "#FFFFFF";
		public const string DEFAULT_EDITOR_FONT_FAMILY = "Courier New";
		
		public const string FILE_NAME = "settings.js";
		
		#region Instance
		private static Settings _instance;
		public static Settings Instance
		{
			get
			{
				if (_instance == null)
				{
					if (!File.Exists(FILE_NAME))
					{
						Notifier.Instance.Show("Settings file does not exists... I'll create one for you");
						_instance = new Settings();
					}
					else
					{
						_instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FILE_NAME));
						
						if (_instance == null)
							_instance = new Settings();
					}
					
					if (_instance.OpenTabs == null)
					{
						_instance.OpenTabs = new List<string>();
					}
				}

				return _instance;
			}
		}
		#endregion

		public string DefaultEditorBackground { get; set; }
		public string DefaultEditorForeground { get; set; }
		public string DefaultEditorFontFamily { get; set; }
		
		public bool IsWindowMaximized { get; set; }
		
		public List<string> OpenTabs { get; set; }
		public List<WidgetSettings> Widgets { get; set; }

		public Settings()
		{
			// set default settings
			DefaultEditorBackground = DEFAULT_EDITOR_BACKGROUND;
			DefaultEditorForeground = DEFAULT_EDITOR_FOREGROUND;
			DefaultEditorFontFamily = DEFAULT_EDITOR_FONT_FAMILY;
			IsWindowMaximized = true;

			OpenTabs = new List<string>();
			Widgets = new List<WidgetSettings>();
		}
		
		public WidgetSettings GetWidgetSettings(string name)
		{
			if (!Widgets.Any(w => w.Name == name))
				Widgets.Add(new WidgetSettings(name));
			
			return Widgets.First(w => w.Name == name);
		}
		
		public void Save()
		{
			File.WriteAllText(ExecDir + FILE_NAME, JsonConvert.SerializeObject(_instance, Formatting.Indented));
		}

		#region Widget Events
		public void OnStart()
		{
		
		}

		public void OnExit()
		{
		
		}

		public void OnResize()
		{
		
		}

		public void OnClick()
		{
			Controller.Current.ShowOverlay();
			SettingsWindow.Instance.ShowDialog();
		}
		#endregion
	}
	
	public class WidgetSettings
	{
		public string Name { get; set; }
		public Dictionary<string, string> Values { get; set; }

		public WidgetSettings()
		{

		}
		
		public WidgetSettings(string name)
		{
			Name = name;
			Values = new Dictionary<string,string>();
		}
		
		public string this[string key]
		{
			get
			{
				if (!Values.ContainsKey(key))
					Values[key] = string.Empty;
				
				return Values[key];
			}
			
			set
			{
				Values[key] = value;
			}
		}
		
		public bool KeyExists(string key)
		{
			return !this[key].IsBlank();
		}
	}
}