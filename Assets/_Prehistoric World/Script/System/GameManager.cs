/// <summary>
/// Game manager. 
/// Handle all the actions, parameter of the game
/// You can easy get the state of the game with the IListener script.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[HideInInspector] public bool isWatchingAd = false;
	public enum GameState { Menu, Playing, GameOver, Success, Pause, Waiting };
	[ReadOnly] public GameState State;
	public bool isStopTimerActivating { get; set; }
	public bool isUsingShield { get; set; }
	[Header("Floating Text")]
	public GameObject FloatingText;
	private MenuManager menuManager;
	[HideInInspector] public ActionButtonElevatorUI actionButtonElevatorUI;

	public bool isPlayerStandOnElevator()
	{
		if (actionButtonElevatorUI == null)
		{
			actionButtonElevatorUI = FindObjectOfType<ActionButtonElevatorUI>();
		}

		return actionButtonElevatorUI.currentElevator != null;
	}

	public Player Player { get; private set; }
	SoundManager soundManager;

	public ControllerInput controllerInput { get; set; }

	public Transform currentCheckpoint { get; set; }
	public int checkpointDir { set; get; }
	[HideInInspector] public Vector3 currentPlayerPos;

	[HideInInspector]
	public bool isNoLives = false;

	public bool isSpecialBullet { get; set; }
	public bool isInDialogue { get; set; }
	public bool isHasKey { get; set; }

	[HideInInspector]
	public List<IListener> listeners;

	GameObject clonePlayer;
	[HideInInspector] public GameObject partnerTempItem;

	void Awake()
	{
		Application.targetFrameRate = 60;

		isSpecialBullet = false;
		Instance = this;
		State = GameState.Menu;
		Player = FindObjectOfType<Player>();
		//playerStartPosition = Player.transform.position;
		listeners = new List<IListener>();

		//if (FindObjectOfType<BigStar>() == null)
		GlobalValue.ResetBigStars();

		InitPlayer();

		currentPlayerPos = Player.transform.position;
		checkpointDir = Player.transform.localScale.x > 0 ? 1 : -1;

		clonePlayer = Instantiate(Player.gameObject);
		clonePlayer.SetActive(false);

	}

	void InitPlayer()
	{
		if (CharacterHolder.Instance != null && CharacterHolder.Instance.CharacterPicked != null)
		{
			GameObject _player = Instantiate(CharacterHolder.Instance.CharacterPicked, Player.transform.position, Player.transform.rotation) as GameObject;
			bool isWater = Player.PlayerState == PlayerState.Water;


			Destroy(Player.gameObject);
			bool isfaceingRight = Player.transform;

			Player = _player.GetComponent<Player>();
			if (isWater)
			{
				Player.PlayerState = PlayerState.Water;
				Player.SetupParameter();
			}

			isInDialogue = false;
		}
	}

	void AllowPlayerMoving()
	{
		Player.isPlaying = true;
	}


	public bool isSwitchToOtherPlayer { get; set; }
	bool isPlayerFacingRight = false;

	bool fromPartner = false;
	public void SwitchPlayerCharacter()
	{
		isSwitchToOtherPlayer = true;
		isPlayerFacingRight = Player.isFacingRight;
		Debug.Log("Switch Player");
		listeners.Remove(Player.GetComponent<IListener>());

		InitPlayer();
		Invoke("AllowPlayerMoving", 0.05f);

		if (!isPlayerFacingRight)
		{
			Player.Flip();
		}
		//Get all objects that have IListener
		listeners.Add(Player.GetComponent<IListener>());
	}

	public int Coin { get; set; }

	public void AddCoin(int addcoin, Transform position)
	{
		Coin += addcoin;
		ShowFloatingText("+" + addcoin, position.position, Color.yellow);
	}

	public void AddNormalBullet(int addbullet, Transform position)
	{
		GlobalValue.normalBullet += addbullet;
		ShowFloatingText("Bullet+", position.position, Color.white);
	}

	IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();

		menuManager = FindObjectOfType<MenuManager>();

		soundManager = FindObjectOfType<SoundManager>();

		State = GameState.Playing;
	}

	public void ShowFloatingText(string text, Vector2 positon, Color color)
	{
		if (GameManager.Instance.State != GameState.Playing)
			return;

		GameObject floatingText = Instantiate(FloatingText) as GameObject;
		var _position = Camera.main.WorldToScreenPoint(positon);

		floatingText.transform.SetParent(menuManager.transform, false);
		floatingText.transform.position = _position;

		var _FloatingText = floatingText.GetComponent<FloatingText>();
		_FloatingText.SetText(text, color);
	}

	public void StartGame()
	{
		State = GameState.Playing;
		LevelManager.Instance.StartGame();
		Player.isPlaying = true;

		var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
		foreach (var _listener in listener_)
		{
			if (listeners.Contains(_listener) == false)
				listeners.Add(_listener);
		}

		foreach (var item in listeners)
		{
			item.IPlay();
		}
	}

	public void AddListener(IListener obj)
    {
		if (listeners.Contains(obj) == false)
			listeners.Add(obj);
    }

	public void RemoveListener(IListener obj)
    {
		if (listeners.Contains(obj))
			listeners.Remove(obj);
	}

	public void Gamepause()
	{
		State = GameState.Pause;
		foreach (var item in listeners)
			item.IPause();
	}

	public void UnPause()
	{
		State = GameState.Playing;
		foreach (var item in listeners)
			item.IUnPause();
	}

	public void ActiveStopTimer(bool active)
	{
		isStopTimerActivating = active;
	}

	public void GameFinish()
	{
		State = GameState.Success;

		if (GlobalValue.bigStar1)
			//GlobalValue.SetStarPosition(1);
			GlobalValue.SetScrollLevelAte(1);
		if (GlobalValue.bigStar2)
			GlobalValue.SetScrollLevelAte(2);
		if (GlobalValue.bigStar3)
			GlobalValue.SetScrollLevelAte(3);

		foreach (var item in listeners)
		{
			if (item != null)
				item.ISuccess();
		}

		StartCoroutine(GameFinishCo());
	}

	IEnumerator GameFinishCo()
	{
		yield return new WaitForSeconds(1);
		MenuManager.Instance.Gamefinish();
		SoundManager.PlaySfx(soundManager.soundGamefinish, 0.5f);

		//save coins and points
		GlobalValue.SavedCoins += Coin;

		//save level and save star
		GlobalValue.UnlockLevel(SceneManager.GetActiveScene().name);

		//unlock next level
		if (GlobalValue.levelPlaying > GlobalValue.LevelPass)
			GlobalValue.LevelPass = GlobalValue.levelPlaying;
	}

	public void GameOver()
	{
		Debug.Log("GAMEOVER");
		if (State == GameState.GameOver)
			return;

		State = GameState.GameOver;
		if (GlobalValue.levelPlaying != -1)		//don't reduce live when testing level
			GlobalValue.SavedLives--;

		if (GlobalValue.SavedLives >= 1)
		{
			if (AdsManager.Instance)
				AdsManager.Instance.ShowNormalAd(GameState.GameOver);
		}

		foreach (var item in listeners)
			item.IGameOver();

		if (GlobalValue.SavedLives <= 0)
		{
			MenuManager.Instance.GameOver();
			SoundManager.PlaySfx(soundManager.soundGameover, 0.5f);

			isNoLives = true;

		}
		else
		{
			StartCoroutine(GotoCheckPointCo(1.5f));
		}
	}

	public void Continues(bool resetGame = false)
	{
			StartCoroutine(GotoCheckPointCo(0.5f));
			MenuManager.Instance.Continues();
	}

	public void ResetValue()
	{
		GlobalValue.SavedLives = 1;
	}

	//go to the last checkpoint after dead
	IEnumerator GotoCheckPointCo(float time)
	{
		MenuManager.Instance.HideController();

		yield return new WaitForSeconds(time);

		State = GameState.Playing;


		Destroy(Player.gameObject);

		var _Player = Instantiate(clonePlayer);
		Player = _Player.GetComponent<Player>();
		//Player.RespawnAt (currentCheckpoint != null ? currentCheckpoint.position : currentPlayerPos, checkpointDir);
		Player.transform.position = currentCheckpoint != null ? (currentCheckpoint.position + Vector3.up ): currentPlayerPos;
		Player.gameObject.SetActive(true);
		Player.isPlaying = true;

		ControllerInput.Instance.StopMove();

		foreach (var item in listeners)
			item.IOnRespawn();

		MenuManager.Instance.ShowController();


		Player.DoBlinking();

	}

	public Vector2 lastJumpPos { get; set; }
}