using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour,ICanTakeDamage {

	[Range(1,300)]
	public float health = 100;
	public AudioClip soundHit;
	public AudioClip soundDestroy;
	public GameObject DestroyFX;


	[Header("Explosion Damage")]
	public bool makeDamageToOther = false;
	public LayerMask collisionLayer;
	public float makeDamage = 100;
	public float radius = 3;
	public bool multiDamage = true;
	// Use this for initialization
	bool explosioned = false;

	void Destroy(){
		if (DestroyFX)
			Instantiate (DestroyFX, transform.position, Quaternion.identity);

		Destroy (gameObject);
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (explosioned)
			return;

		health -= damage;
		if (health <= 0) {
			explosioned = true;
			SoundManager.PlaySfx (soundDestroy);
			if (makeDamageToOther)
				DoExplosion ();
			
			Destroy ();
			return;
		}

		SoundManager.PlaySfx (soundHit);
	}

	#endregion

	private void DoExplosion(){
		var hits = Physics2D.CircleCastAll (transform.position, radius, Vector2.zero,0, collisionLayer);
		if (hits == null)
			return;

		foreach (var hit in hits) {
//			Debug.Log (hit.collider.name);
			var damage = (ICanTakeDamage) hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
			if (damage == null)
				continue;
			
			damage.TakeDamage (makeDamage,Vector2.zero, gameObject, hit.point);
			if (!multiDamage)
				return;

		}
	}

	void OnDrawGizmos(){
		if (!makeDamageToOther)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, radius);
	}
}
