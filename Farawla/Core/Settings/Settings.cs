using System.Windows.Documents;
using System.Collections.Generic;
using System.IO;
using Farawla.Core.Sidebar;
using Newtonsoft.Json;
using System.Linq;
using Farawla.Features;
using Farawla.Features.Settings;
using System;

namespace Farawla.Core
{
	public class Settings: IWidget
	{
		public BarButton SidebarButton { get; set; }
		public static string ExecDir
		{
			get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\"; }
		}

		public const string DEFAULT_THEME = "clouds.js";
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
						Notifier.Show("Settings file does not exists... I'll create one for you");
						_instance = new Settings();
					}
					else
					{
						_instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FILE_NAME));

						// create sidebar button
						_instance.SidebarButton = new BarButton(_instance, "Settings");
						_instance.SidebarButton.IsExpandable = false;
						_instance.SidebarButton.OnClick += () => SettingsWindow.Instance.ShowDialog();
						
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
		
		public string ThemeName { get; set; }
		public bool IsWindowMaximized { get; set; }

		public List<string> OpenTabs { get; set; }
		public List<ClosedTabs> ClosedTabs { get; set; }
		public List<WidgetSettings> Widgets { get; set; }

		public Settings()
		{
			// set default settings
			ThemeName = DEFAULT_THEME;
			IsWindowMaximized = true;

			OpenTabs = new List<string>();
			ClosedTabs = new List<ClosedTabs>();
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
			SidebarButton = null;
			
			File.WriteAllText(ExecDir + FILE_NAME, JsonConvert.SerializeObject(_instance, Formatting.Indented));
		}
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
	
	public class ClosedTabs
	{
		public string Path { get; set; }
		public int Index { get; set; }

		public ClosedTabs(string path, int index)
		{
			Path = path;
			Index = index;
		}
	}
}