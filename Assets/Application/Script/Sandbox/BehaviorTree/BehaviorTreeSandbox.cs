using System.Collections.Generic;
using UnityEngine;
using Project.Lib;
using Project.Game;
using System.Threading;

public class BehaviorTreeSandbox : MonoBehaviour
{
	//Entity集合クラス
	AssemlyManager assemblyManager_ = new AssemlyManager(
		new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
				CharacterAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
	   }
	);

	public GameObject character;
	public CharacterEntity entity;

	private void Start() {
		assemblyManager_.Create(transform);


		UnityEngine.ScriptableObject asset = Resources.Load("behaviorTreeTest") as ScriptableObject;


		BehaviorTree<CharacterEntity>.Initiate(typeof(Project.Game.CharacterEvaluate), typeof(Project.Game.CharacterOrder));



		//entity = CharacterAssembly.I.Create(character);
	}

	// Update is called once per frame
	void Update()
    {
	}
}
