using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlatform : MonoBehaviour, IListener {
	public float workingAngle = 45;
	public float speed = 10;

	public Transform dot;
	public Transform end;

	public Transform target;

	private float offset = 90f;
	private bool goRight;
	private LineRenderer lineRend;

	// Use this for initialization
	void Start () {
		lineRend = GetComponent<LineRenderer> ();

		lineRend.SetPosition (1, end.localPosition);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (isStop)
			return;
		
		if (goRight) {
			transform.Rotate (Vector3.forward, speed * Time.deltaTime);

			Vector3 dirx = end.position - dot.position;
			float angle = Mathf.Atan2 (dirx.y, dirx.x) * Mathf.Rad2Deg + offset;
			if (angle > workingAngle)
				goRight = false;
		} else {
			transform.Rotate (Vector3.forward, -speed * Time.deltaTime);

			Vector3 dirx = end.position - dot.position;
			float angle = Mathf.Atan2 (dirx.y, dirx.x) * Mathf.Rad2Deg + offset;
			if (angle < -workingAngle)
				goRight = true;
		}

		if (target)
			target.position = end.position;
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
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
		isStop = true;

	}

	public void IOnStopMovingOff ()
	{
		isStop = false;
	}

    #endregion

    private void OnDrawGizmos()
    {
		if (Application.isPlaying)
			return;

        if (lineRend == null)
        {
			lineRend = GetComponent<LineRenderer>();
		}

        if (lineRend != null)
        {
			lineRend.SetPosition(1, end.localPosition);
		}
    }
}
