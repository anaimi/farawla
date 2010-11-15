using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farawla.Features.Completion
{
	public class Framework
	{
		public string Name { get; set; }
		public string Path { get; set; }
	}
	
	public class FrameworkTypes
	{
		public List<Type> Types { get; set; }
	}
}
