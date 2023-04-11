using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour, IPlayerRespawnListener, ITriggerPlayer {
	public int coinToAdd = 1;
	public GameObject Effect;
	public bool isRespawnCheckPoint = true;
	public AudioClip sound;
	[Range(0,1)]
	public float soundVolume = 0.5f;

    bool isWorked = false;

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
        if (isRespawnCheckPoint)
        {
            gameObject.SetActive(true);
            isWorked = false;
        }
	}

    public void OnTrigger()
    {
		if (isWorked)
			return;

		isWorked = true;
		SoundManager.PlaySfx(sound, soundVolume);
		GameManager.Instance.AddCoin(coinToAdd, transform);

		if (Effect != null)
			Instantiate(Effect, transform.position, transform.rotation);

		gameObject.SetActive(false);
	}
}
