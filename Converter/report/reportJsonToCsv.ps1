echo "Convert Start"
$path="../Report"
$itemList = Get-ChildItem ($path + "/*.txt") -Recurse;
$csv = ""

$csv += "プレイヤーHP`tプレイヤーLp`tプレイヤーユニット数`tエネミーHP`tエネミーLP`tエネミーユニット数`n"

foreach($item in $itemList){
	echo $item.Fullname
	$json = Get-Content -Path ($item.Fullname) -Encoding UTF8 -Raw | ConvertFrom-Json

	$csv += $json.result.playerHp.ToString("0.00") + "`t" + $json.result.playerLp.ToString("0.00") + "`t" + [string]$json.result.playerNumber + "`t"
	$csv += $json.result.enemyHp.ToString("0.00") + "`t" + $json.result.enemyLp.ToString("0.00") + "`t" + [string]$json.result.enemyNumber + "`n"
}

$filepath = "output/result.csv"
$file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
$file.Write($csv)
$file.Close()