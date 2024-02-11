using Project.Lib;


namespace Project.Game {
    public static partial class BattleMessage{
	    /// <summary>
	    /// 死亡したユニットを管理から削除する
	    /// </summary>
	    public static class DefeatUnit {
		    //メッセージ種別のID
		    private static int ID = -1;
		    //送付するデータ内容
		    private class Data {
			    public CharacterEntity entity;
		    }

		    /// <summary>
		    /// メッセージを送る
		    /// </summary>
		    public static void Broadcast(CharacterEntity e) {
			    MessageSystem.Broadcast(
				    new MessageObject(ID, new Data { entity = e }),
                    (int)MessageGroup.GameEvent
                );
		    }

			/// <summary>
			/// 死亡したキャラを登録している場合はクリア
			/// </summary>
			private static void Recv(CharacterEntity entity, MessageObject msg) {
				Data data = (Data)msg.Data;

				//一番近い敵として登録されていたらクリアする
				if(entity.HaveBlackboard.EnemyBoard.TargetEnemy == data.entity) {
					entity.TargetDeploy();
					entity.HaveBlackboard.EnemyBoard.SetNearEnemy(null);
					entity.HaveBlackboard.EnemyBoard.SetLockonEnemy(null);
				}
			}

#if DEVELOP_BUILD
			/// <summary>
			/// 死亡したキャラを登録している場合はクリア
			/// </summary>
			private static void Recv(CharacterInfo info, MessageObject msg) {
				Data data = (Data)msg.Data;

				info.Defeat(data.entity);
			}
#endif
	    }
	}

}
