﻿// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

#r @"../packages/FAKE/tools/FakeLib.dll"
#r "System.Configuration.dll"

open Fake

open System

let authors = ["Tomasz Jaskula (Compose IT)"]

// project name and description
let projectName = "Akka.NET.FSharp.API.Extensions"
let projectDescription = "Set of extensions to the Akka.NET F# API. Some features are not available in the standard F# API like for example the ability to provide functions to override actors lifecycles, which might be usefull so those are provided here."
let projectSummary = "Set of extensions to the Akka.NET F# API."

// directories
let buildDir = "./ComposeIt.Akka.FSharp.Extensions/bin"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@ "AkkaFSharpExtensions"
let toolPath = "./packages/NuGet.CommandLine.2.8.6/tools/NuGet.exe"

let buildMode = getBuildParamOrDefault "buildMode" "Release"

MSBuildDefaults <- { 
    MSBuildDefaults with 
        ToolsVersion = Some "12.0"
        Verbosity = Some MSBuildVerbosity.Minimal }

Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingRoot; packagingDir]
)

//open Fake.AssemblyInfoFile
//
//Target "AssemblyInfo" (fun _ ->
//    CreateCSharpAssemblyInfo "./SolutionInfo.cs"
//      [ Attribute.Product projectName
//        Attribute.Version releaseNotes.AssemblyVersion
//        Attribute.FileVersion releaseNotes.AssemblyVersion
//        Attribute.ComVisible false ]
//)

let setParams defaults = {
    defaults with
        ToolsVersion = Some("12.0")
        Targets = ["Build"]
        Properties =
            [
                "Configuration", buildMode
            ]
    }

Target "BuildApp" (fun _ ->
    build setParams "./FSharp.Extensions.sln"
        |> DoNothing
)

Target "CreatePackage" (fun _ ->

    let net45Dir = packagingDir @@ "lib/net45/"
//    let netcore45Dir = packagingDir @@ "lib/netcore45/"
//    let portableDir = packagingDir @@ "lib/portable-net45+wp80+win+wpa81/"
    CleanDirs [net45Dir(*; netcore45Dir; portableDir*)]

    CopyFile net45Dir (buildDir @@ "Release/ComposeIt.Akka.FSharp.Extensions.dll")
    CopyFile net45Dir (buildDir @@ "Release/ComposeIt.Akka.FSharp.Extensions.XML")
    CopyFile net45Dir (buildDir @@ "Release/ComposeIt.Akka.FSharp.Extensions.pdb")

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = projectName
            Description = projectDescription                               
            OutputPath = packagingRoot
            Summary = projectSummary
            WorkingDir = packagingDir
            Version = "1.0.0.0"
            Dependencies =
                ["Akka", GetPackageVersion "./packages/" "Akka"
                 "Akka.FSharp", GetPackageVersion "./packages/" "Akka.FSharp"
                 "FsPickler", GetPackageVersion "./packages/" "FsPickler"
                 "FSPowerPack.Core.Community", GetPackageVersion "./packages/" "FSPowerPack.Core.Community"
                 "FSPowerPack.Linq.Community", GetPackageVersion "./packages/" "FSPowerPack.Linq.Community"
                 "Newtonsoft.Json", GetPackageVersion "./packages/" "Newtonsoft.Json"]
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" 
            ToolPath = toolPath}) 
            "Akka.FSharp.Extensions.nuspec"
)

"Clean"
   ==> "BuildApp"

"BuildApp"
   ==> "CreatePackage"

RunTargetOrDefault "CreatePackage"