namespace Project.Game {
    /// <summary>
    /// 無操作カメラ
    /// </summary>
	public class NoneControl : ICameraControl {
        /// <summary>
        /// 前のカメラの設定を引き継ぐ
        /// </summary>
        public void Change(ICameraControl control) {
            position_ = control.Position;
            rotation_ = control.Rotation;
        }
        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Execute(CameraControlSetting setting) {
		}
	}

}
