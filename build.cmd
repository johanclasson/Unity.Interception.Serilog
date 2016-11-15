@ECHO OFF
if not exist build.ps1 ( powershell -NoProfile -Command "Invoke-WebRequest http://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1 -UseBasicParsing" )
powershell -NoProfile -NoLogo -Command "./build.ps1 "