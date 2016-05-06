Framework "4.6.1"

properties {
	$base_dir = resolve-path .
	$source_dir = "$base_dir\src"
	$tools_dir = "$base_dir\tools"
	$build_dir = "$base_dir\.build"
	$result_dir = "$build_dir\result"
	$global:config = "debug"
}

task default -depends clean, compile, compileDemo, test
task ci -depends release, default

task release {
	$global:config = "release"
}

task clean {
	#Delete bin and obj folders
	$binFolders = @(gci $source_dir -recurse -Include bin, obj) | % { $_.FullName }
	foreach ($binFolder in $binFolders)
	{
		Write-Host "Deleting directory $binFolder"
		rd $binFolder -recurse -force -ErrorAction SilentlyContinue | out-null
	}

	#Delete TestResult.xml
	$testResult = "$base_dir\TestResult.xml"
	Write-Host "Deleting $testResult"
	ri $testResult -force -ErrorAction SilentlyContinue | out-null
}

task compile -depends clean {
	$nuget = "$tools_dir\nuget\nuget.exe"
	$solution = "$source_dir\Delivered.sln"

	Write-Host "Restoring nuget packages"
	exec {&$nuget restore $solution}

	Write-Host "Compiling solution"
	exec {msbuild /t:Clean /t:Build /p:Configuration=$config /v:q /nologo $solution}
}

task compileDemo -depends clean {
	$nuget = "$tools_dir\nuget\nuget.exe"
	$solution = "$source_dir\Demo\Demo.sln"

	Write-Host "Restoring nuget packages"
	exec {&$nuget restore $solution}

	Write-Host "Compiling solution"
	exec {msbuild /t:Clean /t:Build /p:Configuration=$config /v:q /nologo $solution}
}

task test -depends compile {
	$testRunner = "$tools_dir\NUnit.ConsoleRunner\tools\nunit3-console.exe"
	$testDll = "$source_dir\Delivered.Tests\bin\$config\Delivered.Tests.dll"

	#Create result directory
	if(!(Test-Path $result_dir))
	{
		ni $result_dir -type directory -force
	}

	exec {&$testRunner $testDll "--result=$result_dir\TestResultNUnit3.xml" "--result=$result_dir\TestResultAppVeyor.xml;format=AppVeyor"}
}
