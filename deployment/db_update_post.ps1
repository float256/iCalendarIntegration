#====== Script destination
# start post-update database

param( 
        [Parameter(Mandatory=$True)]
        [String]
        $Environment
     )

$updateScriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
. "$updateScriptDir\_db_migration.inc.ps1"

#start here
ExecDbUpdate $Environment "CalendarIntegrationWeb" "pre";
