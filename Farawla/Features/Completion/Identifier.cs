﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Farawla.Features.Completion
{
	public class Identifier
	{
		public string OptionType { get; set; }
		public string Match { get; set; }

		private Regex _regex;
		public Regex Regex
		{
			get
			{
				if (_regex == null)
					_regex = new Regex(Match, RegexOptions.Compiled);

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
}