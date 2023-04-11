using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour, ICanTakeDamage {
	[Header("Explosion Damage")]

	public AudioClip soundDestroy;
	public GameObject DestroyFX;

	public LayerMask collisionLayer;
	public float makeDamage = 100;
	public float radius = 3;
	// Use this for initialization

	Rigidbody2D rig;

	void Awake(){
		rig = GetComponent<Rigidbody2D> ();
	}

	public void Init(float _delayBlowOnGrounded, int _damage, float _radius, bool blowImmediately = false)
	{
		makeDamage = _damage;
		radius = _radius;

		if (blowImmediately)
		{
			DoExplosion();
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (collisionLayer == (collisionLayer | (1 << other.gameObject.layer)))
			DoExplosion ();
	}

	public void DoExplosion(float _forceDamage =9999){
		var hits = Physics2D.CircleCastAll (transform.position, radius, Vector2.zero,0, collisionLayer);
		if (hits == null)
			return;

		foreach (var hit in hits) {
			var damage = (ICanTakeDamage) hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
			if (damage == null)
				continue;

            
			damage.TakeDamage (_forceDamage == 9999? makeDamage:_forceDamage,Vector2.zero, gameObject, hit.point);
		}

		if (DestroyFX)
			Instantiate (DestroyFX, transform.position, Quaternion.identity);

		SoundManager.PlaySfx (soundDestroy);
		Destroy (gameObject);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, radius);
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (DestroyFX)
			Instantiate (DestroyFX, transform.position, Quaternion.identity);

		SoundManager.PlaySfx (soundDestroy);
		Destroy (gameObject);
	}

	#endregion
}
