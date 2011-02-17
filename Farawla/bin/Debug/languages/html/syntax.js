{
	IncludeLanguageSyntax: ["xml"],
	
	Spans: [
		{
			Start: "<style\\ type=\"text/css\">",
			End: "</style>",
			Reference: "CSS"
		},
		{
			Start: "<script\\ type=\"text/javascript\">",
			End: "</script>",
			Reference: "JavaScript"
		},
		{
			Start: "<script\\ type=\"application/processing\">",
			End: "</script>",
			Reference: "Processing"
		}
	]
}