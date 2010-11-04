using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Farawla.Core;

namespace Farawla.Features.Completion
{
	public class AutoComplete
	{
		public List<Identifier> Identifiers { get; set; }
		public List<Scope> Scopes { get; set; }
		public List<Type> Types { get; set; }
		public List<string> ObjectAttributeDelimiters { get; set; }
		
		public string GlobalTypeName { get; set; }
		public string DefaultTypeName { get; set; }
		
		public List<Regex> IgnoreExpressions { get; set; }
		public List<string> IgnoreSections { get; set; }

		public AutoComplete()
		{
			Identifiers = new List<Identifier>();
			Scopes = new List<Scope>();
			Types = new List<Type>();
			
			ObjectAttributeDelimiters = new List<string>();
			// DONOT initialize IgnoreExpressions
		}

		public Type GetGlobalType()
		{
			return Types.FirstOrDefault(t => t.Name == GlobalTypeName);
		}

		public Type GetDefaultType()
		{
			return Types.FirstOrDefault(t => t.Name == DefaultTypeName);
		}
	}
}