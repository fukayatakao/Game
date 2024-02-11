. "./commonFunc.ps1"

function clinetSrc($str, $text, $sub){
	#xmlの内容で置換する
	#####
	#継承クラス準備
	$extends = ""
	if(![string]::IsNullOrEmpty($str.Struct.Extends)){
		$extends = " : "+$str.Struct.Extends;
	} 
	$attr = ""
	if($str.Struct.fixdel){
		$attr = "`t[System.Obsolete(`"FIXME:DELETE`")]`n"
	}
	######################################################################################################
	#単純置換するもの
	$text = $text.Replace('{{{$CLASS_ATTR$}}}', $attr)
	$text = $text.Replace('{{{$SCRIPTNAME$}}}', $str.Struct.ClassName+$Extends)

	######################################################################################################
	#response設定
	$RESPONSE_VARIABLES = ""
	foreach ($member in $str.Struct.member) {
		$type = TypeToClinet $member.type
		$name = $member.name
		$comment = $member.comment
		$attr = ""
		if($member.fixdel){
			$attr = "`t`t[System.Obsolete(`"FIXME:DELETE`")]`n"
		}

		$RESPONSE_VARIABLES += "$attr`t`tpublic $type $name; //$comment`n"
	}
	$text = $text.Replace('{{{$RESPONSE_VARIABLES$}}}', $RESPONSE_VARIABLES)

	#結果をファイルに保存する
	$filepath = $setting.Setting.dst_client_struct_dir+ "/" + $sub + "/" + $str.Struct.ClassName + ".cs"
	
	New-Item $filepath -Force -Value $text
	#$file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
	#$file.Write($text)
	#file.Close()

}


$setting = [xml](Get-Content setting.xml)

if(Test-Path $setting.Setting.dst_client_struct_dir){
	Remove-Item $setting.Setting.dst_client_struct_dir -Recurse
}

#テンプレートを読み込んで変数に貯める
$f = (Get-Content ($setting.Setting.template_dir + "/StructTemplete.cs.txt")) -as [string[]]
$i=1

$templateCS=""
foreach ($l in $f) {
	$templateCS += ($l + "`n")
	$i++
}


$path=$setting.Setting.src_struct_dir
$itemList = Get-ChildItem ($path + "/*.json") -Recurse;
foreach($item in $itemList){
	echo $item.Fullname
	$strfile = Get-Content -Path ($item.Fullname) -Encoding UTF8 -Raw | ConvertFrom-Json



	#カレントパス+相対パス
	$p = (Convert-Path $path) 
	#定義ファイルのディレクトリパスから設定ファイルで指定したパスの文字列を削除
	$subdir = $item.DirectoryName.Replace($p, "")

	#サブディレクトリの文字列を抜き出し
	if ($subdir.Length -ne 0){
		$subdir = $subdir.Substring(1, $subdir.Length - 1);
		echo $subdir
	}
	
	clinetSrc $strfile $templateCS $subdir

}
