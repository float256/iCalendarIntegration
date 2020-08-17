@ECHO OFF
SET DIR=%~dp0

SET SELF_NAME=%~n0
SET TOOL=%DIR%\DBMigration\DBMigration.exe

SET ENV=%1
SET COMMAND=%2
SET SETTINGS_NAME=%3
SET FILE_NAME=%4

IF '%ENV%'=='help' (GOTO usage)
IF '%COMMAND%'=='help' (GOTO usage)

IF '%ENV%'=='' (GOTO usage)
IF '%COMMAND%'=='' (GOTO usage)

powershell.exe -ExecutionPolicy Bypass -File db_migration.ps1 -Environment %ENV% -Command %COMMAND% -SettingsName %SETTINGS_NAME% -File "%FILE_NAME%"

GOTO end

:usage
    ECHO --------------------------------
    ECHO   TravelLine DB migration tool
    ECHO --------------------------------
    ECHO Usage: %SELF_NAME% ^<env^> ^<command^> ...
    ECHO Examples: 
    ECHO   %SELF_NAME% help [command]
    ECHO   %SELF_NAME% ^<env^> update ^<SettingsName>
    ECHO   %SELF_NAME% ^<env^> info ^<SettingsName>
    ECHO   %SELF_NAME% ^<env^> mark ^<SettingsName> ^<migration^>
    EXIT /B 1
GOTO end

:end