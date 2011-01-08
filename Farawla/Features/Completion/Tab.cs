using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farawla.Features.Completion;

namespace Farawla.Core.TabContext
{
	public partial class Tab
	{
		public bool WasAssignedAutoCompletionState { get; set; }
		public AutoCompleteState AutoCompleteState { get; set; }
	}
}