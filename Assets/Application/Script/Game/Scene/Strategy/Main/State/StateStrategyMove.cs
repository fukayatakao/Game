using UnityEngine;
using System.Collections;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateStrategyMove : IState<StrategyMain> {
		CoroutineTaskList taskList_ = new CoroutineTaskList();
		MessageSystem.Receptor receptor_;

		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
		}

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(StrategyMain owner) {
			Gesture.Enable();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
			Gesture.Disable();
        }
        /// <summary>
        /// 更新
        /// </summary>
		public override void Execute(StrategyMain owner) {
			taskList_.Execute();
			//一番最初に制御チェックを行う
			owner.ExecUserContorl();
		}

		public void BuildTask(StrategyBoard board, Geopolitics.Way pair) {
			FieldWayMap map = board.Field.HaveWaymap;

			StrategyMessage.StartPieceMove.Broadcast();

			PieceEntity actor = board.Invader[pair.startNodeId];
			actor.Move(pair);

			//進む先のノードにユニットがいるか検査
			int nextNode = pair.goalNodeId;
			board.MovePiece(pair.startNodeId, nextNode);

			//防衛部隊がいる場合は移動後にバトルに入る
			if (board.Defender.TryGetValue(nextNode, out var target)) {
				taskList_.Add(TargetMove(actor, target, nextNode));
			} else {
				taskList_.Add(TargetNodeMove(actor, nextNode));
			}
		}

		static IEnumerator TargetNodeMove(PieceEntity actor, int nodeId) {
			//近くに来るまで待つ
			while (actor.HaveState.IsCurrentState(PieceState.State.Move)) {
				yield return null;
			}

			StrategyMessage.EndPieceMove.Broadcast();
		}


		static IEnumerator TargetMove(PieceEntity actor, PieceEntity target, int nodeId) {
			//近くに来るまで待つ
			while ((actor.GetPosition() - target.GetPosition()).sqrMagnitude > 3f * 3f) {
				yield return null;
			}
			//モーションをちょっと入れてバトルに突入
			actor.ChangeState(PieceState.State.None);
			target.ChangeState(PieceState.State.None);
			actor.HaveModel.HaveAnimation.Play(BattleMotion.AttackA);
			target.HaveModel.HaveAnimation.Play(BattleMotion.AttackA);

			while (actor.HaveModel.HaveAnimation.IsPlay(BattleMotion.AttackA)) {
				yield return null;
			}

			StrategyMessage.EndPieceMove.Broadcast();
			SceneTransition.ChangeBattle(actor.Id, target.Id, nodeId);

		}

	}
}
