/*
	TODO: optimize.
*/

{
	Rules: [
		{
			Name: "css-block",
			Regex: ".+"
		}
	],
	Spans: [
		{
			Name: "comment multi-line",
			Start: "/\\*",
			End: "\\*/"
		},
		{
			Name: "css-block",
			Start: "\\{",
			End: "\\}",
			Syntax: {
				Spans: [
					{
						Name: "css-value",
						Start: "\\:",
						End: ";",
					}
				],
				Rules: [
					{
						Name: "css-property",
						Regex: ".+"
					}
				]
			}
		}
	]
}