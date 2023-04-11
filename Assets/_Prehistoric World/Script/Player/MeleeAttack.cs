using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeAttack : MonoBehaviour {
	[Tooltip("What layers should be hit")]
	public LayerMask CollisionMask;
	[Tooltip("Hit more than one enemy at the same time")]
	public bool multiDamage = false;
	[Tooltip("Give damage to the enemy or object")]
	public float damageToGive;
	[Tooltip("Apply the force to enemy if they are hit, only for Rigidbody object")]
	public Vector2 pushObject = new Vector2 (5, 1);
	public Transform MeleePoint;
	public float attackZone = .7f;

	public float attackRate = 0.2f;
	float lastTimeAttack = -99;
	[Tooltip("Check target in range after a delay time, useful to sync the right attack time of the animation")]
	public float attackAfterTime = 0.15f;

	float nextAttack = 0;
	public GameObject DetectEnemies;
	public AudioClip soundAttack;
	public GameObject hitFX;

	//called by animation event
	public void ComboEnd(){
		GameManager.Instance.Player.anim.SetInteger ("combo_", 0);
        ComboKillOff ();
	}

	public bool CanAttack()
	{
		if ((Time.time - lastTimeAttack) > attackRate)
		{
			lastTimeAttack = Time.time;
			return true;
		}
		else
			return false;
	}

	public void PlaySoundAttack(int combo)
	{
		SoundManager.PlaySfx(soundAttack);
	}

	public void CheckEnemy(){
		var hits = Physics2D.CircleCastAll (MeleePoint.position, attackZone, Vector2.zero,0,CollisionMask);

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

            damage.TakeDamage(damageToGive, pushObject, GameManager.Instance.Player.gameObject, hit.point);
            
            if (hitFX)
				Instantiate (hitFX, hit.point, hitFX.transform.rotation);
			
			if (!multiDamage)
				break;
		}
    }

	public void ComboKillOn(){
		DetectEnemies.SetActive (true);
	}

	public void ComboKillOff(){
		DetectEnemies.SetActive (false);
	}

	void OnDrawGizmos(){
		if (MeleePoint == null)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (MeleePoint.position, attackZone);
	}
}
