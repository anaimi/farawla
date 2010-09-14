using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Farawla.Core.Language
{
	public class AutoComplete
	{
		public List<Identifier> Identifiers { get; set; }

		private LanguageMeta language;
		
		public AutoComplete()
		{
			Identifiers = new List<Identifier>();
		}
		
		public void Initialize(LanguageMeta language)
		{
			this.language = language;
		}
		
		public List<IdentifierMatch> GetIdentifiersFromCode(string code)
		{
			var sbCode = new StringBuilder(code);
			var matches = new List<IdentifierMatch>();
			
			foreach(var identifier in Identifiers)
			{
				var match = identifier.Regex.Match(sbCode.ToString());

				while(match.Success)
				{
					if (identifier.Type == "Object")
					{
						matches.Add(new IdentifierMatch(IdentifierType.Object, match.Groups["name"].Value, match.Groups["expression"].Value));
					}

					sbCode.Remove(match.Index, match.Length);
					
					for(var i = 0; i < match.Length; i++)
						sbCode.Insert(match.Index + i, " ");
					
					match = match.NextMatch();
				}
			}

			return matches;
		}
	}

	public class Identifier
	{
		public string Type { get; set; }
		public string Match { get; set; }

		private Regex _regex;
		public Regex Regex
		{
			get
			{
				if (!Match.IsBlank() && _regex == null)
					_regex = new Regex(Match);

				return _regex;
			}
		}
	}
	
	public class IdentifierMatch
	{
		public string Name { get; set; }
		public string Expression { get; set; }
		public IdentifierType Type { get; set; }

		public IdentifierMatch(IdentifierType type, string name, string expression)
		{
			Type = type;
			Name = name;
			Expression = expression;
		}
	}

	public enum IdentifierType
	{
		Object,
		Function
	}
}
