@echo off
echo Cleaning solution...
for /r %%i in (bin,obj) do if exist "%%i" rd /s /q "%%i"
echo Done. You can reopen Visual Studio and rebuild.
pause
