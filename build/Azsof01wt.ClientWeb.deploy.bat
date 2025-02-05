SET PROJECT_NAME=Sofco.ClientWeb

SET PUBLISH_PATH=C:\Temp\PublishTemp\%PROJECT_NAME%

SET WORKSPACE_PATH=%~1

SET DEPLOY_PATH=%~2

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar
IF [%2] == [] SET DEPLOY_PATH=C:\Projects\Deploys\Sofco.WebApi\wwwroot

cd "%WORKSPACE_PATH%\src\%PROJECT_NAME%"

cmd /c npm install

cmd /c DEL /F /Q "%PUBLISH_PATH%"
cmd /c FOR /D %%p IN ("%PUBLISH_PATH%\*.*") DO rmdir "%%p" /s /q

cmd /c ng build --prod --configuration=azsof01wt --output-path "%PUBLISH_PATH%"

@if %errorlevel% neq 0 exit /b %errorlevel%

cmd /c COPY /Y /S "%PUBLISH_PATH%" "%DEPLOY_PATH%"
