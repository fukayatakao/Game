# プリミティブ型列挙
$primitive = New-Object System.Collections.Generic.HashSet[string](,[string[]](`
	# 共通
	"short",`
	"int",`
	"long",`
	"float",`
	"double",`
	"byte",
	# cs
	"bool",`
	"string",`
	# java
	"Boolean",`
	"String",`
	"Short",`
	"Integer",`
	"Long",`
	"Float",`
	"Double",`
	"Byte"`
))


#cs->java用に型を適用させる
function TypeToClinet($type) {
	$type = $type.trim()
	if(($type -match "^List<(?<in_type>.*?)>$") -eq $true){
		$in_type = $Matches["in_type"]
		$type = "List<"+(TypeToClinet $in_type)+">"
	}elseif((($type -match "^Map<(?<key>.*?),(?<val>.*?)>$") -eq $true) -Or (($type -match "^Dictionary<(?<key>.*?),(?<val>.*?)>$") -eq $true)){
		$key = $Matches["key"]
		$val = $Matches["val"]
		$type = "Dictionary<"+(TypeToClinet $key)+","+(TypeToClinet $val)+">"
	}else{
		if($primitive.Contains($type)){
			switch($type){
				"String" { $type = "string"}
				"boolean" { $type = "bool"}
				#java→c#変換のみ
				"Boolean" { $type = "bool"}
				"Integer" { $type = "int"}
				"Short" { $type = "short"}
				"Long" { $type = "long"}
				"Float" { $type = "float"}
				"Double" { $type = "double"}
				"Byte" { $type = "byte"}
			}
		}
	}
	return $type
}


#cs->java用に型を適用させる
function TypeToServer($type) {
	$type = $type.trim()
	if(($type -match "^List<(?<in_type>.*?)>$") -eq $true){
		$in_type = $Matches["in_type"]
		$type = "List<"+(InTypeToServer $in_type)+">"
	}elseif((($type -match "^Map<(?<key>.*?),(?<val>.*?)>$") -eq $true) -Or (($type -match "^Dictionary<(?<key>.*?),(?<val>.*?)>$") -eq $true)){
		$key = $Matches["key"]
		$val = $Matches["val"]
		$type = "Map<"+(InTypeToServer $key)+","+(InTypeToServer $val)+">"
	}else{
		if($primitive.Contains($type)){
			switch($type){
				"string" { $type = "String" }
				"bool" { $type = "boolean" }
			}
		}else{
			$type = $type+"Resdata"
		}
	}
	return $type
}
function InTypeToServer($type){
	$type = $type.trim()
	if(($type -match "^List<(?<in_type>.*?)>$") -eq $true){
		$in_type = $Matches["in_type"]
		$type = "List<"+(InTypeToServer $in_type)+">"
	}elseif((($type -match "^Map<(?<key>.*?),(?<val>.*?)>$") -eq $true) -Or (($type -match "^Dictionary<(?<key>.*?),(?<val>.*?)>$") -eq $true)){
		$key = $Matches["key"]
		$val = $Matches["val"]
		$type = "Map<"+(InTypeToServer $key)+","+(InTypeToServer $val)+">"
	}else{
		if($primitive.Contains($type)){
			switch($type){
				"string" { $type = "String" }
				"bool" { $type = "Boolean" }
				"int" { $type = "Integer" }
				"long" { $type = "Long" }
				"short" { $type = "Short" }
				"float" { $type = "Float" }
				"double" { $type = "Double" }
				"byte" { $type = "Byte" }
			}
		}else{
			$type = $type+"Resdata"
		}
	}
	return $type
}