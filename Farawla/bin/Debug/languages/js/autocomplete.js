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
	
	Inference: [
		{
			Expression: "\\\"(.*)\\\"",
			Type: "String"
		},
		{
			Expression: "(\\d|\\.)+",
			Type: "Number"
		},
		{
			Expression: "(true|false)+",
			Type: "Boolean"
		},
		{
			Expression: "\\[(.*)\\]",
			Type: "Array"
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
	
	IgnoreSections: [
		"\\\".*?\\\"", /* double quoted strings */
		"'.*?'", /* single quoted strings */
		"//.+", /* line comments */
		"/\\*.*?\\*/" /* block comments */
	],
	
	Frameworks: [
		{ Name: "Core", Path: "framework-core.js" },
		{ Name: "Browser Objects", Path: "framework-browser.js" }
	],
	
	GlobalTypeName: "_Farawla_Global",
	FunctionTypeName: "_Farawla_Function",
	BaseTypeName: "Object",
	ObjectAttributeDelimiters: ["."]
}