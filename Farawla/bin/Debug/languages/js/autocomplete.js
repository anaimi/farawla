{
	Identifiers: [
		{
			Type: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(,)"
		},
		{
			Type: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(;)"
		},
		{
			Type: "Function",
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
		
	IgnoreSections: [
		"\\\".*?\\\"", /* double quoted strings */
		"'.*?'", /* single quoted strings */
		"//.+", /* line comments */
		"/\\*(.|\\n|\\r)+\\*/" /* block comments */
	]
}