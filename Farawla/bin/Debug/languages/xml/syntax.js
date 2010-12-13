{
	Spans: [
		{
			Name: "comment",
			Start: "<!--",
			End: "-->"
		},
		{
			Name: "xml-tag-span",
			Start: "<",
			End: ">",
			Syntax: {
				Spans: [
					{
						Name: "string single-quote",
						Start: "'",
						End: "'"
					},
					{
						Name: "string double-quote",
						Start: "\"",
						End: "\""
					}
				],
				Rules: [
					{
						Name: "xml-attribute",
						Regex: "(\\w|-|:)+(=)"
					},
					{
						Name: "xml-tag",
						Regex: "^(\\w+|!DOCTYPE) | (\\/)(\\w+)$"
					}
				]
			}
		}
	]
}