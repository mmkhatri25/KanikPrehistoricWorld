using UnityEngine;
using System.Collections;

public class GlobalValue : MonoBehaviour
{
	public static bool isFirstOpenMainMenu = true;
	public static int worldPlaying = 1;
	public static int levelPlaying = -1;

	public static Vector3 lastCheckPoint;

	public static string WorldReached = "WorldReached";
	public static string Coins = "Coins";
	public static string Lives = "Lives";
	public static string Points = "Points";
	public static string Bullets = "Bullets";
	public static string Character = "Character";
	public static string ChoosenCharacterID = "choosenCharacterID";
	public static string ChoosenCharacterInstanceID = "ChoosenCharacterInstanceID";
	public static GameObject CharacterPrefab;
	public static bool isSound = true;
	public static bool isMusic = true;
	public static bool isRestart = false;
	public static bool isPlayingVideo = false;
	public static bool allowClickUnityAdAgain = true;

	public static bool bigStar1 = false;
	public static Sprite bigStarSprite1;
	public static bool bigStar2 = false;
	public static Sprite bigStarSprite2;
	public static bool bigStar3 = false;
	public static Sprite bigStarSprite3;

	public static void ResetBigStars()
	{
		GlobalValue.bigStar1 = false;
		GlobalValue.bigStarSprite1 = null;
		GlobalValue.bigStar2 = false;
		GlobalValue.bigStarSprite2 = null;
		GlobalValue.bigStar3 = false;
		GlobalValue.bigStarSprite3 = null;
	}

	//public static bool GetStarPosition(int World, int level, int starPos)
	//{
	//	return PlayerPrefs.GetInt("World" + World + level + "stars" + starPos, 0) == 1 ? true : false;
	//}

	//public static void SetStarPosition(int starPos)
	//{
	//	PlayerPrefs.SetInt("World" + GlobalValue.worldPlaying + GlobalValue.levelPlaying + "stars" + starPos, 1);
	//}

	public static void EnemyHealthStore(string EnemyID, int Health)
	{
		PlayerPrefs.SetInt("Health" + EnemyID, Health);
	}

	public static int EnemyHealthGet(string EnemyID)
	{
		return PlayerPrefs.GetInt("Health" + EnemyID, 9999);
	}

	public static void AutoSpawnStore(string ID, int current)
	{
		PlayerPrefs.SetInt("AutoSpawnStore" + ID, current);
	}

	public static int AutoSpawnGet(string ID)
	{
		return PlayerPrefs.GetInt("AutoSpawnStore" + ID, 9999);
	}

	public static int SavedLives
	{
		get { return PlayerPrefs.GetInt(GlobalValue.Lives, 10); }
		set { PlayerPrefs.SetInt(GlobalValue.Lives, value); }
	}
	public static int SavedCoins
	{
		get { return PlayerPrefs.GetInt("Coins", 200); }
		set { PlayerPrefs.SetInt("Coins", value); }
	}
	public static int SavedPoints
	{
		get { return PlayerPrefs.GetInt("Points", 0); }
		set { PlayerPrefs.SetInt("Points", value); }
	}

	public static int LevelPass
	{
		get { return PlayerPrefs.GetInt("LevelReached", 0); }
		set { PlayerPrefs.SetInt("LevelReached", value); }
	}

	public static void LevelStar(string level, int stars)
	{
		PlayerPrefs.SetInt("LevelStars" + level, stars);
	}

	public static int LevelStar(string level)
	{
		return PlayerPrefs.GetInt("LevelStars" + level, 0);
	}

	public static void UnlockLevel(string level)
	{
		PlayerPrefs.SetInt("Unlock" + level, 1);
	}

	public static bool isLevelUnlocked(string level)
	{
		return PlayerPrefs.GetInt("Unlock" + level, 0) == 1 ? true : false;
	}

	public static bool RemoveAds
	{
		get { return PlayerPrefs.GetInt("RemoveAds", 0) == 1 ? true : false; }
		set { PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0); }
	}

	public static int normalBullet
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("normalBullet", 10)); }
		set { PlayerPrefs.SetInt("normalBullet", value); }
	}

	public static int shieldBullet
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("normalBullet", 10)); }
		set { PlayerPrefs.SetInt("normalBullet", value); }
	}

	public static int grenade
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("grenade", 10)); }
		set { PlayerPrefs.SetInt("grenade", value); }
	}

	public static bool CheckUnlockCharacter(int ID)
	{
		return (PlayerPrefs.GetInt(GlobalValue.Character + ID, 0) == 1 ? true : false);
	}
	public static int storeGod
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("storeGod", 0)); }
		set { PlayerPrefs.SetInt("storeGod", value); }
	}
	public static int storeTimer
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("storeTimer", 0)); }
		set { PlayerPrefs.SetInt("storeTimer", value); }
	}
	public static int storeSlowmotionTimer
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("storeSlowmotionTimer", 0)); }
		set { PlayerPrefs.SetInt("storeSlowmotionTimer", value); }
	}
	public static int storeShield
	{
		get { return Mathf.Max(0, PlayerPrefs.GetInt("storeShield", 0)); }
		set { PlayerPrefs.SetInt("storeShield", value); }
	}

	public static int lastTimeWatchRewardedVideoAd = -999;

	public static void SetScrollLevelAte(int scrollID)
	{
		//Debug.LogError("EAT: " + scrollID);
		PlayerPrefs.SetInt("AteScroll" + levelPlaying + scrollID, 1);
	}

	public static bool IsScrollLevelAte(int scrollID)
	{
		//Debug.LogError(scrollID + ":" + (PlayerPrefs.GetInt("AteScroll" + levelPlaying + scrollID, 0) == 1));
		return PlayerPrefs.GetInt("AteScroll" + levelPlaying + scrollID, 0) == 1 ? true : false;
	}

	public static bool IsScrollLevelAte(int level, int scrollID)
	{
		return PlayerPrefs.GetInt("AteScroll" + level + scrollID, 0) == 1 ? true : false;
	}

	//public static int Scroll
	//{
	//	get { return PlayerPrefs.GetInt("Scroll", 0); }
	//	set { PlayerPrefs.SetInt("Scroll", value); }
	//}
}
