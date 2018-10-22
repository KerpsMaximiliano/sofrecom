SET PROJECT_NAME=Sofco.ClientWeb

SET PUBLISH_PATH=C:\Projects\Sofcoar\src\%PROJECT_NAME%\dist

SET WORKSPACE_PATH=%~1

SET DEPLOY_PATH=%~2

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar
IF [%2] == [] SET DEPLOY_PATH=C:\Projects\Deploys.Dev\Sofco.WebApi\wwwroot

cd "%WORKSPACE_PATH%\src\%PROJECT_NAME%"

cmd /c npm install

REM cmd /c DEL /F /Q "%PUBLISH_PATH%"
REM cmd /c FOR /D %%p IN ("%PUBLISH_PATH%\*.*") DO rmdir "%%p" /s /q

REM cmd /c ng build --prod --configuration=azsof01wd-dev --extract-css=false -op "%PUBLISH_PATH%"
cmd /c ng build --prod --configuration=azsof01wd-dev

@if %errorlevel% neq 0 exit /b %errorlevel%

cmd /c XCOPY /S "%PUBLISH_PATH%" "%DEPLOY_PATH%"
