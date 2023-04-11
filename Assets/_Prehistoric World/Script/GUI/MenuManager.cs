using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	public GameObject Startmenu;
	public GameObject GUI;
	public GameObject Controller;
	public GameObject Gameover;
	public GameObject GameFinish;
	public GameObject GamePause;
	public GameObject LoadingScreen;

	public Text levelName;

	[Header("Sound")]
	public Image Music;
	public Image Sound;
	public Color colorOn = Color.white;
	public Color colorOff = Color.blue;

	public GameObject ShopPopup;
	public GameObject ControllerPlatform;

	void Awake()
	{
		Instance = this;
		Startmenu.SetActive(true);
		GUI.SetActive(false);
		Controller.SetActive(false);
		Gameover.SetActive(false);
		GameFinish.SetActive(false);
		GamePause.SetActive(false);
		LoadingScreen.SetActive(false);
		levelName.text = SceneManager.GetActiveScene().name;
		ShowController();
	}

	public void TurnController(bool turnOn)
	{
		Controller.SetActive(turnOn);
	}
	public void TurnGUI(bool turnOn)
	{
		GUI.SetActive(turnOn);
	}

	public void HideController()
	{
		ControllerPlatform.SetActive(false);
	}

	public void ShowController()
	{
		ControllerPlatform.SetActive(true);
	}

	// Use this for initialization
	void Start()
	{
		CheckSoundMusic();
		StartCoroutine(StartGame(1));
	}

	void Update()
	{
		CheckSoundMusic();
	}

	public void TurnMusic()
	{
		if (SoundManager.Instance)
		{
			GlobalValue.isMusic = !GlobalValue.isMusic;
			Music.color = GlobalValue.isMusic ? colorOn : colorOff;
			SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
			SoundManager.Click();
		}
	}
	public void TurnSound()
	{
		GlobalValue.isSound = !GlobalValue.isSound;
		Sound.color = GlobalValue.isSound ? colorOn : colorOff;
		SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
		SoundManager.Click();
	}

	private void CheckSoundMusic()
	{
		if (SoundManager.Instance)
		{
			Music.color = GlobalValue.isMusic ? colorOn : colorOff;
			SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;

			Sound.color = GlobalValue.isSound ? colorOn : colorOff;
			SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
		}
	}

	public void NextLevel()
	{
		GoNextLevel();
	}

	private void GoNextLevel()
	{
		SoundManager.Click();
		Time.timeScale = 1;
		SoundManager.PlaySfx(SoundManager.Instance.soundClick);

		if (LoadingScreen)
			LoadingScreen.SetActive(true);

		if (SceneManager.GetActiveScene().name == "Tutorial")
		{
			StartCoroutine(LoadAsynchronously("MainMenu"));
			return;
		}

		//GameManager.Instance.UnlockNextLevel();
		GlobalValue.levelPlaying += 1;

		if (GameMode.Instance && GlobalValue.levelPlaying > GameMode.Instance.totalLevel)
		{
			Debug.LogError("Exceed the levels, go to HOME SCENE! Set the total level in the GameMode object in the Logo scene");
			HomeScene();
		}
		else
			StartCoroutine(LoadAsynchronously("Playing"));
	}

	public void RestartGame()
	{
		SoundManager.Click();
		Time.timeScale = 1;
		SoundManager.PlaySfx(SoundManager.Instance.soundClick);
		if (LoadingScreen)
			LoadingScreen.SetActive(true);
		StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
	}

	public void HomeScene()
	{
		SoundManager.Click();
		SoundManager.PlaySfx(SoundManager.Instance.soundClick);
		Time.timeScale = 1;
		if (LoadingScreen)
			LoadingScreen.SetActive(true);
		StartCoroutine(LoadAsynchronously("MainMenu"));

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
			progressText.text = progress * 100f + "%";
			yield return null;
		}
	}

	public void Gamefinish()
	{
		StartCoroutine(GamefinishCo(2));
		Controller.SetActive(false);
	}

	public void GameOver()
	{
		StartCoroutine(GameOverCo(1));
		Controller.SetActive(false);
	}

	public void Pause()
	{
		SoundManager.Click();
		if (Time.timeScale == 0)
		{
			SoundManager.PlaySfx(SoundManager.Instance.soundPause);
			GamePause.SetActive(false);
			GUI.SetActive(true);
			Time.timeScale = 1;
			GameManager.Instance.State = GameManager.GameState.Playing;
		}
		else
		{
			GamePause.SetActive(true);
			GUI.SetActive(false);
			Time.timeScale = 0;
			GameManager.Instance.State = GameManager.GameState.Pause;
		}
	}

	public void ExitGame()
	{
		SoundManager.Click();
		SceneManager.LoadScene("MainMenu");
	}

	public void Continues()
	{
		SoundManager.Click();
		Startmenu.SetActive(false);
		GUI.SetActive(true);
		Controller.SetActive(true);
	}

	IEnumerator StartGame(float time)
	{
		yield return new WaitForSeconds(time - 0.5f);

		yield return new WaitForSeconds(0.5f);
		Startmenu.SetActive(false);
		//GameManager.Instance.Player.ShowUpAnim();
		yield return new WaitForSeconds(0.5f);

		yield return null;

		Startmenu.SetActive(false);
		GUI.SetActive(true);
		Controller.SetActive(true);

		GameManager.Instance.StartGame();
	}

	IEnumerator GamefinishCo(float time)
	{
		GUI.SetActive(false);

		yield return new WaitForSeconds(time);

		GameFinish.SetActive(true);
	}

	IEnumerator GameOverCo(float time)
	{
		GUI.SetActive(false);

		yield return new WaitForSeconds(time);

		Gameover.SetActive(true);
	}

	void OnDestroy()
	{
		Time.timeScale = 1;
	}
	public GameObject characterHolder;
	public void OpenShop()
	{
		GameManager.Instance.Player.StopMove();

		if (CharacterHolder.Instance == null)
			Instantiate(characterHolder);

		SoundManager.Click();
		if (ShopMenuPopupUI.instance)
		{
			ShopMenuPopupUI.instance.gameObject.SetActive(true);
			ShopMenuPopupUI.Show();

		}
		else
		{
			Instantiate(ShopPopup);
		}
	}

	public void ShowRewardVideo()
	{
		ShowRewardedAd();
	}

	//	#if UNITY_ADS
	private void ShowRewardedAd()
	{
		if (!GlobalValue.allowClickUnityAdAgain)
			return;

		if (AdsManager.Instance.isRewardedAdReady())
		{
			GlobalValue.allowClickUnityAdAgain = false;
			AdsManager.AdResult += AdsManager_AdResult;
			AdsManager.Instance.ShowRewardedAds();
		}
	}

	public void ShowNormalVideo()
	{
		if (GlobalValue.RemoveAds)
		{
			GoNextLevel();
			return;
		}

		if (!GlobalValue.allowClickUnityAdAgain)
		{
			Debug.LogError("allowClickUnityAdAgain = " + GlobalValue.allowClickUnityAdAgain);
			return;
		}

		if (AdsManager.Instance.isRewardedAdReady())
		{
			GlobalValue.allowClickUnityAdAgain = false;
			AdsManager.AdResult += AdsManager_AdResult;
			AdsManager.Instance.ShowRewardedAds();
		}
	}

	private void AdsManager_AdResult(bool isSuccess, int rewarded)
	{
		AdsManager.AdResult -= AdsManager_AdResult;
		GlobalValue.allowClickUnityAdAgain = true;
		if (isSuccess)
			GoNextLevel();
		else
			Debug.LogError("Message: ADS FAIL");
	}
}
