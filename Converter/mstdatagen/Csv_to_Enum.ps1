function createCS
{
    $cs ="using UnityEngine;`n"
    $cs +="using System;`n"
    $cs +="using System.Collections;`n"
    $cs +="using System.Collections.Generic;`n"
    $cs +="using Project.Lib;`n"
    $cs +="`n"
    $cs +="namespace Project.Http.Mst {`n"

    #テーブルのスネークケースをアッパーキャメルに変換
    $className = $enumName
    
    $cs +="`t//$comment`n"
    $cs +="`tpublic enum $className {`n"

    $val = -1;
    #メンバ定義を記述
    for($c=0; $c -lt $field.Length ; $c++){
        $m = $field[$c].toUpper();
        if(![string]::IsNullOrEmpty($fieldValue[$c])){
            $val = [int]$fieldValue[$c]
        }else{
            $val += 1;
        }
        #if(![string]::IsNullOrEmpty($fieldExt)){
            $cs+= "`t`t[Field(`"" + $fieldComment[$c] + "`")] "+ $m + " = " + $val + "," + "`n"
        #}else{
        #    $cs+= "`t`t" + $m + " = " + $val + "," + " // " + $fieldComment[$c] + "`n"
        #}
    }




    $cs +="`t}`n"
    $cs +="}`n"
    echo $cs




    #結果をファイルに保存する
    $filepath = $path + "/" + $className + ".cs"

    $file = New-Object System.IO.StreamWriter($filepath, $false, (New-Object System.Text.UTF8Encoding($false)))
    $file.Write($cs)
    $file.Close()
}
function createEnum($csv){
    #変換元のExcelデータ
    $path = Split-Path -Parent $csv

    $rows = @()
    $rows = (Get-Content $csv) -as [string[]]
    
    
    #それぞれのデータ行のマーキング名
    set-variable -name enumDef -value "enum" -option constant       #データ型行を示す定義名
    $max = $rows.Length


    for($r=0; $r -lt $max; $r++){
        if($rows[$r].TrimStart().StartsWith($enumDef)){
            $comment = $rows[$r - 1].TrimStart().Split(",")[0]
            $enumName = $rows[$r].Split(",")[1];
            $fieldExt = $rows[$r].Split(",")[3];
            $field = @()
            $fieldValue = @()
            $fieldComment = @()
            for($i = $r + 1; $i -lt $max; $i++){
                if($rows[$i].TrimStart().StartsWith($enumDef)){
                    break;
                }
                $l = $rows[$i].Replace(",", "");
                if([string]::IsNullOrEmpty($l.Trim())){
                    break;
                }

                $cell = $rows[$i].Split(",");

                $field+=$cell[1];
                $fieldValue+=$cell[2];
                $fieldComment+=$cell[3];

            }
            echo "enum = $enumName"
            createCS
        }
    }

}



if($Args[0]){
    createEnum $Args[0]
}else{
    echo "引き数が正しくありません。[Excelファイル]を入れてください"
}


