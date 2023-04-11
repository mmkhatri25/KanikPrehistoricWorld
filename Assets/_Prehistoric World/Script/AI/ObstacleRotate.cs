using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRotate : MonoBehaviour, IListener {
	public float speed = 3;
	public float maxAngle = 60;
	//public float turnOffset = 2f;
	//int dir = 1;
    float startZ;
	// Use this for initialization
	void Start () {
        startZ = transform.rotation.eulerAngles.z - (transform.up.y > 0 ? 0 : 360);

    }
	
	// Update is called once per frame
	void Update () {
		if (isStop)
			return;
		
//		var rotate = Mathf.Clamp (Mathf.Lerp (transform.rotation.z, maxAngle, speed * Time.deltaTime),-maxAngle,maxAngle);
//		var rotate = Mathf.Lerp (transform.eulerAngles.z - (transform.rotation.z>=0? 0:360), dir*maxAngle, speed * Time.deltaTime);
//
//		Quaternion rotation = Quaternion.Euler(0, 0, rotate);
//
//		transform.rotation = rotation;

		transform.rotation = Quaternion.Euler (0, 0, startZ + Mathf.PingPong (speed * Time.time, maxAngle * 2) - maxAngle);
			
//		if (Mathf.Abs (transform.eulerAngles.z - (transform.rotation.z>=0? 0:360) - dir*maxAngle) < turnOffset)
//			dir *= -1;

	}
	bool isStop = false;
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
		isStop = true;
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		isStop = false;
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
//		anim.enabled = false;
		isStop = true;
		//		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	public void IOnStopMovingOff ()
	{
//		anim.enabled = true;
		isStop = false;
		//		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

	#endregion

}
