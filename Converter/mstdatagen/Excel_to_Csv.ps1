
function createCsv($excel){
    #変換元のExcelデータ
    echo $excel

    #オブジェクトを変数に設定
    $objExcel = New-Object -ComObject Excel.Application 
    #警告なしで上書きする
    $objExcel.DisplayAlerts = $false

    $book = $objExcel.Workbooks.Open($excel)
 

    $xCSV = 6
    $i = 0
    $path = Split-Path -Parent $excel
 
    foreach($sheet in $book.Sheets){
         #改行コードを\nにして出力
        $sheet.Cells.Replace("`n", "\n", [Microsoft.Office.Interop.Excel.XlLookAt]::xlPart)
        $sheet.Cells.Replace("`r", "", [Microsoft.Office.Interop.Excel.XlLookAt]::xlPart)
        $sheet.Select()
        $book.SaveAs($path + "/" + $sheet.Name + ".csv", $xCSV)
        $i += 1
    }
    $book.Close($false)
    $objExcel.Quit()
}




if($Args[0]){
    createCsv($Args[0])
}else{
    echo "Excelが指定されてません"
}
