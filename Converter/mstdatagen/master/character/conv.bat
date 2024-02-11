@echo off
rem ファイル名の一覧を取得
set TEMP_PATH=%~dp0
set SCHEMA=parade_base

set RELATIVE_PATH=..\..\
set ASSET_RELATIVE_PATH=..\..\..\..\
set JSON_PATH=%RELATIVE_PATH%output\json\

rem xlsからcsvファイルを作成
for %%A in (*.xls) do (
 	powershell -ExecutionPolicy RemoteSigned -File %RELATIVE_PATH%Excel_to_Csv.ps1 %TEMP_PATH%%%A
)
rem csvからsql,cs,jsonファイルを作成
for %%A in (*.csv) do (
	powershell -ExecutionPolicy RemoteSigned -File %RELATIVE_PATH%Csv_to_Mst.ps1 %TEMP_PATH%%%A %SCHEMA% true
	del %TEMP_PATH%%%A
)
rem csvファイルから各種ソース・データを生成
for %%A in (*.csv) do (
    	powershell -ExecutionPolicy RemoteSigned -File  %RELATIVE_PATH%Csv_to_Enum.ps1 %TEMP_PATH%%%A
)

rem jsonを移動
for %%A in (*_json.txt) do (
	move %TEMP_PATH%%%A %ASSET_RELATIVE_PATH%Assets\Application\Json\Resources\
)

rem 不要になったcsv削除
for %%A in (*.csv) do (
	del %TEMP_PATH%%%A
)
rem 不要なcs削除
for %%A in (*.cs) do (
	del %TEMP_PATH%%%A
)
