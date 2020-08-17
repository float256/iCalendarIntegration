@echo off
SET ENV=%1

IF '%ENV%'=='' (GOTO usage)

ECHO --------------------------------
ECHO   Updating CalendarIntegrationWeb DB
ECHO --------------------------------
call db_migration %ENV% update CalendarIntegrationWeb

pause

GOTO end

:usage
    ECHO --------------------------------
    ECHO   TravelLine DB migration tool
    ECHO --------------------------------
    ECHO Usage: %SELF_NAME% ^<env^>
    EXIT /B 1
GOTO end

:end
