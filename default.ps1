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
	rd $build_dir -recurse -force -ErrorAction SilentlyContinue | out-null
}

task compile -depends clean {
	$nuget = "$tools_dir\nuget\nuget.exe"
	$solution = "$source_dir\Delivered.sln"

	Write-Host "Restoring nuget packages"
	exec {&$nuget restore $solution}

	Write-Host "Compiling app"
	exec {msbuild /t:Clean /t:Build /p:Configuration=$config /p:OutputPath="$build_dir\app\bin" /p:IntermediateOutputPath="$build_dir\app\obj\" /v:q /nologo "$source_dir\Delivered\Delivered.csproj"}

	Write-Host "Compiling tests"
		exec {msbuild /t:Clean /t:Build /p:Configuration=$config /p:OutputPath="$build_dir\tests\bin" /p:IntermediateOutputPath="$build_dir\tests\obj\" /v:q /nologo "$source_dir\Delivered.Tests\Delivered.Tests.csproj"}
}

task compileDemo -depends clean {
	$nuget = "$tools_dir\nuget\nuget.exe"
	$solution = "$source_dir\Demo\Demo.sln"

	Write-Host "Restoring nuget packages"
	exec {&$nuget restore $solution}

	Write-Host "Compiling project"
	exec {msbuild /t:Clean /t:Build /p:Configuration=$config /p:OutputPath="$build_dir\demo" /v:q /nologo "$source_dir\Demo\DemoDistributor\DemoDistributor.csproj"}
}

task test -depends compile {
	$testRunner = "$tools_dir\NUnit.ConsoleRunner\tools\nunit3-console.exe"
	$testDll = "$build_dir\tests\bin\Delivered.Tests.dll"

	#Create result directory
	if(!(Test-Path $result_dir))
	{
		ni $result_dir -type directory -force
	}

	exec {&$testRunner $testDll "--result=$result_dir\TestResultNUnit3.xml" "--result=$result_dir\TestResultAppVeyor.xml;format=AppVeyor"}
}
