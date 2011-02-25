$VERBOSE = nil

require 'albacore'
require 'FileUtils'

SOLUTION_PATH = "Farawla.sln"
PUBLISH_PATH = "C:/Users/Ahmad/Desktop/My Dropbox/Farawla/app/"

task :publish => [:build, :zip] do
	puts "Done."
end

msbuild :clean do |msb|
	puts "Cleaning..."
	msb.solution = "#{SOLUTION_PATH}"
	msb.targets :clean
	msb.verbosity = "Quiet"
	msb.properties :configuration => :RELEASE
end

msbuild :build do |msb|
	puts "Building..."
	msb.properties(
		:Configuration => "Release",
		:OutDir => "#{PUBLISH_PATH}",
		:DebugType => "None",
		:DebugSymbols => "false",
		:Optimize => "yes",
		:GenerateDocumentation => "No"
	)
	msb.targets :clean, :build
	msb.verbosity = "Quiet"
	msb.solution = "#{SOLUTION_PATH}"
end

zip :zip do |zip|
	puts "Zipping..."
	FileUtils.rm Dir.glob("#{PUBLISH_PATH}*.xml")
	
	zip.directories_to_zip "#{PUBLISH_PATH}"
	zip.output_file = 'Farawla.zip'
	zip.output_path = "#{PUBLISH_PATH}../"
end