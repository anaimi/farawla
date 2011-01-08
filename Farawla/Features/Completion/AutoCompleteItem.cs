using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farawla.Core.TabContext;

namespace Farawla.Features.Completion
{
	public class AutoCompleteItem : CompletionWindowItem
	{
		public const string COMPLETION_OWNER_NAME = "Completion";

		public AutoCompleteItem(CompletionItemType type, string text, string description) : base(type, COMPLETION_OWNER_NAME, text, description)
		{

		}
	}
}
