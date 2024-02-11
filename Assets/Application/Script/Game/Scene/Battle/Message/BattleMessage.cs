using System;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// オブジェクト間処理の仲介
    /// </summary>
    public sealed class BattleMessageSetting : IMessageSetting {
        //メッセージの受け取り関数名はこの名前でないと受け取れない
        public string RecvFuncName{ get { return "Recv"; } }
        //メッセージクラスの中にこの文字列のメンバ変数が存在しないといけない
        public string IdFieldName{ get { return "ID"; } }

        public int GroupMax{ get { return (int)MessageGroup.Max; } }

		//メッセージクラスのTypeをキャッシュ
		private List<System.Type> messageTypeCache_;
		//パフォーマンス向上のために予めインスタンスを作っておくことも考える
		public List<System.Type> MessageType {
			get {
				return messageTypeCache_;
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BattleMessageSetting() {
			var list = new List<Type>(typeof(BattleMessage).GetNestedTypes());
			list.AddRange(typeof(CommonMessage).GetNestedTypes());
			messageTypeCache_ = list;
		}
	}
}
