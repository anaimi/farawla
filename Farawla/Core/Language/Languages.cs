using System.Collections.Generic;
using System.Linq;
using System.IO;
using Farawla.Features;
using Newtonsoft.Json;

namespace Farawla.Core.Language
{
	public class Languages
	{
		public List<LanguageMeta> Items { get; private set; }
		
		public Languages()
		{
			Items = new List<LanguageMeta>();
		}
		
		public LanguageMeta GetLanguageByExtension(string extension)
		{
			foreach(var lang in Items)
				if (lang.Associations.Any(a => a == extension))
					return lang;
			
			return GetDefaultLanguage();
		}
		
		public LanguageMeta GetLanguageByName(string name)
		{
			var lang = Items.FirstOrDefault(l => l.Name.ToUpper() == name.ToUpper());
			
			if (lang == null)
				return GetDefaultLanguage();

			return lang;
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
		
		public LanguageMeta GetDefaultLanguage()
		{
			var language = new LanguageMeta();
			
			language.LoadChildren();
			language.InitializeChildren();

			return language;
		}

		public void LoadLanguages()
		{
			if (!VerifyLanguagesFolderExist())
				return;

			// load POCO language meta
			foreach (var lang in Directory.GetDirectories("languages"))
			{
				var path = lang + "\\main.js";

				if (!File.Exists(path))
				{
					Notifier.Show(string.Format("Definition for language '{0}' was skipped because 'main.js' is missing. Either delete the directory or create a main.js with correct values.", lang));
					continue;
				}

				var json = File.ReadAllText(path);
				var obj = JsonConvert.DeserializeObject<LanguageMeta>(json);

				obj.Directory = lang;
				obj.LoadChildren();

				Items.Add(obj);
			}

			// initialize languages' children
			foreach (var lang in Items)
			{
				lang.InitializeChildren();
			}
		}
	}
}