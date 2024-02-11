using System.Collections.Generic;

namespace Project.Game {

	[System.Serializable]
	public class SlotStatus {
		public int Amount;
		public float Grade;
	}

	[System.Serializable]
	public class Slot {
		public Slot(Dictionary<int, SlotStatus> dict) {
			slotDict = dict;
		}

		private Dictionary<int, SlotStatus> slotDict;
		public SlotStatus Status { get { return slotDict[goodsId_]; } }
		public bool IsEnabe() { return goodsId_ != 0; }
		//グッズID
		int goodsId_;
		public int GoodsId {
			get { return goodsId_; }
			set {
				//登録削除
				if (value == 0) {
					slotDict.Remove(goodsId_);
					goodsId_ = value;
				} else {
					//既に他のスロットで管理している場合は無視（もしくはこちらに移動させる）
					if (slotDict.ContainsKey(value))
						return;
					//新規登録
					if (goodsId_ == 0) {
						goodsId_ = value;
						slotDict[goodsId_] = new SlotStatus();
						//登録変更
					} else {
						slotDict.Remove(goodsId_);
						goodsId_ = value;
						slotDict[goodsId_] = new SlotStatus();
					}
				}
			}
		}
		//貯蔵量
		public int Amount {
			get {
				if (goodsId_ == 0)
					return 0;
				return slotDict[goodsId_].Amount;
			}
			set {
				if (goodsId_ == 0)
					return;
				slotDict[goodsId_].Amount = value;
			}
		}

		//グレード
		public float Grade {
			get {
				if (goodsId_ == 0)
					return 0;
				return slotDict[goodsId_].Grade;
			}
			set {
				if (goodsId_ == 0)
					return;
				slotDict[goodsId_].Grade = value;
			}
		}
	}
}
