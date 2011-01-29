[
	{
		Name: "DOCTPYE",
		Trigger: "doc",
		Body: '<!DOCTYPE html>'
	},
	{
		Name: "Link",
		Trigger: "links",
		Body: '<link rel="stylesheet" type="text/css" href="example.css" media="all" />'
	},
	{
		Name: "Style",
		Trigger: "style",
		Body: '<style type="text/css">\n\t$0\n</style>'
	},
	{
		Name: "Script",
		Trigger: "script",
		Body: '<script type="text/javascript">\n\t$0\n</script>'
	},
	{
		Name: "Google jQuery",
		Trigger: "jquery",
		Body: '<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>'
	},
	{
		Name: "Favicon",
		Trigger: "favicon",
		Body: '<link rel="shortcut icon" href="favicon.ico">'
	},
	{
		Name: "RSS",
		Trigger: "rss",
		Body: '<link rel="alternate" type="application/rss+xml" title="RSS Title Goes Here" href="http://www.example.com/rss" />'
	},
	{
		Name: "Description",
		Trigger: "description",
		Body: '<meta name="description" content="$0" />'
	},
	{
		Name: "Keywords",
		Trigger: "keywords",
		Body: '<meta name="keywords" content="$0" />'
	},
	{
		Name: "hCard",
		Trigger: "hcard",
		Body: '<div class="vcard">\n	<p class="fn n"><span class="given-name">FirstName</span> <span class="family-name">FamilyName</span></p>\n	<img src="{path-to-avatar}" class="photo" alt="{name}" width="50" height="50" />\n	<ul>\n		<li><a class="url" href="http://www.example.com/" title="example">example.com</a></li>\n		<li><a class="email" href="mailto:your@mail.address" title="Mail to FirstName LastName">your@mail.address</a></li>\n		<li class="adr"><span class="locality">City</span>, <span class="country-name">Country</span></li>\n	</ul>\n</div>'
	},
	{
		Name: "Template",
		Trigger: "template",
		Body: '<!DOCTYPE html>\n<html lang="en">\n	<head>\n		<title>Page Title$0</title>\n		\n		<meta charset="utf-8" />\n		<meta name="description" content="" />\n		<meta name="keywords" content="" />\n		\n		<link rel="shortcut icon" href="favicon.ico" />\n		<link rel="stylesheet" href="example.css" media="all" />\n		<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>\n	</head>\n	\n	<body>\n		\n	</body>\n</html>'
	},
	{
		Name: "IE Comment",
		Trigger: "iecomment",
		Body: '<!--[if IE]>\n	$0\n<![endif]-->'
	}
	
]