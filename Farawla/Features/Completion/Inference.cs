using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Farawla.Features.Completion
{
	public class Inference
	{
		public string Expression { get; set; }
		public string Type { get; set; }

		private Regex _expressionRegex;
		public Regex ExpressionRegex
		{
			get
			{
				if (_expressionRegex == null)
					_expressionRegex = new Regex(Expression, RegexOptions.Compiled | RegexOptions.CultureInvariant);

				return _expressionRegex;
			}
		}

		private Type _completionType;
		public Type GetCompletionType(AutoComplete completion)
		{
			if (_completionType == null)
			{
				_completionType = completion.Types.FirstOrDefault(t => t.Name == Type);
				
				if (_completionType == null)
					_completionType = completion.GetBaseType();
			}

			return _completionType;
		}
	}
}
