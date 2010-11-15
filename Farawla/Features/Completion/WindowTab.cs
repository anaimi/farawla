using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farawla.Features.Completion;

namespace Farawla.Core
{
	public partial class WindowTab
	{
		public bool WasAssignedAutoCompletionState { get; set; }
		public AutoCompleteState AutoCompleteState { get; set; }
	}
}
