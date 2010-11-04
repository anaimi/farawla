using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Farawla.Features.Completion;
using Newtonsoft.Json;

namespace Farawla.Core.Language
{
	public class LanguageMeta
	{
		public const string DEFAULT_NAME = "Default";
		
		public string Name { get; set; }
		public string Directory { get; set; }
		public List<string> Associations { get; set; }
		
		public bool IsDefault
		{
			get
			{
				return Name == DEFAULT_NAME;
			}
		}
		
		public bool HasSyntax { get; private set; }
		public Syntax Syntax { get; private set; }

		public LanguageMeta()
		{
			Name = DEFAULT_NAME;
			Associations = new List<string>();
		}
		
		public void Initialize(string directory)
		{
			Directory = directory;
			
			#region Initialize Syntax
			
			if (File.Exists(directory + "\\syntax.js"))
			{
				HasSyntax = true;
				
				var json = File.ReadAllText(directory + "\\syntax.js");
				Syntax = JsonConvert.DeserializeObject<Syntax>(json);
			}
			else
			{
				Syntax = new Syntax();
			}

			Syntax.Initialize(this);
			
			#endregion
		}
		
	}
}
