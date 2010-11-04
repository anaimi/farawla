{
	Identifiers: [
		{
			OptionType: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(,)"
		},
		{
			OptionType: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(;)"
		},
		{
			OptionType: "Function",
			Match: "function(\\s)(?<name>\\w+)(\\s?)\\((?<parameters>.*)\\)(\\s?){"
		}
	],
		
	Scopes: [
		{
			Begin: "\\{",
			End: "\\}"
		},
		{
			Begin: "\\(",
			End: "\\)"
		}
	],
	
	Types: [
		{
			Name: "object",
			Options: [
				{ Name: "toString()", Description: "Returns a string representation of the current object", OptionType: "Function", ReturnType: "String" },
				{ Name: "test2", Description: "test 2" }
			]
		},
		{
			Name: "this",
			Options: [
				{ Name: "test1", Description: "test 1", OptionType: "Object", ReturnType: "String" },
				{ Name: "test2", Description: "test 2" }
			]
		},
		{
			Name: "String",
			Options: [
				{ Name: "bar1", Description: "bar 1" },
				{ Name: "bar2", Description: "bar 2" }
			]
		}
	],
		
	IgnoreSections: [
		"\\\".*?\\\"", /* double quoted strings */
		"'.*?'", /* single quoted strings */
		"//.+", /* line comments */
		"/\\*(.|\\n|\\r)+\\*/" /* block comments */
	],
	
	GlobalTypeName: "this",
	DefaultTypeName: "object",
	ObjectAttributeDelimiters: ["."]
}