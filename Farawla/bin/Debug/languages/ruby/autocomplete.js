{
	Identifiers: [
		{
			OptionType: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(,|;|\\n)?"
		},
		{
			OptionType: "Function",
			Match: "def(\\s)(?<name>\\w+)(\\s?)\\((?<parameters>.*)\\)(\\s?){"
		}
	],
	
	Inference: [
		{
			Expression: "\\\"(.*)\\\"",
			Type: "String"
		},
		{
			Expression: "%\\{(.*)\\}",
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
		},
		{
			Expression: "\\{(.*)\\}",
			Type: "Hash"
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
		},
		{
			Begin: "begin",
			End: "end"
		},
		{
			Begin: "do",
			End: "end"
		}
	],
	
	IgnoreSections: [
		"comment", "string"
	],
	
	Frameworks: [
		{ Name: "Core", Path: "framework-core.js" }
	],
	
	GlobalTypeName: "_Farawla_Global",
	FunctionTypeName: "_Farawla_Function",
	BaseTypeName: "Object",
	ObjectAttributeDelimiters: ["."]
}