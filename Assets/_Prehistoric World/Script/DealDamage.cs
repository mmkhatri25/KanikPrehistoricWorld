using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour {
	public Vector2 attackZone;
	public LayerMask CollisionMask;
	public float dealDamage = 30;
	public Vector2 pushObject = new Vector2 (5, 1);
	public bool multiDamage = true;

	// Use this for initialization
	void Start () {
		var hits = Physics2D.BoxCastAll (transform.position, attackZone, 0, Vector2.zero, 0, CollisionMask);

		if (hits == null)
			return;
		foreach (var hit in hits) {
			Debug.Log (hit.collider.name);
			var damage = (ICanTakeDamage) hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
			if (damage == null)
				continue;

			var projectile = (Projectile) hit.collider.gameObject.GetComponent (typeof(Projectile));
			if (projectile != null && projectile.Owner == gameObject)
				continue;

			damage.TakeDamage (dealDamage,pushObject, GameManager.Instance.Player.gameObject, hit.point);
			if (!multiDamage)
				return;

		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube (transform.position, attackZone);
	}
}
