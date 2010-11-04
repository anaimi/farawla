using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farawla.Core;

namespace Farawla.Features.Completion
{
	public class AutoCompleteItem : CompletionWindowItem
	{
		public const string COMPLETION_OWNER_NAME = "Completion";

		public AutoCompleteItem(string text)
			: base(COMPLETION_OWNER_NAME, text)
		{

		}
	}
}
