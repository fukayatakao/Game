using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	public class StorageParam : MonoPretender{
		public Slot[] Slots;
		public Dictionary<int, SlotStatus> SlotDict;

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup(int capcity) {
			SlotDict = new Dictionary<int, SlotStatus>();
			Slots = new Slot[capcity];
			for (int i = 0, max = Slots.Length; i < max; i++) {
				Slots[i] = new Slot(SlotDict);
			}
		}
	}
}
