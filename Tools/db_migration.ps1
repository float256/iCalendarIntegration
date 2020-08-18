#====== Script destination
# start pre-update database

param( 
        [Parameter(Mandatory=$True)]
        [String]
        $Environment,
        [Parameter(Mandatory=$True)]
        [ValidateSet('info','update','mark')]
        [String]
        $Command,
        [Parameter(Mandatory=$True)]
        [ValidateSet('CalendarIntegrationWeb')]
        [String]
        $SettingsName,
        [Parameter(Mandatory=$False)]
        [String]
        $File
     )

$currDir = Split-Path $MyInvocation.MyCommand.Path -Parent
. "$currDir\..\deployment\_db_migration.inc.ps1"

function GetUtilParams($env, $settingsName) {
    $connStrName    = $dbSettings[$settingsName].connStrName;
    $dir            = $dbSettings[$settingsName].dir;
    $path           = $dbSettings[$settingsName].localPath;

    $appEnvConfigurationFilePath = "$basePath\$path\appsettings.$env.json";
    $sharedEnvConfigurationFilePath = "$basePath\SharedConfiguration\sharedSettings.$env.json";

    if (Test-Path $appEnvConfigurationFilePath) {
        $connStr =  (Get-Content -Raw -Path $appEnvConfigurationFilePath | ConvertFrom-Json).ConnectionStrings.$connStrName
    }
    elseif (Test-Path $sharedEnvConfigurationFilePath) {
        $connStr =  (Get-Content -Raw -Path $sharedEnvConfigurationFilePath | ConvertFrom-Json).ConnectionStrings.$connStrName
    }
    else {
        throw [System.ArgumentException] "No configuration file found for $settingsName";
    }

    if( [string]::IsNullOrEmpty($connStr) ) {
        throw [System.ArgumentException] "No connection string found for $settingsName";
    }

    return @{
        "connStr" = "$connStr";
        "dir"     = "$basePath\$dir";
    };
}

#start here
switch ($Command) {
    "info" {
        ExecMigrationInfo "$Environment" $SettingsName;
    }
    "update" {
        #pre
        ExecDbUpdate "$Environment" $SettingsName "pre";

        #post
        ExecDbUpdate "$Environment" $SettingsName "post";
    }
    "mark" {
         ExecMark "$Environment" $SettingsName "$File";
   }
}