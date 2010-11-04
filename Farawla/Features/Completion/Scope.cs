using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Farawla.Features.Completion
{
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
					beginRegex = new Regex(Begin, RegexOptions.Compiled);

				return beginRegex;
			}
		}

		private Regex endRegex;
		public Regex EndRegex
		{
			get
			{
				if (endRegex == null)
					endRegex = new Regex(End, RegexOptions.Compiled);

				return endRegex;
			}
		}

		private Regex scopeMatch;
		public Regex ScopeMatch
		{
			get
			{
				if (scopeMatch == null)
					scopeMatch = new Regex("(" + Begin + ")|(" + End + ")", RegexOptions.Compiled);

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
		
		public bool IsCaretInsideScope(int position)
		{
			if (From < position && To >= position)
				return true;

			return false;
		}
	}
}
