namespace Project.Mst {
	public partial class MstGoodsData {
		public void Resolve() {
			Resource = new int[] { this.ResourceId1, this.ResourceId2, this.ResourceId3 };
			UseAmount = new int[] { this.UseAmount1, this.UseAmount2, this.UseAmount3 };
		}
		[System.NonSerialized]
		public int[] Resource; //材料消費量
		[System.NonSerialized]
		public int[] UseAmount; //材料消費量
	}
}
