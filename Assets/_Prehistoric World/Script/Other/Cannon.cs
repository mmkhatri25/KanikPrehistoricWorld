using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {
	public Transform FirePoint;
	public Animator anim;
	public float force = 500;
	public AudioClip sound;

	void LateUpdate(){
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> ()) {

            anim.SetBool ("Rotate", true);

			other.transform.position = transform.position;


			GameManager.Instance.Player.isInCannon = true;
			other.transform.SetParent (transform);
			GameManager.Instance.Player.imageCharacterSprite.color = new Color (1, 1, 1, 0);

		}
	}

	public void FireCannon(){
		if (anim.GetBool ("Rotate")) {
			anim.SetBool ("Rotate", false);
			GameManager.Instance.Player.isInCannon = false;
			Vector2 angleV = (FirePoint.position - transform.position).normalized;

			GameManager.Instance.Player.imageCharacterSprite.color = new Color (1, 1, 1, 1);
			GameManager.Instance.Player.AddForce (new Vector2 (angleV.x * force, angleV.y * force));
			SoundManager.PlaySfx (sound);
            GameManager.Instance.Player.transform.parent = null;

        }
	}

	void OnTriggExit2D(Collider2D other){
		if (other.GetComponent<Player> ()) {
			anim.SetBool ("Rotate", false);
			GameManager.Instance.Player.isInCannon = false;
			GameManager.Instance.Player.imageCharacterSprite.color = new Color (1, 1, 1, 1);
		}
	}
}
