using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farawla.Core.TabContext;

namespace Farawla.Features.Completion
{
	public class Type
	{
		public string Name { get; set; }
		public List<TypeOption> Options { get; set; }

		public Type()
		{
			Options = new List<TypeOption>();
		}
	}
	
	public class TypeOption
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ReturnType { get; set; }
		public string OptionType { get; set; }
		
		public CompletionItemType GetCompletionType()
		{
			if (OptionType.ToLower() == "function")
				return CompletionItemType.Function;

			return CompletionItemType.Object;
		}
	}
}
