@echo off

SET MSBUILD_PATH=C:\Program Files (x86)\MSBuild\14.0\Bin

SET WORKSPACE_PATH=%~1

IF [%1] == [] SET WORKSPACE_PATH=..

CD "%WORKSPACE_PATH%"

dotnet restore

"%MSBUILD_PATH%\MSBuild.exe" Sofco.sln

cmd /c dotnet test src/Sofco.UnitTest

CD src\Sofco.WebJob

cmd /c dotnet publish -c azsof01wd --output \\azsof01wd\Sofco.WebJob
