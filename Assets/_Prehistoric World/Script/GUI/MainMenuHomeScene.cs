using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHomeScene : MonoBehaviour {
	public static MainMenuHomeScene Instance;
	public GameObject Levels;
	public GameObject Loading;
    public GameObject GetMoreCoin;
	public GameObject ShopUI;

	public GameObject RemoveAdBut;
	[Header("Sound")]
	public Image Music;
	public Image Sound;
	public Color colorOn = Color.white;
	public Color colorOff = Color.blue;

	void Awake(){
		Instance = this;
		if (Loading != null)
			Loading.SetActive (false);
		if (Levels != null)
			Levels.SetActive (false);
        if (GetMoreCoin)
            GetMoreCoin.SetActive(false);
		if (ShopUI)
			ShopUI.SetActive(false);
	}

	public void LoadScene(string name){
		if (Loading != null)
			Loading.SetActive (true);
		Levels.SetActive(false);
		//SceneManager.LoadSceneAsync (name);
		StartCoroutine(LoadAsynchronously(name));
    }

	public void LoadPlayingScene()
    {
		if (Loading != null)
			Loading.SetActive(true);
		Levels.SetActive(false);
		StartCoroutine(LoadAsynchronously("Level " + GlobalValue.levelPlaying));
	}


	public void OpenGetMoreCoin(bool open)
    {
        SoundManager.Instance.ClickBut();
        GetMoreCoin.SetActive(open);
    }

	// Use this for initialization
	IEnumerator Start()
	{
		CheckSoundMusic();

		if (GlobalValue.isFirstOpenMainMenu)
		{
			GlobalValue.isFirstOpenMainMenu = false;
			SoundManager.Instance.PauseMusic(true);
			SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);
			if (SoundManager.Instance.beginSoundInMainMenu != null)
				yield return new WaitForSeconds(SoundManager.Instance.beginSoundInMainMenu.length);
			SoundManager.Instance.PauseMusic(false);
			SoundManager.ResetMusic();
			//SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
		}
	}

	void Update(){
		CheckSoundMusic();
		if (RemoveAdBut)
			RemoveAdBut.SetActive((GlobalValue.RemoveAds ? false : true));
	}

	public void NewGame(){
		Levels.SetActive(true);
	}

	public void TurnMusic(){
		GlobalValue.isMusic = !GlobalValue.isMusic;
		Music.color = GlobalValue.isMusic ? colorOn : colorOff;
		if (SoundManager.Instance) {
			SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
			SoundManager.Click ();
		}
	}
	public void TurnSound(){
		GlobalValue.isSound = !GlobalValue.isSound;
		Sound.color = GlobalValue.isSound ? colorOn : colorOff;
		SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
		SoundManager.Click ();
	}

	private void CheckSoundMusic(){
		if (SoundManager.Instance) {
			Debug.Log ("Checked");
			Music.color = GlobalValue.isMusic ? colorOn : colorOff;
			SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;

			Sound.color = GlobalValue.isSound ? colorOn : colorOff;
			SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
		}
	}

	public void OpenShop(bool show){
		SoundManager.Click ();
		ShopUI.SetActive(show);
	}
	public void Tutorial(){
		SoundManager.Click ();
		SceneManager.LoadScene ("Level ALL IN ONE");
	}
	public void Exit(){
		SoundManager.Click ();
		Application.Quit ();
	}

    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = (int)( progress * 100f )+ "%";
            //			Debug.LogError (progress);
            yield return null;
        }
    }
}
