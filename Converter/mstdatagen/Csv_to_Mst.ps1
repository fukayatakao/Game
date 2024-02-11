function createJson
{
    #jsonには不要なデータを削除したcsvを作る
    echo "create json"
    #メンバ名を一行目にする
    $mem = $member
    $mem = [regex]::replace("$mem", "^.", { $args.value.toUpper() }) 
    #memberのデリミタが", "とスペース付きになってるのに注意
    $mem = [regex]::replace("$mem", ", .", { $args.value.toUpper() }) 
    $mem = [regex]::replace("$mem", "_.", { $args.value.toUpper() }) 
    $mem = $mem.Replace("_", "")

    $json = $mem + "`n"



    for($r = 0; $r -le $data.Count; $r++){
        $val = $data[$r] -join ","
        $json += $val
    }


    $className = $tableName
    $className = [regex]::replace("$className", "^.", { $args.value.toUpper() }) 
    $className = [regex]::replace("$className", "_.", { $args.value.toUpper() }) 
    $className = $className.Replace("_", "")

    #結果をファイルに保存する
    $temppath = $path + "/" + $className + "_json_tmp.txt"
    $outpath = $path + "/" + $className + "_json.txt"
    #一旦csvに出力してjsonに変換する
    $json | Out-File $temppath -Encoding utf8
    gc $temppath | ConvertFrom-Csv | ConvertTo-Json | Out-File $outpath -Encoding utf8
    Remove-Item $temppath
}

