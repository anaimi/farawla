{
	Identifiers: [
		{
			Type: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(,)"
		},
		{
			Type: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(;)"
		}
	]
}