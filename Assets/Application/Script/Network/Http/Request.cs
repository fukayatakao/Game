namespace Project.Network {
	public class Request {
		public long _tm_;
	}


	public class Response {
		public const int RES_CODE_SUCCESS = 0;
		//---- レスポンス変数定義 ----
		public int res_code;
		public long server_time;
		public float client_wait;
		//---- コンストラクタ ----
		public Response() { }
	}


}
