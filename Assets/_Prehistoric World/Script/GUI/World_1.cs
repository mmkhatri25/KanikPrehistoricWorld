using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class World_1 : MonoBehaviour {
	public static World_1 Instance;
	public RectTransform BlockLevel;
	public int howManyBlocks = 3;

	public float step = 720f;
	private bool sliding = false;
	private float smooth = 10f;
	private float newPosX = 0;
	public AudioClip music;

    private void Awake()
    {
		Instance = this;
	}

    void OnEnable(){
		SoundManager.PlayMusic (music);
	}

	void OnDisable(){

	}

	void Show(){
		BlackScreenUI.instance.Hide (0.35f);
	}
	
	// Update is called once per frame
	void Update () {
		if (sliding) {
			float X = Mathf.Lerp (BlockLevel.anchoredPosition.x, newPosX, smooth * Time.deltaTime);
			BlockLevel.anchoredPosition = new Vector2 (X, BlockLevel.anchoredPosition.y);
			if (Mathf.Abs (BlockLevel.anchoredPosition.x - newPosX) < 10) {
				BlockLevel.anchoredPosition = new Vector2 (newPosX, BlockLevel.anchoredPosition.y);
				sliding = false;
			}
		}
	}

	public void Next(){
		if (!sliding) {

			if (newPosX != (-step * (howManyBlocks - 1))) {
				BlackScreenUI.instance.Show (0.35f);
				Invoke ("Show", 0.35f);
			

				newPosX -= step;
				newPosX = Mathf.Clamp (newPosX, -step * (howManyBlocks - 1), 0);
				sliding = true;
				SoundManager.Click ();
			} else {
				BlackScreenUI.instance.Show (0.35f);
				Invoke ("Show", 0.35f);

				newPosX = 0;
				newPosX = Mathf.Clamp (newPosX, -step * (howManyBlocks - 1), 0);
				sliding = true;
				SoundManager.Click ();
			}
		}
	}

	public void Pre(){
		if (!sliding) {
			if (newPosX != 0) {
				BlackScreenUI.instance.Show (0.35f);
				Invoke ("Show", 0.35f);

				newPosX += step;
				newPosX = Mathf.Clamp (newPosX, -step * (howManyBlocks - 1), 0);
				sliding = true;
				SoundManager.Click ();
			} else {
				BlackScreenUI.instance.Show (0.35f);
				Invoke ("Show", 0.35f);

				newPosX = -999999;
				newPosX = Mathf.Clamp (newPosX, -step * (howManyBlocks - 1), 0);
				sliding = true;
				SoundManager.Click ();
			}
		}
	}

	public void UnlockAllLevels(){
		GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);
		UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		SoundManager.Click ();
	}

	public void AvailableLevelGroup(Level level)
    {
		for(int i = 1; i < level.group; i++)
        {
			newPosX -= step;
			newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
		}
		//BlackScreenUI.instance.Show(0.35f);
		//Invoke("Show", 0.35f);
		sliding = true;
	}
}