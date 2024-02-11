. "./commonFunc.ps1"

function clinetSrc($act, $text, $sub){
	#xmlの内容で置換する
	######################################################################################################
	#単純置換するもの
	$text = $text.Replace('{{{$SCRIPTNAME$}}}', $act.Action.ClassName)

	$attr = ""
	if($act.Action.fixdel){
		$attr = "`t[System.Obsolete(`"FIXME:DELETE`")]`n"
	}
	$text = $text.Replace('{{{$CLASS_ATTR$}}}', $attr)
	
	######################################################################################################
	#configに関する設定を書き込み
	$config = ""
	$indent = "`t`t`t`t"
	if (![string]::IsNullOrEmpty($act.Action.Config.api)) { $config += $indent + "Api = `"" + $act.Action.Config.api + """,`n" }
	else{ $config += $indent + "Api = `"" + $act.Action.ClassName + ".do`",`n" }
	if ($act.Action.Config.server) { $config += $indent + "Server = Command.Config.ServerRoot[ `"" + $act.Action.Config.server + "`" ],`n" }
	if ($act.Action.Config.useragent) { $config += $indent + "UserAgent = Command.Config.UserAgent[ `"" + $act.Action.Config.useragent + "`" ],`n" }
	#暗号化使う場合はセッションの設定によって分岐
	if ($act.Action.Config.encrypt -And $act.Action.Config.encrypt -eq "true") { 
		if($act.Action.Config.session -And $act.Action.Config.session -eq "false"){
			$config += $indent + "EncryptKey = Command.Config.DefaultEncryptKey,`n"
		}else{
			$config += $indent + "EncryptKey = Command.Config.AccountEncryptKey,`n"
		}
	}else{
		$config += $indent + "EncryptKey = `"`",`n"
	}
	#設定がない場合はデフォルトを使用
	if ($act.Action.Config.timeout) { $config += $indent + "TimeoutTime = " + $act.Action.Config.timeout + "f,`n" }
	else{ $config += $indent + "TimeoutTime = CommandSetting.DEFAULT_TIMEOUT_TIME,`n" }

	if ($act.Action.Config.retry) { $config += $indent + "RetryCount = " + $act.Action.Config.retry + ",`n" }
	else{ $config += $indent + "RetryCount = CommandSetting.DEFAULT_RETRY_COUNT,`n" }

	if ($act.Action.Config.post) { $config += $indent + "IsPostMethod = " + $act.Action.Config.post + ",`n" }

	if ($act.Action.Config.offline) { $config += $indent + "IsOffline = " + $act.Action.Config.offline + ",`n" }

	$text = $text.Replace('{{{$CONFIG$}}}', $config)

	######################################################################################################
	#request設定
	$val = "req"
	$text = $text.Replace('{{{$VAL$}}}', $val)

	#最初の１回目は区切りを入れないようにする
	$delimiter=""
	$REQUEST_ARGUMENTS = ""
	$REQUEST_VARIABLE_ARGUMENTS = ""
	$REQUEST_VARIABLE_ASSIGNMENT = ""
	$REQUEST_VARIABLES = ""
	$REQUEST_VARIABLES_FIELDS = ""

	foreach ($member in $act.Action.Request.member) {
		$type = TypeToClinet $member.type
		$name = $member.name
		$comment = $member.comment

		$REQUEST_ARGUMENTS += "$delimiter$type $name"
		$REQUEST_VARIABLE_ARGUMENTS += "$delimiter$name"
		$REQUEST_VARIABLE_ASSIGNMENT += "`t`t`tthis.$name = $name;`n"
		$REQUEST_VARIABLES += "`t`tpublic $type $name; //$comment`n"
		if($type.Contains("List")){
			$REQUEST_VARIABLES_FIELDS += "`t`t`tif($name != null){`n"
			$REQUEST_VARIABLES_FIELDS += "`t`t`t`tfor(int i = 0, max = $name.Count; i < max; i++){`n"
			$REQUEST_VARIABLES_FIELDS += "`t`t`t`t`tAddField(`"$name`", $name[i].ToString(), ref fields);`n"
			$REQUEST_VARIABLES_FIELDS += "`t`t`t`t}`n"
			$REQUEST_VARIABLES_FIELDS += "`t`t`t}`n"
		}else{
			$REQUEST_VARIABLES_FIELDS += "`t`t`tAddField(`"$name`", $name.ToString(), ref fields);`n"
		}
		$delimiter=", "
	}
	$text = $text.Replace('{{{$REQUEST_ARGUMENTS$}}}', $REQUEST_ARGUMENTS)
	$text = $text.Replace('{{{$REQUEST_VARIABLE_ARGUMENTS$}}}', $REQUEST_VARIABLE_ARGUMENTS)
	$text = $text.Replace('{{{$REQUEST_VARIABLE_ASSIGNMENT$}}}', $REQUEST_VARIABLE_ASSIGNMENT)
	$text = $text.Replace('{{{$REQUEST_VARIABLES$}}}', $REQUEST_VARIABLES)
	$text = $text.Replace('{{{$REQUEST_VARIABLES_FIELDS$}}}', $REQUEST_VARIABLES_FIELDS)

	#メンバが一つもない場合は範囲内を丸ごと削除。それ以外はキーワードのみ削除
	if(!$act.Action.Request.member){
		$text = $text -replace '>>>\$REQUEST_PARAM_NONE_CONSTRUCTOR\$\n.*\n<<<\$REQUEST_PARAM_NONE_CONSTRUCTOR\$\n', ""
	}else{
		$text = $text -replace '>>>\$REQUEST_PARAM_NONE_CONSTRUCTOR\$\n', ""
		$text = $text -replace '<<<\$REQUEST_PARAM_NONE_CONSTRUCTOR\$\n', ""
	}
	######################################################################################################
	#response設定
	$RESPONSE_VARIABLES = ""
	foreach ($member in $act.Action.Response.member) {
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
	$filepath = $setting.Setting.dst_client_action_dir+ "/" + $sub + "/" + $act.Action.ClassName + "Cmd.cs"
	

	New-Item $filepath -Force -Value $text
	#$file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
	#$file.Write($text)
	#$file.Close()

}

$setting = [xml](Get-Content setting.xml)

#前回の結果を削除
if(Test-Path $setting.Setting.dst_client_action_dir){
	Remove-Item $setting.Setting.dst_client_action_dir -Recurse
}

#テンプレートを読み込んで変数に貯める
$f = (Get-Content ($setting.Setting.template_dir + "/ActionTemplete.cs.txt")) -as [string[]]
$i=1

$templateCS=""
foreach ($l in $f) {
	$templateCS += ($l + "`n")
	$i++
}

$path=$setting.Setting.src_action_dir
$itemList = Get-ChildItem -Path ($path + "/*.json") -Recurse;

foreach($item in $itemList){
	echo $item.Fullname
	$actfile = Get-Content -Path ($item.Fullname) -Encoding UTF8 -Raw | ConvertFrom-Json

	#カレントパス+相対パス
	$p = (Convert-Path $path) 
	#定義ファイルのディレクトリパスから設定ファイルで指定したパスの文字列を削除
	$subdir = $item.DirectoryName.Replace($p, "")

	#サブディレクトリの文字列を抜き出し
	if ($subdir.Length -ne 0){
		$subdir = $subdir.Substring(1, $subdir.Length - 1);
		echo $subdir
	}

	clinetSrc $actfile $templateCS $subdir

}
