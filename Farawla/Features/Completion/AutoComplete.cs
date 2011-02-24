using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Farawla.Core;
using System.IO;
using Newtonsoft.Json;
using Farawla.Utilities;

namespace Farawla.Features.Completion
{
	public class AutoComplete
	{
		#region static instances: GetCompletion

		private static WidgetSettings Settings = Core.Settings.Instance.GetWidgetSettings("Completion");
		
		private static Dictionary<string, AutoComplete> instances = new Dictionary<string, AutoComplete>();
		
		public static AutoComplete GetCompletion(string languageName)
		{
			languageName = languageName.ToLower();
			
			if (!instances.ContainsKey(languageName))
			{
				var language = Controller.Current.Languages.GetLanguageByName(languageName);
				var languagePath = language.Directory + "\\";

				if (File.Exists(languagePath + "autocomplete.js"))
				{
					var completion = JsonHelper.Load<AutoComplete>(languagePath + "autocomplete.js");
					
					completion.Initialize(language.Name, languagePath);
					completion.LoadFrameworks(completion.GetEnabledFrameworks());
					
					instances.Add(language.Name.ToLower(), completion);
				}
				else
				{
					instances.Add(language.Name.ToLower(), null);
				}
			}

			return instances[languageName];
		}
		
		#endregion
		
		public List<Identifier> Identifiers { get; set; }
		public List<Scope> Scopes { get; set; }
		public List<Inference> Inference { get; set; }
		public List<Framework> Frameworks { get; set; }
		public List<Type> Types { get; set; }

		public List<string> IgnoreSections { get; set; }		
		public List<string> ObjectAttributeDelimiters { get; set; }
		public List<string> AllowableIdentifierCharacters { get; set; }
		
		public string BaseTypeName { get; set; }
		public string GlobalTypeName { get; set; }
		public string FunctionTypeName { get; set; }

		public string LanguageName { get; private set; }
		public string LanguagePath { get; private set; }

		public AutoComplete()
		{
			Identifiers = new List<Identifier>();
			Scopes = new List<Scope>();
			Inference = new List<Inference>();
			Types = new List<Type>();
			Frameworks = new List<Framework>();

			IgnoreSections = new List<string>();
			ObjectAttributeDelimiters = new List<string>();
			AllowableIdentifierCharacters = new List<string>();
		}

		public void Initialize(string languageName, string languagePath)
		{
			LanguageName = languageName;
			LanguagePath = languagePath;
		}

		public Type GetGlobalType()
		{
			return Types.FirstOrDefault(t => t.Name == GlobalTypeName);
		}

		public Type GetBaseType()
		{
			return Types.FirstOrDefault(t => t.Name == BaseTypeName);
		}
		
		public Type GetFunctionType()
		{
			return Types.FirstOrDefault(t => t.Name == FunctionTypeName);
		}
		
		public List<string> GetEnabledFrameworks()
		{
			return Settings[LanguageName + "Frameworks"].Split(',').Distinct().Where(f => !f.IsBlank()).ToList();
		}

		public void LoadFrameworks(List<string> frameworksNames)
		{
			Types.Clear();
			
			foreach(var framework in Frameworks.Where(f => frameworksNames.Contains(f.Name)))
			{
				if (!File.Exists(LanguagePath + framework.Path))
				{
					continue;
				}

				var obj = JsonHelper.Load<FrameworkTypes>(LanguagePath + framework.Path);
				
				foreach(var type in obj.Types)
				{
					var currentType = Types.FirstOrDefault(t => t.Name == type.Name);
					
					if(currentType != null)
					{
						currentType.Options.AddRange(type.Options);
					}
					else
					{
						Types.Add(type);
					}
				}
			}
		}
	}
}