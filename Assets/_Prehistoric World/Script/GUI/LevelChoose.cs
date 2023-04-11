using UnityEngine;
using System.Collections;

public class LevelChoose : MonoBehaviour {

	public GameObject[] Levels;

	int numberBlock;
	int currentBlock;

	void OnEnable(){
		currentBlock = 1;
		SoundManager.Click ();
	}

	void OnDisable(){
		SoundManager.Click ();
	}

	// Use this for initialization
	void Start () {
		numberBlock = Levels.Length;
	}

	public void Next(){
		currentBlock++;
		TurnOnBlock ();
	}

	public void Previous(){
		currentBlock--;
		TurnOnBlock ();
	}

	private void TurnOnBlock(){
		SoundManager.Click ();

		foreach (var level in Levels) {
			level.SetActive (false);
		}

		currentBlock = Mathf.Clamp (currentBlock, 1, numberBlock);
		Levels [currentBlock-1].SetActive (true);
	}
}
