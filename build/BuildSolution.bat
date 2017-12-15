@ECHO OFF

SET MSBUILD_PATH=C:\Program Files (x86)\MSBuild\14.0\Bin

SET WORKSPACE_PATH=%~1

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar

cd "%WORKSPACE_PATH%"

dotnet restore

"%MSBUILD_PATH%\MSBuild.exe" Sofco.sln

cmd /c dotnet test src/Sofco.UnitTest
