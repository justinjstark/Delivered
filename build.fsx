// include Fake lib
#r "./tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.NUnit3
open Fake.NuGet.Install

let toolsDir = "./tools"
let workDir = "./.build"
let srcDir = "./src"

let appDir = workDir @@ "app"
let testDir = workDir @@ "tests"

Target "Clean" (fun _ ->
  CleanDir workDir
)

Target "UpdateBuildTools" (fun _ ->
  (toolsDir @@ "nuget/packages.config")
  |> NugetInstall (fun p ->
    {p with
      OutputDirectory = toolsDir
      ExcludeVersion = true }))

Target "RestorePackages" (fun _ ->
  !! (srcDir @@ "**/packages.config")
  |> Seq.iter(RestorePackage (fun p ->
    {p with
      OutputPath = "./src/packages"})))

Target "BuildApp" (fun _ ->
  !! (srcDir @@ "Delivered/Delivered.csproj")
    |> MSBuildRelease appDir "Build"
    |> Log "Build-Output: ")

Target "BuildTests" (fun _ ->
  !! (srcDir @@ "Delivered.Tests/Delivered.Tests.csproj")
    |> MSBuildDebug testDir "Build"
    |> Log "Test-Output: ")

Target "RunTests" (fun _ ->
  !! (testDir @@ "*Tests.dll")
    |> NUnit3 (fun p ->
      {p with
        ResultSpecs = "TestResult.xml;format=AppVeyor" }))

"Clean"
  ==> "RestorePackages"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "BuildApp"

// start build
RunTargetOrDefault "BuildApp"
