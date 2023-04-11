using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public enum Type
	{
Camera,
		Player

	}

	public Type followTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (followTarget == Type.Camera)
			transform.position = new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.y);
		else
			transform.position = GameManager.Instance.Player.transform.position;
	}
}
