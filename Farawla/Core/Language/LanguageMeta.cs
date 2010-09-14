using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Farawla.Core.Language
{
	public class LanguageMeta
	{
		public string Name { get; set; }
		public string Directory { get; set; }
		public List<string> Associations { get; set; }
		
		public bool HasHighlighting { get; private set; }
		public Highlighting Highlighting { get; private set; }
		
		public bool HasAutoComplete { get; private set; }
		public AutoComplete AutoComplete { get; private set; }

		public LanguageMeta()
		{
			Name = "Default";
			Associations = new List<string>();
		}
		
		public void Initialize(string directory)
		{
			Directory = directory;
			
			#region Initialize Highlighting
			
			if (File.Exists(directory + "\\highlighting.js"))
			{
				HasHighlighting = true;
				
				var json = File.ReadAllText(directory + "\\highlighting.js");
				Highlighting = JsonConvert.DeserializeObject<Highlighting>(json);
			}
			else
			{
				Highlighting = new Highlighting();
			}

			Highlighting.Initialize(this);
			
			#endregion

			#region Initialize AutoComplete

			if (File.Exists(directory + "\\autocomplete.js"))
			{
				HasAutoComplete = true;
				
				var json = File.ReadAllText(directory + "\\autocomplete.js");
				AutoComplete = JsonConvert.DeserializeObject<AutoComplete>(json);
			}
			else
			{
				AutoComplete = new AutoComplete();
			}

			AutoComplete.Initialize(this);

			#endregion
		}
		
	}
}
