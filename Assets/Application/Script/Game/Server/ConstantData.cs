using System.Collections.Generic;

namespace Project.Server {
	public static class ConstantData {
		public static List<string> CharaName = new List<string>()
		{
			"アーロン",
"エイベル",
"エイブラハム",
"エイブラム",
"アダム",
"アドルフ",
"エイドリアン",
"アラン",
"アラスター（アラステア）",
"アルバート",
"アレックス",
"アレグザンダー（アレクサンダー）",
"アレクシス",
"アルフ",
"アルフィー",
"アルフレッド",
"アルジャノン、アルジャーノン",
"アリスター",
"アリスター",
"アラン",
"アレン",
"アーリン",
"アルヴィン",
"アンブローズ",
"アンドルー（アンドリュー）",
"アンディ",
"アンガス",
"アントニー、アンソニー",
"アントン",
"アントニー",
"アーチボルド",
"アリエル、エリアル",
"アーノルド",
"アーサー",
"アシュリー",
"アシュトン",
"オーブリー",
"オーガスト",
"オーガスタス",
"エイルマー、エルマー",
"ボールドウィン",
"バーナビー",
"バーニー",
"バリー",
"バーソロミュー",
"バージル、ベイジル",
"ベン",
"ベネディクト",
"ベンジャミン",
"バーナード",
"バート",
"バートランド",
"ビル",
"ビリー",
"ボブ",
"ボビー",
"ブラッド",
"ブラッドフォード",
"ブラッドリー",
"ブランドン",
"ブレンダン",
"ブレンドン",
"ブレント",
"ブレット",
"ブライアン",
"ブルース",
"ブルーノ",
"ブライアン",
"バイロン",
"カルヴィン",
"キャメロン",
"ケアリー",
"カール",
"キャロル",
"ケイシー",
"セシル",
"セドリック",
"チャド",
"チャールズ",
"チャーリー",
"チェスター",
"クリス",
"クリスティアン",
"クリストファー",
"クラレンス",
"クラーク",
"クリフ",
"クリフォード",
"クリフトン",
"クライヴ",
"クライド",
"コーディ",
"コリン",
"コンラッド",
"コンスタント",
"コーニーリアス",
"コリー",
"クレイグ",
"カーティス",
"シリル",
"サイラス",
"デール",
"デイミアン",
"デイモン",
"ダン",
"ダニエル",
"ダニー",
"ダライアス",
"ダレル",
"ダーレン",
"ダリル",
"ダリル",
"デイヴ",
"デイヴィッド",
"ディーン",
"デニス",
"デニス",
"デレク",
"デリック",
"デリック",
"デリック",
"デズモンド",
"デクスター",
"ディック",
"ドルフ",
"ドム",
"ドミニク",
"ドン",
"ドナルド",
"ダグラス",
"ドルー",
"ダドリー",
"デューク",
"ダンカン",
"ダスティン",
"ドウェイン",
"イーモン",
"アール",
"エディー",
"エディ",
"イーデン",
"エドガー",
"エドマンド",
"エドワード",
"エドウィン",
"エグバート",
"エルドレッド",
"イライアス",
"イライジャ",
"エリオット",
"エリオット",
"エリス",
"エルマー",
"エルトン",
"エルヴィス",
"エメリー",
"エマニュエル",
"イーノック",
"エリック",
"アーネスト",
"イーサン",
"エセルバート",
"ユージーン",
"エヴァン",
"ユーイン",
"フェイビアン",
"フェリックス",
"ファーディナンド",
"フランシス",
"フランク",
"フランクリン",
"フレッド",
"フレディ",
"フレドリック",
"ゲイブリエル",
"ギャリー",
"ジーン",
"ジェフ",
"ジェフリー",
"ジョージ",
"ジェラルド",
"ジェラード",
"ジェリー",
"ギディオン",
"ギル",
"ギルバート",
"ジャイルズ",
"グレン",
"グレン",
"ゴドフリー",
"ゴドウィン",
"ゴードン",
"グレアム",
"グラントリー",
"グレッグ",
"グレゴリー",
"ガス",
"ガイ",
"ハドリー",
"ハミルトン",
"ハロルド",
"ハリスン",
"ハリー",
"ハーヴィー",
"ヘイデン",
"ヘクター",
"ヘンリー",
"ハーバート",
"ハーマン",
"ヒラリー",
"ホレス",
"ホレイシオ",
"ハワード",
"ヒューバート",
"ヒュー",
"ヒューゴー",
"ハンフリー",
"イアン",
"アーヴィン",
"アーヴィング",
"アイザック",
"アイヴァン",
"ジャック",
"ジャッキー",
"ジャクソン",
"ジェイコブ",
"ジェイラス",
"ジェイク",
"ジェームズ",
"ジェイミー",
"ジャレッド",
"ジェイソン",
"ジャスパー",
"ジェフ",
"ジェフリー",
"ジェフリー",
"ジェレマイア",
"ジェレミー",
"ジェローム",
"ジェリー",
"ジェシー",
"ジム",
"ジミー",
"ジミー",
"ジョスリン",
"ジョール",
"ジョン",
"ジョニー",
"ジョン",
"ジョナス",
"ジョナサン",
"ジョーダン",
"ジョーセフ",
"ジョッシュ",
"ジョシュア",
"ジョザイア",
"ジュード",
"ジュリアン",
"ジャスティン",
"キース",
"ケネス",
"ケント",
"ケヴィン",
"キム",
"カイル",
"ランドン",
"ローレンス",
"ローレンス",
"リー",
"リオ",
"リオン",
"レナード",
"レスリー",
"レスター",
"リーヴァイ",
"ルイス",
"レックス",
"リーアム",
"リンジー",
"リンジー",
"ライナス",
"ライオネル",
"ロイド",
"ロニー",
"ルイス",
"ルーカス",
"ルーシャン",
"ルーク",
"ルーサー",
"マルコム",
"マヌエル",
"マーカス",
"マーク",
"マーティン",
"マーティー",
"マーヴィン",
"マシュー",
"マシュー",
"モーリス",
"マックス",
"マクシミリアン",
"メイナード",
"メルヴィン",
"メレディス",
"マイケル",
"ミック",
"ミッキー",
"ミッキー",
"マイク",
"マイルズ",
"ミッチェル",
"モンタギュー",
"モーガン",
"モーリス",
"モージズ",
"マイルズ",
"ネイト",
"ネイサン",
"ナサニエル",
"ニール",
"ニコラス",
"ニック",
"ニコラス",
"ナイジェル",
"ノア",
"ノエル",
"ノーマン",
"オリヴァー",
"オスカー",
"オズワルド",
"オスニエル",
"オーウェン",
"パスカル",
"パット",
"パトリック",
"ポール",
"パーシヴァル",
"パーシヴァル",
"パーシー",
"ピーター",
"フィル",
"フィランダー",
"フィリップ",
"フィリップ",
"クェンティン",
"クィンシー",
"レイフ",
"レイフ",
"ランドル",
"ランドルフ",
"ランドルフ",
"ランディー",
"ラフェエル",
"レイ",
"レイモンド",
"レジナルド",
"レックス",
"レナルド",
"リチャード",
"リック",
"リッキー",
"ロブ",
"ロバート",
"ロビン",
"ロードリック",
"ロジャー",
"ロドニー",
"ロジャー",
"ローランド",
"ローマン",
"ロナルド",
"ロニー",
"ローランド",
"ロイ",
"ルーファス",
"ルパート",
"ラッセル",
"ラッセル",
"サム",
"サミー",
"サミュエル",
"サンディー",
"スコット",
"セバスチャン",
"ショーン",
"シドニー",
"サイラス",
"シルヴェスター",
"シミオン",
"サイモン",
"ソロモン",
"スペンサー",
"スタンリー",
"スティーヴン",
"スティーヴ",
"スティーヴン",
"スティーヴィー",
"スチュアート",
"スチュアート",
"シルヴェスター",

		};


	}
}
