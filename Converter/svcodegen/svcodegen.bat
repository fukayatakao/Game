powershell -ExecutionPolicy RemoteSigned -File convertAction.ps1
powershell -ExecutionPolicy RemoteSigned -File convertStruct.ps1

set CLIENT_ACTION_SOURCE_PATH=..\..\Assets\Application\Script\Network\Autogenerate\Protocol\Action\
set CLIENT_STRUCT_SOURCE_PATH=..\..\Assets\Application\Script\Network\Autogenerate\Protocol\Struct\

if exist %CLIENT_ACTION_SOURCE_PATH% (
       echo クライアントソースのコピー
       xcopy /s /y output\action\client\*.cs %CLIENT_ACTION_SOURCE_PATH%
)
if exist %CLIENT_STRUCT_SOURCE_PATH% (
       echo クライアントソースのコピー
       xcopy /s /y output\struct\client\*.cs %CLIENT_STRUCT_SOURCE_PATH%
)
set SERVER_SOURCE_PATH=..\..\socialsv\WebContent\WEB-INF\src\jp\co\sega\ribbon\response\

rem if exist %SERVER_SOURCE_PATH% (
rem        echo クライアントソースのコピー
rem        xcopy /s /y output\action\server\*.java %SERVER_SOURCE_PATH%
rem        xcopy /s /y output\struct\server\*.java %SERVER_SOURCE_PATH%
rem )