#====== Script destination
# creating deployment package with msbuild package task and copying
# db-migration scripts to package directory $packageDstDir

param(  
        [Parameter(Mandatory=$True)]
        [String]
        $packageDstDir,
                
        [Parameter(Mandatory=$False)]
        [String]
        $WithTests
     )

$scriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
$basePath = "$scriptDir\.."
$sfProjectFolder = "$basePath\CalendarIntegration"
$sfProjectFile = "$sfProjectFolder\CalendarIntegration.sfproj"
$visualStudioVersion = "15.0";

$nugetSource = "https://www.nuget.org/api/v2/"

function CreatePackage {
    $build = "$basePath/.nuget/nuget restore $basePath  -source $nugetSource"
    cmd.exe /c $build

    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }

    $build = "dotnet restore $basePath --packages $basePath\packages\ --source $nugetSource"
    cmd.exe /c $build

    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }

    $build = "dotnet msbuild $sfProjectFile /nr:false /p:VisualStudioVersion=$visualStudioVersion /t:Package /p:Configuration=Release"
    $build += " || exit 1"
    cmd.exe /c $build

    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }
}

function CopyFiles {
    Copy-Item $sfProjectFolder/pkg/Release -Destination $packageDstDir/sf-package -Recurse
    Copy-Item $sfProjectFolder/ApplicationParameters -Destination $packageDstDir/sf-parameters -Recurse
    Copy-Item $basePath/deployment -Destination $packageDstDir/deployment -Recurse
    Copy-Item $basePath/Tools -Destination $packageDstDir/Tools -Recurse
    Copy-Item $basePath/db-migrations -Destination $packageDstDir/db-migrations -Recurse
}

function RunTestProject($baseTests, $testProject) {
    Write-Host
    Write-Host "Run tests: $testProject"
    $command = "dotnet test $baseTests\$testProject\$testProject.csproj --filter Category!=Fragile"
    $command += " || exit 1"
    cmd.exe /c $command;
    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }
}

function RunTests {
    if ( $WithTests -eq "true" ) {
        RunTestProject "$basePath" "iCalendarIntegration.Tests"
    }
}

#start here
CreatePackage
RunTests
CopyFiles