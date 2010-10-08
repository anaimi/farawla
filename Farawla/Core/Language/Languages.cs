using System.Collections.Generic;
using System.Linq;
using System.IO;
using Farawla.Features;
using Newtonsoft.Json;

namespace Farawla.Core.Language
{
	public class Languages
	{
		public List<LanguageMeta> List { get; private set; }
		
		public Languages()
		{
			List = new List<LanguageMeta>();
			
			if (!VerifyLanguagesFolderExist())
				return;
			
			foreach(var lang in Directory.GetDirectories("languages"))
			{
				var json = File.ReadAllText(lang + "\\main.js");
				var obj = JsonConvert.DeserializeObject<LanguageMeta>(json);
				
				obj.Initialize(lang);
				List.Add(obj);
			}
		}
		
		public LanguageMeta GetLanguage(string extension)
		{
			foreach(var lang in List)
				if (lang.Associations.Any(a => a == extension))
					return lang;
			
			var defaultLanguage = new LanguageMeta();
			defaultLanguage.Initialize(string.Empty);

			return defaultLanguage;
		}
		
		private bool VerifyLanguagesFolderExist()
		{
			if (!Directory.Exists("languages"))
			{
				Notifier.Show("The folder 'languages' was not found in the same directory of the executable. You should create it and load it with a folder for each language you want Farawla to support");
				return false;
			}

			if (Directory.GetDirectories("languages").Length == 0)
			{
				Notifier.Show("The folder 'languages' is empty. You should load it with folders of each language you want Farawla to support");
				return false;
			}
			
			return true;
		}
	}
}