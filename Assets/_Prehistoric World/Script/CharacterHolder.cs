using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterHolder : MonoBehaviour, IListener
{
	public static CharacterHolder Instance;
	[HideInInspector]
	public GameObject CharacterPicked;

	public List<GameObject> Characters;

	public List<int> CharacterUnlocked { get; set; }
	int originalPlayerID = int.MaxValue;    //mean if Player eat item can switch player, then eat another item switch player, 
											//keep the original player ID, when out of time use another player -> Back to the original player

	void Awake()
	{
		if (CharacterHolder.Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		GetPickedCharacter();

		UpdateUnlockCharacter();
	}

	void Update()
	{
		GetPickedCharacter();
		//Debug.LogError(CharacterHolder.Instance.CharacterPicked.name);
	}

	public void GetPickedCharacter()
	{
		CharacterPicked = Characters[0];    //default character is 0
		var characterIDChoosen = PlayerPrefs.GetInt(GlobalValue.ChoosenCharacterInstanceID, 0);
		foreach (var character in Characters)
		{
			var ID = character.GetInstanceID();
			if (ID == characterIDChoosen)
			{
				CharacterPicked = character;
				return;
			}
		}
	}

	public void UpdateUnlockCharacter()
	{
		CharacterUnlocked = new List<int>();

		for (int i = 0; i < Characters.Count; i++)
		{
			if (GlobalValue.CheckUnlockCharacter(i + 1) || (i == 0))
				CharacterUnlocked.Add(Characters[i].GetInstanceID());
		}

		Debug.Log("Totol Player Available: " + CharacterUnlocked.Count);
	}

	public bool isThisCharIdUnlock(int id)
	{
		for (int i = 0; i < CharacterUnlocked.Count; i++)
		{
			if (CharacterUnlocked[i] == Characters[id].GetInstanceID())
			{
				return true;
			}
		}
		return false;
	}

	public void SwitchPlayer()
	{
		int currentPos = 0;
		var characterIDChoosen = PlayerPrefs.GetInt(GlobalValue.ChoosenCharacterInstanceID, 0);
		for (int i = 0; i < CharacterUnlocked.Count; i++)
		{
			if (CharacterUnlocked[i] == characterIDChoosen)
			{
				currentPos = i;
				break;
			}
		}

		Debug.Log(currentPos);

		currentPos++;
		if (currentPos >= CharacterUnlocked.Count)
			currentPos = 0;

		var nextCharacter = CharacterUnlocked[currentPos];
		foreach (var character in Characters)
		{
			var ID = character.GetInstanceID();
			if (ID == nextCharacter)
			{
				CharacterPicked = character;
				break;
			}
		}

		PlayerPrefs.SetInt(GlobalValue.ChoosenCharacterID, CharacterPicked.GetComponent<Player>().ID);
		//Debug.LogError("SET PLAYER: " + CharacterPicked.name);
		PlayerPrefs.SetInt(GlobalValue.ChoosenCharacterInstanceID, CharacterPicked.GetInstanceID());

		//		originalPlayerID = CharacterPicked.GetComponent<Player> ().ID;
		//		PlayerPrefs.SetInt (GlobalValue.ChoosenCharacterID, CharacterPicked.GetComponent<);
		//		CharacterHolder.Instance.CharacterPicked = CharacterPrefab;
		if (GameManager.Instance)
			GameManager.Instance.SwitchPlayerCharacter();

		Debug.Log(currentPos);
	}

	[HideInInspector]
	public bool isWaitingForBackMainPlayer = false;
	IEnumerator waitTimeBack;

	public void WaitTimeBackToMainPlayer(float delay, int playerID)
	{
		if (isWaitingForBackMainPlayer)
		{
			//stop waiting if before is actived
			StopCoroutine(waitTimeBack);
		}

		if (originalPlayerID == int.MaxValue)
			originalPlayerID = playerID;


		waitTimeBack = WaitTimeBackToMainPlayerCo(delay);
		StartCoroutine(waitTimeBack);
	}


	IEnumerator WaitTimeBackToMainPlayerCo(float delay)
	{
		isWaitingForBackMainPlayer = true;
		yield return new WaitForSeconds(delay);

		while (GameManager.Instance.State != GameManager.GameState.Playing)
		{
			yield return null;  //wait until game playing again
		}

		SwichBackToOriginalPlayer();

		BlackScreenUI.instance.Show(0.3f, Color.white);
		yield return new WaitForSeconds(0.1f);
		BlackScreenUI.instance.Hide(0.3f);

		//SwitchBackWeapon();
	}

	public void BackToMainPlayer()
	{
		StopCoroutine(waitTimeBack);
		SwichBackToOriginalPlayer();
	}

	string originalWeapon;

	public void SwichBackToOriginalPlayer()
	{
		//if (FindObjectOfType<WeaponChangerUI>() != null && FindObjectOfType<WeaponChangerUI>().currentBullet)
		//	originalWeapon = FindObjectOfType<WeaponChangerUI>().currentBullet.name;

		CharacterHolder.Instance.CharacterPicked = CharacterHolder.Instance.Characters[originalPlayerID];

		originalPlayerID = int.MaxValue;        //reset original player
		isWaitingForBackMainPlayer = false;

		if (GameManager.Instance)
			GameManager.Instance.SwitchPlayerCharacter();
	}

	//public void SwitchBackWeapon()
	//{
	//	WeaponChangerUI weaponChanger = FindObjectOfType<WeaponChangerUI>();
	//	if (weaponChanger)
	//	{
	//		weaponChanger.SwithWeapon(originalWeapon);
	//	}
	//}

	#region IListener implementation

	public void IPlay()
	{

	}

	public void ISuccess()
	{
		if (isWaitingForBackMainPlayer)
		{
			//stop waiting if before is actived
			StopCoroutine(waitTimeBack);

			CharacterHolder.Instance.CharacterPicked = CharacterHolder.Instance.Characters[originalPlayerID];
			originalPlayerID = int.MaxValue;        //reset original player
			isWaitingForBackMainPlayer = false;
		}
	}

	public void IPause()
	{

	}

	public void IUnPause()
	{

	}

	public void IGameOver()
	{
		if (isWaitingForBackMainPlayer)
		{
			//stop waiting if before is actived
			StopCoroutine(waitTimeBack);

			CharacterHolder.Instance.CharacterPicked = CharacterHolder.Instance.Characters[originalPlayerID];
			originalPlayerID = int.MaxValue;        //reset original player
			isWaitingForBackMainPlayer = false;
		}
	}

	public void IOnRespawn()
	{

	}

	public void IOnStopMovingOn()
	{

	}

	public void IOnStopMovingOff()
	{

	}

	#endregion
}
