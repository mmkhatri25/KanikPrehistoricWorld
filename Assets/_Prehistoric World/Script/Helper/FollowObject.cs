using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
	[Tooltip("if this target == null, the target will be Main Camera")]
	public Transform target;
	public Vector2 offset;
	public bool followX = true;
	public bool followY = true;

	public void Init(Transform _target){
		target = _target;
	}

	void Start(){
		if (target == null)
			target = Camera.main.transform;
	}

	void Update () {
		
		if (target == null) {
			Destroy (gameObject);
			return;
		}
//		else {
//			gameObject.SetActive (target.gameObject.activeInHierarchy);
//		}
		
		Vector3 follow = target.position + new Vector3 (offset.x, offset.y * (target.localScale.y > 0 ? 1 : -1),0);
//		if (target.localScale.y < 0)
//			follow.y = -follow.y;

		if (followX && followY)
			transform.position = new Vector3 (follow.x, follow.y, transform.position.z);
		else if (followX)
			transform.position = new Vector3 (follow.x, transform.position.y, transform.position.z);
		else if (followY)
			transform.position = new Vector3 (transform.position.x, follow.y, transform.position.z);
	}
}
