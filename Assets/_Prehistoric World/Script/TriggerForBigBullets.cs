using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForBigBullets : MonoBehaviour, IListener  {
	public bool canUseAgain = true;
	public AudioClip soundShowUp;
	public GameObject[] BigBullets;
	bool isWorked = false;
	// Use this for initialization
	void Start () {
		foreach (var obj in BigBullets) {
			if (obj != null)
				obj.SetActive (false);
		}
	}


	//when detect Player, set active all monsters in list
	IEnumerator OnTriggerEnter2D(Collider2D other){
		if (isWorked)
			yield break;

		if (other.GetComponent<Player> () == null)
			yield break;

		for (int i = 0; i < BigBullets.Length; i++) {
			if (BigBullets [i] != null)
				Instantiate (BigBullets [i], BigBullets [i].transform.position, BigBullets [i].transform.rotation).gameObject.SetActive (true);
		}

		SoundManager.PlaySfx (soundShowUp);

		isWorked = true;
	}

	void OnDrawGizmos(){
		if (BigBullets.Length > 0) {
			foreach (var obj in BigBullets) {
				if(obj)
					Gizmos.DrawLine (transform.position, obj.transform.position);
			}
		}
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
		isWorked = !canUseAgain;
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
