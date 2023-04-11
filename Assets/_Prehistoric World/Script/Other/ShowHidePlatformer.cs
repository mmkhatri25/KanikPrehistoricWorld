using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHidePlatformer : MonoBehaviour, IListener {
	public AudioClip sound;
	public GameObject[] platformers;
	[ReadOnly] public List<GameObject> platformAvailable;

	public float showTime = 1;
	float timer;
	int current;
	public AudioSource ASource;
	// Use this for initialization
	void Start()
	{
		timer = showTime;
		current = 0;

		platformAvailable = new List<GameObject>();
		for (int i = 0; i < platformers.Length; i++)
		{
			if (platformers[i].activeSelf)
				platformAvailable.Add(platformers[i]);
		}

		ShowPlatformer(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (isStop)
			return;
		
		if (timer > 0) {
			timer -= Time.deltaTime;
			return;
		}

		timer = showTime;
		current++;
		if (current == platformAvailable.Count)
			current = 0;

		ShowPlatformer (current);
	}

	void ShowPlatformer(int i){
		foreach (GameObject obj in platformAvailable) {
			obj.SetActive (false);
		}

		platformAvailable[i].SetActive (true);
		if (ASource) {
			ASource.clip = sound;
			ASource.volume = GlobalValue.isSound ? 0.5f : 0;
			if (Vector2.Distance(platformAvailable[i].transform.position, GameManager.Instance.Player.transform.position) < 8)
				ASource.Play();
		}

		if(i>0)
			platformAvailable[i-1].SetActive (true);
		else if(i == 0)
			platformAvailable[platformAvailable.Count - 1].SetActive (true);
	}

	bool isStop = false;
	#region IListener implementation

	public void IPlay ()
	{

	}

	public void ISuccess ()
	{

	}

	public void IPause ()
	{

	}

	public void IUnPause ()
	{

	}

	public void IGameOver ()
	{

	}

	public void IOnRespawn ()
	{

	}

	public void IOnStopMovingOn ()
	{
		isStop = true;
	}

	public void IOnStopMovingOff ()
	{
		isStop = false;
	}

	#endregion
}
