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
		public List<Scope> Scopes { get; set; }
		public List<string> IgnoreSections { get; set;}

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
			var scopes = new List<ScopeRange>();
			var matches = new List<IdentifierMatch>();
			
			#region Ignore sections
			
			foreach(var section in IgnoreSections)
			{
				var match = new Regex(section).Match(sbCode.ToString());
				
				while(match.Success)
				{
					for (var i = 0; i < match.Length; i++)
						sbCode[match.Index + i] = ' ';
					
					match = match.NextMatch();
				}
			}
			
			#endregion
			
			#region Populate scopes

			foreach (var scope in Scopes)
			{
				var scopeStack = new Stack<int>();
				var beginMatch = scope.BeginRegex;
				var scopeMatch = scope.ScopeMatch.Match(sbCode.ToString());
				
				while (scopeMatch.Success)
				{
					if (beginMatch.IsMatch(scopeMatch.Value))
					{
						scopeStack.Push(scopeMatch.Index);
					}
					else
					{
						if (scopeStack.Count > 0)
						{
							scopes.Add(new ScopeRange(scopeStack.Pop(), scopeMatch.Index));
						}
					}

					scopeMatch = scopeMatch.NextMatch();
				}
			}
			
			scopes.Add(new ScopeRange(0, sbCode.Length));
			
			#endregion
			
			#region Populate matches
			
			foreach(var identifier in Identifiers)
			{
				var match = identifier.Regex.Match(sbCode.ToString());

				while(match.Success)
				{
					var offset = match.Index;
					var scope = scopes.Where(s => s.From < offset && s.To >= offset).OrderBy(s => s.Size).First();
					
					if (identifier.Type == "Object")
					{
						var expression = code.Substring(match.Groups["expression"].Index, match.Groups["expression"].Length);
						matches.Add(new IdentifierMatch(IdentifierType.Object, scope, offset, match.Groups["name"].Value, expression));
					}
					else if (identifier.Type == "Function")
					{
						var parameters = code.Substring(match.Groups["parameters"].Index, match.Groups["parameters"].Length);
						matches.Add(new IdentifierMatch(IdentifierType.Function, scope, offset, match.Groups["name"].Value, parameters));
					}
					
					// remove the match, so its not processed again
					for(var i = 0; i < match.Length; i++)
						sbCode[match.Index + i] = ' ';
					
					// allow for next match
					match = match.NextMatch();
				}
			}
			
			#endregion

			return matches.Distinct().ToList();
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
				if (_regex == null)
					_regex = new Regex(Match);

				return _regex;
			}
		}
	}
	
	public class IdentifierMatch
	{
		public string Name { get; set; }
		public string Expression { get; set; }
		public int Offset { get; set; }
		public IdentifierType Type { get; set; }
		public ScopeRange Scope { get; set; }

		public IdentifierMatch(IdentifierType type, ScopeRange scope, int offset, string name, string expression)
		{
			Type = type;
			Name = name;
			Expression = expression;
			Offset = offset;
			Scope = scope;
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return GetHashCode() == obj.GetHashCode();
			
		}
		
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}

	public enum IdentifierType
	{
		Object,
		Function
	}
	
	public class Scope
	{
		public string Begin { get; set; }
		public string End { get; set; }

		private Regex beginRegex;
		public Regex BeginRegex
		{
			get
			{
				if (beginRegex == null)
					beginRegex = new Regex(Begin);

				return beginRegex;
			}
		}

		private Regex endRegex;
		public Regex EndRegex
		{
			get
			{
				if (endRegex == null)
					endRegex = new Regex(End);

				return endRegex;
			}
		}

		private Regex scopeMatch;
		public Regex ScopeMatch
		{
			get
			{
				if (scopeMatch == null)
					scopeMatch = new Regex("(" + Begin + ")|(" + End + ")");

				return scopeMatch;
			}
		}
	}
	
	public class ScopeRange
	{
		public int From { get; set; }
		public int To { get; set; }
		public int Size { get; private set; }

		public ScopeRange(int from, int to)
		{
			From = from;
			To = to;

			Size = To - From;
		}
	}
}
