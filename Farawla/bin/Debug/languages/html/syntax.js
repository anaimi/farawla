{
	IncludeLanguageSyntax: ["xml"],
	
	Spans: [
		{
			Name: "css-syntax",
			Start: "<style\\ type=\"text/css\">",
			End: "</style>",
			Reference: "css"
		},
		{
			Name: "js-syntax",
			Start: "<script\\ type=\"text/javascript\">",
			End: "</script>",
			Reference: "javascript"
		},
		{
			Name: "processing-syntax",
			Start: "<script\\ type=\"application/processing\">",
			End: "</script>",
			Reference: "javascript"
		}
	]
}