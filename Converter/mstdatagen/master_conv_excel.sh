#$schema=$1
path=$(cd $(dirname $0); pwd)
schema="schema_base"

path+="/master"
echo $path
echo $schema

#csvファイルを作成
for file in `\find $path -name '*.xls'`; do
    echo "file = $file"
    powershell -ExecutionPolicy RemoteSigned -File Excel_to_Csv.ps1 $file
done
#csvファイルから各種ソース・データを生成
for file in `\find $path -name '*.csv'`; do
    echo "file = $file"
    powershell -ExecutionPolicy RemoteSigned -File Csv_to_Mst.ps1 $file $schema false
done

#csvファイルから各種ソース・データを生成
for file in `\find $path -name '*.csv'`; do
    echo "file = $file"
    powershell -ExecutionPolicy RemoteSigned -File Csv_to_Enum.ps1 $file
done


output_path="./output"
if [ -d $output_path ]; then
    rm -r $output_path
fi
mkdir $output_path

cs_path="./output/cs/"
csv_path="./output/csv/"
sql_path="./output/sql/"
json_path="./output/json/"


mkdir $cs_path
mkdir $csv_path
mkdir $sql_path
mkdir $json_path



#csファイルを所定の場所に移動
for file in `\find $path -name '*.cs'`; do
    mv $file $cs_path
done
#csvファイルを所定の場所に移動
for file in `\find $path -name '*.csv'`; do
    mv $file $csv_path
done
#jsonファイルを所定の場所に移動
for file in `\find $path -name '*_json.txt'`; do
    mv $file $json_path
done
#sqlファイルを所定の場所に移動
for file in `\find $path -name '*.sql'`; do
    mv $file $sql_path
done



##########################################
#csファイルを所定の場所に移動
for file in `\find $cs_path -name '*.cs'`; do
    cp $file ../../Assets/Application/Script/Network/Autogenerate/Master/
done

#jsonファイルを所定の場所に移動
for file in `\find $json_path -name '*_json.txt'`; do
    cp $file ../../Assets/Application/Json/Resources/
done

