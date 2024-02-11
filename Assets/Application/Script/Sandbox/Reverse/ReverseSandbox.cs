#if false

public class ReverseSandbox : MonoBehaviour
{
	Dictionary<float, string> RecordAnimation = new Dictionary<float, string>();
	[System.Serializable]
	class Record {
		public string currentAnim;
		public float currentTime;

		public string prevAnim;
		public float prevTime;
		public float fadeTime;
		public float accumulateTime;
	}
	[SerializeField]
	List<Record> record_;
	CharacterEntity character;
	CameraEntity camera_;
	bool loop = true;
	bool isReady = false;
	private UserOperation userOperation_;

	CoroutineTaskList taskList_ = new CoroutineTaskList();
	/// <summary>
	/// 初期化
	/// </summary>
	void Awake()
    {
		record_ = new List<Record>();
		loop = true;
		isReady = false;
		Gesture.Create();
		VirtualScreen.Create();
		MessageSystem.Create(new BattleMessageSetting());
		//初めにインスタンス作らないとメッセージをスルーされる
		userOperation_ = new UserOperation((int)MessageGroup.TouchControl);
		AddressableAssistOld.LoadCategoriesAsync(new List<string>() { "Animation-Normal" } , (s) => { loop = false; });

		// 目標フレームレート
		ManipulateTime.Setup();

		Setup();
    }

	async void Setup() {
		while (loop) {
			await System.Threading.Tasks.Task.Delay(100);
		}
		CharacterAssembly.CreateInstance(transform);
		CameraAssembly.CreateInstance(transform);
		LightAssembly.CreateInstance(transform);


		LightAssembly.I.Create(ResourcesPath.LIGHT_DIRECTIONAL);
		camera_ = CameraAssembly.I.Create(ResourcesPath.VIEW_CAMERA_PREFAB);
		camera_.ChangeControlEditorView(true);

		character = CharacterAssembly.I.Create("Character/Model/Yuko_sum_humanoid", "Animation-Normal");

		//character.HaveAnimation.Play(Project.Game.BattleMotion.Attack);



		//デフォルト制御を登録
		userOperation_.SetDefault((int)OperationPriority.CameraEditorViewing);

		isReady = true;
	}


	// Update is called once per frame
	void Update()
    {
		if (!isReady)
			return;

		if (ManipulateTime.IsRecord()) {
			if (Input.GetKeyDown(KeyCode.A)) {
				BuildTask();
			}
			taskList_.Execute();
		}else if (ManipulateTime.IsReplay()) {

		}

		userOperation_.Execute();
		CameraAssembly.I.Execute();
		CharacterAssembly.I.Execute();


		ManipulateTime.DebugKey();
	}

	private void LateUpdate() {
		if (!isReady)
			return;
		CameraAssembly.I.LateExecute();
		CharacterAssembly.I.LateExecute();

		ManipulateTime.Execute();
	}

	private void BuildTask() {
		taskList_.Clear();
		taskList_.Add(PlayAnimation(BattleMotion.GoDown));
		taskList_.Add(WaitAnimationEnd());
		taskList_.Add(PlayAnimation(BattleMotion.DownToUp));
		taskList_.Add(WaitAnimationEnd());

	}

	IEnumerator PlayAnimation(Project.Game.BattleMotion motion) {
		character.HaveAnimation.Play(motion);
		yield break;
	}
	IEnumerator WaitAnimationEnd() {
		while (!character.HaveAnimation.IsEnd()) {
			yield return null;
		}

		yield break;
	}

}
#endif
