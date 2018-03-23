@ECHO OFF

SET WORKSPACE_PATH=%~1

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar

cd "%WORKSPACE_PATH%"

cmd /c dotnet "C:\Pancho\Install.Programmes\SonarQubeScanner\netcoreapp2\SonarScanner.MSBuild.dll" begin /k:"Gaps-ApiJob:develop" /d:sonar.host.url="http://azsof01wd:9100" /d:sonar.login="admin" /d:sonar.password="admin"
cmd /c dotnet build
cmd /c dotnet "C:\Pancho\Install.Programmes\SonarQubeScanner\netcoreapp2\SonarScanner.MSBuild.dll" end /d:sonar.login="admin" /d:sonar.password="admin"
