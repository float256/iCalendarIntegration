$scriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
$basePath = "$scriptDir\.."
$migrationTool = "$basePath\Tools\DBMigration\DBMigration.exe"

$dbSettings = @{
    "CalendarIntegrationWeb" = @{
        "connStrName"   = "default";
        "dir"           = "db-migrations";
        "path"          = "sf-package\CalendarIntegrationWebPkg\Code.zip";
        "localPath"     = "CalendarIntegrationWeb";
    };
};

function GetUtilParams($env, $settingsName) {
    $connStrName    = $dbSettings[$settingsName].connStrName;
    $dir            = $dbSettings[$settingsName].dir;
    $path           = $dbSettings[$settingsName].path;

    Add-Type -assembly "system.io.compression.filesystem"
    $zip = [io.compression.zipfile]::OpenRead($basePath + "\" + $path)

    $file = $zip.Entries | where-object { $_.Name -eq "appsettings." + $env + ".json"}
    if ( -not $file ) {
        $file = $zip.Entries | where-object { $_.Name -eq "sharedSettings." + $env + ".json"}
    }
    if ( -not $file ) {
        throw [System.ArgumentException] "No configuration file found for $settingsName";
    }

    $stream = $file.Open()

    $reader = New-Object IO.StreamReader($stream)

    $connStr =  ($reader.ReadToEnd() | ConvertFrom-Json).ConnectionStrings.$connStrName

    $reader.Close()
    $stream.Close()
    $zip.Dispose()

    if( [string]::IsNullOrEmpty($connStr) ) {
        throw [System.ArgumentException] "No connection string found for $settingsName";
    }
    
    return @{
        "connStr" = "$connStr";
        "dir"     = "$basePath\$dir";
    };
}

function ExecDbUpdate($env, $settingsName, $prePost) {
    $utilParams = GetUtilParams $env $settingsName;

    $connStr = $utilParams["connStr"];
    $dir     = $utilParams["dir"];

    $updateCommand = "$migrationTool update --$prePost --conn=""$connStr"" --dir=$dir || exit 1";

    cmd.exe /c $updateCommand
    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }
}

function ExecMigrationInfo($env, $settingsName) {
    $utilParams = GetUtilParams $env $settingsName;

    $connStr = $utilParams["connStr"];
    $dir     = $utilParams["dir"];

    $updateCommand = "$migrationTool info --conn=""$connStr"" --dir=$dir || exit 1";

    cmd.exe /c $updateCommand
    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }
}

function ExecMark($env, $settingsName, $file) {
    $utilParams = GetUtilParams $env $settingsName;

    $connStr = $utilParams["connStr"];
    $dir     = $utilParams["dir"];

    $updateCommand = "$migrationTool mark --conn=""$connStr"" --dir=$dir $file || exit 1";

    cmd.exe /c $updateCommand
    if ($LASTEXITCODE -ne "0") {
        throw $LASTEXITCODE
    }
}