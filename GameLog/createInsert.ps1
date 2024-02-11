param (
	$tableName,
	$prefix
)

# フォルダのパスとテーブル名
$folderPath = "./Log"

# フォルダ内のCSVファイルを取得
$csvFiles = Get-ChildItem -Path $folderPath -Filter "$prefix*.txt"

# INSERT INTO文を生成

# CSVファイルごとにデータを取得し、INSERT INTO文に追加
foreach ($csvFile in $csvFiles) {
    $csvData = (Get-Content -Encoding UTF8 $csvFile.FullName) -as [string[]]
    
	$insertStatement = "INSERT INTO $tableName ("
    $insertStatement += $csvData[0] -join ", "
    $insertStatement += ") VALUES "
    
	foreach ($row in $csvData) {
		$insertStatement += "($row), `r`n"
		#echo $line
    }
	# INSERT INTO文を表示する
	$insertStatement = $insertStatement.Substring(0, $insertStatement.LastIndexOf(',')) + ";`r`n"

    #結果をファイルに保存する
    $filepath = "./insert/" + $csvFile.name.Substring(0, $csvFile.name.LastIndexOf('.')) + "_insert.sql"

    $file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
    $file.Write($insertStatement)
    $file.Close()
}


