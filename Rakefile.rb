$VERBOSE = nil

require 'albacore'
require 'FileUtils'

SOLUTION_PATH = "Farawla.sln"
PUBLISH_PATH = "C:/Users/Ahmad/Desktop/My Dropbox/Farawla/app/"
LOCAL_DEPLOY_PATH = "C:/Users/Ahmad/Farawla/"

task :publish => [:build, :zip] do
	puts "Done."
end

task :deployl do
	puts "Deploy locally..."
	FileUtils.cp Dir.glob("#{PUBLISH_PATH}*.exe"), LOCAL_DEPLOY_PATH
	FileUtils.cp Dir.glob("#{PUBLISH_PATH}*.dll"), LOCAL_DEPLOY_PATH
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