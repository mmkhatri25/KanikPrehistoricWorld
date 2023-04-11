 using UnityEngine;
using System.Collections;

public class GiveBullet : MonoBehaviour, IPlayerRespawnListener, ITriggerPlayer
{
	public int bulletToAdd = 1;
	public GameObject Effect;
	public bool isRespawnCheckPoint = true;
	public AudioClip sound;
	[Range(0, 1)]
	public float soundVolume = 0.5f;

	bool isWorked = false;

	public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
	{
		if (isRespawnCheckPoint)
			gameObject.SetActive(true);
	}

	public void OnTrigger()
	{
		if (isWorked)
			return;

		isWorked = true;

		SoundManager.PlaySfx(sound, soundVolume);

		GameManager.Instance.AddNormalBullet(bulletToAdd, transform);

		if (Effect != null)
			Instantiate(Effect, transform.position, transform.rotation);
		gameObject.SetActive(false);
	}
}