using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Farawla.Core;
using System.IO;
using Newtonsoft.Json;

namespace Farawla.Features.Completion
{
	public class AutoComplete
	{
		public List<Identifier> Identifiers { get; set; }
		public List<Scope> Scopes { get; set; }
		public List<Inference> Inference { get; set; }
		public List<Framework> Frameworks { get; set; }
		public List<Type> Types { get; set; }
		
		public List<string> ObjectAttributeDelimiters { get; set; }
		
		public string BaseTypeName { get; set; }
		public string GlobalTypeName { get; set; }
		public string FunctionTypeName { get; set; }
				
		public List<Regex> IgnoreExpressions { get; set; }
		public List<string> IgnoreSections { get; set; }

		private string languagePath;

		public AutoComplete()
		{
			Identifiers = new List<Identifier>();
			Scopes = new List<Scope>();
			Inference = new List<Inference>();
			Types = new List<Type>();
			Frameworks = new List<Framework>();
			
			ObjectAttributeDelimiters = new List<string>();
			
			// DONOT initialize IgnoreExpressions
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
		
		public void Initialize(string languagePath)
		{
			this.languagePath = languagePath;
		}

		public void LoadFrameworks(List<string> frameworksNames)
		{
			Types.Clear();
			
			foreach(var framework in Frameworks.Where(f => frameworksNames.Contains(f.Name)))
			{
				if (!File.Exists(languagePath + framework.Path))
				{
					continue;
				}

				var json = File.ReadAllText(languagePath + framework.Path);
				var obj = JsonConvert.DeserializeObject<FrameworkTypes>(json);
				
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