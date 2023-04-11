using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveFoward : MonoBehaviour, IListener {
	public float speed = 1;
	Rigidbody2D rig;
	public float timeToLive = 5;
	float timeCouint = 0;
	// Update is called once per frame
	void Start(){
		rig = GetComponent<Rigidbody2D> ();
//		Destroy (gameObject, timeToLive);
		GameManager.Instance.listeners.Add (this);
		rig.isKinematic = true;
	}
	void Update () {
		if (isStop)
			return;
		
		transform.Translate (speed * Time.deltaTime, 0, 0,Space.Self);
		rig.velocity = transform.right * speed * Time.deltaTime;

		timeCouint += Time.deltaTime;
		if (timeCouint > timeToLive)
			Destroy (gameObject);
	}

	#region IListener implementation

	public void IPlay ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void ISuccess ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IUnPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		//		throw new System.NotImplementedException ();
	}

	bool isStop = false;
	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
		//		anim.enabled = false;
		if (GetComponent<Animator> ())
			GetComponent<Animator> ().enabled = false;
		isStop = true;
	}

	public void IOnStopMovingOff ()
	{
		if (GetComponent<Animator> ())
			GetComponent<Animator> ().enabled = true;
		isStop = false;
	}
	#endregion
}
