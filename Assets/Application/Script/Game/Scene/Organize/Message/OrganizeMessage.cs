using System;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	public static partial class OrganizeMessage {
	}
	/// <summary>
	/// オブジェクト間処理の仲介
	/// </summary>
	public sealed class OrganizeMessageSetting : IMessageSetting {
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
		public OrganizeMessageSetting() {
			var list = new List<Type>(typeof(OrganizeMessage).GetNestedTypes());
			list.AddRange(typeof(CommonMessage).GetNestedTypes());
			list.AddRange(typeof(BattleMessage).GetNestedTypes());
			messageTypeCache_ = list;
		}
	}
}
