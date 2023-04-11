using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class SpawnItem : MonoBehaviour {
	[Range(0,100)]
	public float timeToLive = 0f;		//0 mean no destroy
	public float gravity = 35f;
	public float minForce = 9;
	public float maxForce = 9;

	Vector3 velocity = Vector3.zero;
	Controller2D controller;
	bool isPlaying;
	float velocityXSmoothing;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();

		if (timeToLive != 0)
			Destroy (gameObject, timeToLive);

		velocity.y = Random.Range (minForce, maxForce);
	}

	void LateUpdate(){
		velocity.y += -gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime, false);
		velocity.x = 0;

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;
	}
}
