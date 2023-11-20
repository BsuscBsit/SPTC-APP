@echo off
cd /D %~dp0
echo MySQL is trying to start
echo Please wait ...
echo MySQL is starting with mysql\bin\my.ini (console)

start "MySQL Console" /B mysql\bin\mysqld --defaults-file=mysql\bin\my.ini --standalone

if errorlevel 1 goto error

rem Add a delay to allow MySQL to start (adjust the time as needed) type shell:startup in win run
timeout /t 3

rem Close the Command Prompt window
taskkill /F /FI "WINDOWTITLE eq MySQL Console*"

exit

:error
echo.
echo MySQL konnte nicht gestartet werden
echo MySQL could not be started
pause

rem Add a delay to allow MySQL to start (adjust the time as needed)
timeout /t 3

rem Close the Command Prompt window
taskkill /F /FI "WINDOWTITLE eq MySQL Console*"

exit
