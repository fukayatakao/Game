using System.Collections.Generic;
using Project.Http.Mst;
using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 分隊の基本パラメータ管理
	/// </summary>
	public class SquadParam : MonoPretender {
		[SerializeField]
		private TotalParam total_ = new TotalParam();
		[SerializeField]
		private PhaseParam phase_ = new PhaseParam();
		[SerializeField]
		private DecayParam decay_ = new DecayParam();

		//最大HP・現在HP
		public float MaxHp { get { return total_.MaxHp; } }
		public float Hp { get { return total_.Hp; } }

		//最大LP・現在LP
		public float MaxLp { get { return total_.MaxLp; } }
		public float Lp { get { return total_.Lp; } }

		//分隊の組織崩壊値
		public float Decay { get { return decay_.Decay; } }

		//属性値
		public int GetPhaseCount(PHASE phase) {
			return phase_.GetPhaseCount(phase);
		}

		//最大員数・現在員数
		private int maxNumber_;
		private int number_;
		//リーダーがいるか
		private bool isLeader_;
		/// <summary>
		/// Total値のセットアップ
		/// </summary>
		public void Setup(SquadEntity owner) {
			maxNumber_ = owner.Members.Count;
			number_ = maxNumber_;
			isLeader_ = owner.Leader != null;
			total_.Apply(owner.Members);
			phase_.Apply(owner.Members);
			decay_.Apply(number_, maxNumber_, isLeader_);
		}

		/// <summary>
		/// 全滅したので数値をクリア
		/// </summary>
		public void Defeat() {
			number_ = 0;
			isLeader_ = false;
			total_.Defeat();
		}
		/// <summary>
		/// 分隊の状況を反映する
		/// </summary>
		public void Apply(SquadEntity owner) {
			number_ = owner.Members.Count;
			isLeader_ = owner.Leader != null;
			total_.Apply(owner.Members);
			phase_.Apply(owner.Members);
			decay_.Apply(number_, maxNumber_, isLeader_);
		}

		/// <summary>
		/// 定期的に状態を記録
		/// </summary>
		public void Execute(SquadEntity owner) {
			total_.Execute(owner.Members);
		}

	}
}