function createInsert
{
    echo "create insert"

    #deleteを入れる
    $sql = "delete from $schoema.$tableName;`n"
    #insert文を作成
    $sql +="insert into $schoema.$tableName ($member) values`n"
    $max = $rows.Length
    
    for($r = $startRow; $r -le $max; $r++){
        #空行は無視
        if(!$rows[$r]){
            continue;
        }
        $cell = $rows[$r].Split(",");

        #左端にコメント指定があったら無視
        $comment = $cell[0]
        if($comment -and $comment.TrimStart().StartsWith("#")){
            continue
        }

        #プライマリ列が空の場合は無視する
        $p = $cell[$searchCol]
        if(!$p){
            continue
        }
        
        echo "Calculate $r / $max"

        #有効行の場合はinsert文を作る
        $delimiter=""
        $sql += "("
        for($i = 0; $i -le $fieldIndex.Length - 1; $i++){
            $index = $fieldIndex[$i]
            $val = $cell[$index]
            if(![String]::IsNullOrEmpty($val)){
                #改行コードはcsv時に\nに置換しているのでまた改行コードに戻す
                $val = $val.Replace("\n", "`n")
                
            }
            
            $val = "`"" + $val + "`""
            $sql += $delimiter + $val
            $delimiter=", "
        }
        $sql += "),`n"
    }

    $len = $sql.Length
    $sql = $sql.Remove($len - 2)
    $sql += ";`n"
    echo $sql




    #結果をファイルに保存する
    $filepath = $path + "/" + $tableName + "_insert.sql"

    $file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
    $file.Write($sql)
    $file.Close()
}

#create table文を生成
function createTable
{
    echo "create table"

    #deleteを入れる
    $sql = "DROP TABLE $schoema.$tableName;   -- " + "`n"
    #insert文を作成
    $sql +="CREATE TABLE $schoema.$tableName(`n"

    #primary keyの記述分を生成
    $keys=""
    $delimiter=""
    for($c=0; $c -lt $primary.Length; $c++){
        if($primary[$c] -eq 1){
            $keys+=$delimiter
            $keys+=$field[$c]
            $delimiter = ", "
        }
    }
 
    #primary keyがある場合はフィールド定義には必ずカンマがつく
    if($keys -ne ""){
        #フィールド定義を記述
        for($c=0; $c -lt $field.Length ; $c++){
            $sql+= "`t" + $field[$c] + " " + $type[$c].toUpper() + " NOT NULL,   -- " + $comment[$c] + "`n"
        }

        $sql +="`tPRIMARY KEY($keys)`n" 
    #primary keyがない場合はフィールドの最後の行にカンマが不要になる
    }else{
        for($c=0; $c -lt $field.Length - 1; $c++){
            $sql+= "`t" + $field[$c] + " " + $type[$c] + " NOT NULL,   -- " + $comment[$c] + "`n"
        }
        $c = $field.Length - 1
        $sql+= "`t" + $field[$c] + " " + $type[$c] + " NOT NULL   -- " + $comment[$c] + "`n"
    }

    $sql +=");"
    echo $sql




    #結果をファイルに保存する
    $filepath = $path + "/" + $tableName + "_create.sql"

    $file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
    $file.Write($sql)
    $file.Close()
}

#csソースを出力
function createCS
{
    echo "create cs"


    $cs ="using UnityEngine;`n"
    $cs +="using System;`n"
    $cs +="using System.Collections;`n"
    $cs +="using System.Collections.Generic;`n"
    $cs +="`n"
    $cs +="namespace Project.Mst {`n"
    $cs +="`t[System.Serializable]`n"

    #テーブルのスネークケースをアッパーキャメルに変換
    $className = $tableName
    $className = [regex]::replace("$className", "^.", { $args.value.toUpper() }) 
    $className = [regex]::replace("$className", "_.", { $args.value.toUpper() }) 
    $className = $className.Replace("_", "")
    
    $cs +="`tpublic partial class $className {`n"

    #メンバ定義を記述
    for($c=0; $c -lt $field.Length ; $c++){
        $m = $field[$c]
        #メンバのスネークケースをアッパーキャメルに変換
        $m = [regex]::replace("$m", "^.", { $args.value.toUpper() }) 
        $m = [regex]::replace("$m", "_.", { $args.value.toUpper() }) 
        $m = $m.Replace("_", "")

        #tableの型をcsの対応する型で定義
        $cs_type = $type[$c].toLower()
        #ここのif文を逐次増やす
        if($cs_type.StartsWith("varchar")){
            $cs_type = "string"
        }elseif($cs_type.StartsWith("tinyint")){
            $cs_type = "byte"
        }elseif($cs_type.StartsWith("smallint")){
            $cs_type = "short"
        }
        $cs+= "`t`tpublic " + $cs_type + " " + $m + ";" + " // " + $comment[$c] + "`n"
    }

    $cs +="`n"
    $cs+= "`t`tpublic static List<" + $className + "> GetList() {`n"
	$cs+= "`t`t`treturn BaseDataManager.GetList<" + $className + ">();`n"
	$cs+= "`t`t}`n"

    $cs +="`n"
	$cs+= "`t`tpublic static " + $className +" GetData(int id) {`n"
	$cs+= "`t`t`tDebug.Assert(BaseDataManager.GetDictionary<int, " + $className + ">().ContainsKey(id), typeof(" + $className + ").Name + "" not found id:"" + id);`n"
	$cs+= "`t`t`treturn BaseDataManager.GetDictionary<int, " + $className + ">()[id];`n"
	$cs+= "`t`t`}`n"




    $cs +="`t}`n"
    $cs +="}`n"
    echo $cs




    #結果をファイルに保存する
    $filepath = $path + "/" + $className + ".cs"

    $file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
    $file.Write($cs)
    $file.Close()
}



function create($csv, $schoema, $json_only){
    #変換元のExcelデータ
    $path = Split-Path -Parent $csv

    $rows = @()
    $rows = (Get-Content $csv) -as [string[]]
    
    
    #それぞれのデータ行のマーキング名
    set-variable -name tableDef -value "table" -option constant       #データ型行を示す定義名
    set-variable -name typeName -value "type" -option constant       #データ型行を示す定義名
    set-variable -name fieldName -value "name" -option constant      #カラム名
    set-variable -name primaryName -value "primary" -option constant #プライマリキーか？
    set-variable -name commentName -value "info" -option constant    #コメント
    set-variable -name startName -value "value" -option constant     #データ開始行

    $max = $rows.Length

    #テーブル名定義がない場合は自動生成なしで終了
    if($max -le 0 -or !$rows[0].TrimStart().ToLower().StartsWith($tableDef)){
        return;
    }
    $sp = $rows[0].Split(",")
    $tableName = $sp[1]
    echo $tableName

    echo "field = $max"
    for($r=0; $r -lt $max; $r++){
        #空行は無視
        if(!$rows[$r]){
            continue;
        }
 
        if($rows[$r].TrimStart().StartsWith($typeName)){
            $typeRow = $r
        }
        if($rows[$r].TrimStart().StartsWith($fieldName)){
            $fieldRow = $r
        }
        if($rows[$r].TrimStart().StartsWith($primaryName)){
            $primaryRow = $r
        }
        if($rows[$r].TrimStart().StartsWith($commentName)){
            $commentRow = $r
        }
        if($rows[$r].TrimStart().StartsWith($startName)){
            $startRow = $r
            break;
        }

    }

    #データ開始の定義がない場合はコメントの次の行をデータ開始とみなす
    if(!$startRow){
        $startRow = $commentRow + 1
    }

    #フィールド名と対応する列番号を配列に入れる
    $fieldIndex = @()
    $field = @()
    $type = @()
    $primary = @()
    $comment = @()
    

    $cell = $rows[$fieldRow].Split(",");
    $typeCell = $rows[$typeRow].Split(",");
    $primaryCell = $rows[$primaryRow].Split(",");
    $commentCell = $rows[$commentRow].Split(",");
    for($c=1; $c -le $cell.Length; $c++){
        $val = $cell[$c]
        #フィールド名が"#"から始まっている場合はコメントアウトされているとみなして無視する
        if($val -and !$val.TrimStart().StartsWith("#")){ 
            $fieldIndex+=$c
            $field += $val
            $type +=$typeCell[$c]
            $primary +=$primaryCell[$c]
            $comment+=$commentCell[$c]
        }
    }

    #プライマリ列がない場合はフィールドの最初の定義を使うようにする
    $searchCol=$fieldIndex[0]

    echo "primary check"

    $cell = $rows[$primaryRow].Split(",");
    foreach($index in $fieldIndex){
        #プライマリ設定がされているか
        $p = $cell[$index].Text

        if($p -and $p -ne "0") {
            $searchCol = $index;
            break;
        }
    }
    #無効行などを省いた有効なデータのみの情報を作る
    $data = New-Object "System.Collections.Generic.List[string[]]"
    for($r = $startRow; $r -le $max; $r++){
        #空行は無視
        if(!$rows[$r]){
            continue;
        }
        #左端にコメント指定があったら無視
        if($rows[$r] -and $rows[$r].TrimStart().StartsWith("#")){
            continue
        }
        $cell = $rows[$r].Split(",");

        #プライマリ列が空の場合は無視する
        $p = $cell[$searchCol]
        if(!$p){
            continue
        }
        $line = New-Object string[] ($fieldIndex.Length + 1)
        for($i = 0; $i -le $fieldIndex.Length - 1; $i++){
            $line[$i] = $cell[$fieldIndex[$i]]
        }
        $line[$fieldIndex.Length] = "`n"
        $data.Add($line)
    }


    echo "create field"

    #テーブルのカラム名を取得
    $delimiter=""
    $member=""
    foreach($f in $field){
        $member += $delimiter + $f
        $delimiter=", "
    }
    echo $json_only

    if($json_only -ne "true"){
        #データのinsert文を生成
        createInsert
        #オフライン動作用のjsonデータを生成
        createJson
        #create table文を生成
        createTable
        #csファイルを出力
        createCS
    }else{
        #オフライン動作用のjsonデータを生成
        createJson       
    }
}



if($Args[0] -and $Args[1] -and $Args[2]){
    create $Args[0] $Args[1] $Args[2]
}else{
    echo "引き数が正しくありません。[Excelファイル][スキーマ名][フラグ]を入れてください"
}


