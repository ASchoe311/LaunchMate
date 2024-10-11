if "%1"=="Debug" start /d "C:\Users\adamr\Desktop\Playnite Portable" Playnite.DesktopApp.exe
@REM Powershell.exe -ExecutionPolicy Bypass -File "C:\Users\adamr\Documents\Coding\Playnite\ATLInt\ATLauncherInstanceImporter\taillog.ps1"
if "%1"=="Release" (
    start /D "C:\Users\adamr\AppData\Local\Playnite" Toolbox.exe pack %2 %3
)