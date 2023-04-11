using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class MovingItem : MonoBehaviour {

	public float gravity = 35f;
	public float moveSpeed = 3;
	public bool canBeFallDown = false;	//if true, the enemy will be fall from the higher platform
	public AudioClip soundCollected;

	Vector3 velocity;
	Controller2D controller;
	Vector2 _direction;
	bool isPlaying;
	float velocityXSmoothing;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();
		_direction = Vector2.right;
	}
	
	// Update is called once per frame
	void Update () {
		isPlaying = GameManager.Instance.Player.isPlaying;
		if (!isPlaying) {
			velocity.x = 0;
			return;
		}

		if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
			|| (!canBeFallDown && !controller.isGrounedAhead(_direction.x == 1) && controller.collisions.below)) {

			_direction = -_direction;
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}

	void LateUpdate(){
		float targetVelocityX = _direction.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);


		velocity.y += -gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime, false);

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;
	}

	void OnDestroy(){
		if(SoundManager.Instance)
		SoundManager.PlaySfx (soundCollected);
	}
}
