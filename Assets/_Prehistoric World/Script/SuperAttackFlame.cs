using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperAttackFlame : MonoBehaviour {
	public LayerMask target;
	public float timeOn = 1f;
	public float timeOff = 1.5f;

	public float damage = 20;

	public ParticleSystem beginParticSys;
	public ParticleSystem[] ParticlsSys;
	bool hitTarget = false;
	BoxCollider2D box2D;

	// Use this for initialization
	void Start () {
		foreach(var child in ParticlsSys){
			child.gameObject.SetActive (false);
		}

		box2D = GetComponent<BoxCollider2D> ();
		box2D.enabled = false;

//		RaycastHit2D hit = Physics2D.Raycast (GameManager.Instance.Player.transform.position, Vector2.down, 10, layerGround);
//		if (hit) {
//			transform.position = hit.point;

			Invoke ("TurnOn", timeOn);
//		}
	}

	public void TurnOn(){
		box2D.enabled = true;
		foreach(var child in ParticlsSys){
			child.gameObject.SetActive (true);
			var em = child.emission;
			em.enabled = true;
		}

		Invoke ("TurnOff", timeOn);
	}

	public void TurnOff(){
		//		foreach(var child in ParticlsSys){
		foreach(var child in ParticlsSys){
			var em = child.emission;
			em.enabled = false;
		}

		var em2 = beginParticSys.emission;
		em2.enabled = false;
		box2D.enabled = false;
		//		Invoke ("TurnOn", timeOff);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (hitTarget)
			return;

		if (target == (target | (1 << other.gameObject.layer))) {
			var _damage = (ICanTakeDamage)other.GetComponent (typeof(ICanTakeDamage));
			if (_damage != null) {
				_damage.TakeDamage (damage, Vector2.zero, gameObject, other.transform.position);
				hitTarget = true;
				box2D.enabled = false;
			}
		}
	}
}
