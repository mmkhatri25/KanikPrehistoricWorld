﻿using UnityEngine;
using System.Collections;

public class GiveDamageToPlayerX : MonoBehaviour {
	//[Header("Option")]
	//[Tooltip("destroy this object when hit player?")]
	//public bool isDestroyWhenHitPlayer = false;
	//public GameObject DestroyFx;

	//[Header("Make Damage")]
	//public int DamageToPlayer;
	//[Tooltip("delay a moment before give next damage to Player")]
	//public float rateDamage = 0.2f;
	
	//float nextDamage;
 //   [Header("JUMP ON HEAD")]
 //   [Tooltip("Give damage to this object when Player jump on his head")]
	//public bool canBeKillOnHead = false;
	//public float damageOnHead;
 //   public Vector2 pushPlayer = new Vector2(0, 10);
 //   public Transform headPoint;

	//void OnTriggerStay2D(Collider2D other){
	//	var Player = other.GetComponent<Player> ();
	//	if (Player == null)
	//		return;

	//	if (!Player.isPlaying)
	//		return;

	//	if (Time.time < nextDamage + rateDamage)
	//		return;

	//	nextDamage = Time.time;

 //       if (canBeKillOnHead && Player.feetPosition.transform.position.y > (headPoint != null ? headPoint.position.y : transform.position.y))
 //       {
 //           Player.velocity = pushPlayer;
 //           var canTakeDamage = (ICanTakeDamage)GetComponent(typeof(ICanTakeDamage));
 //           if (canTakeDamage != null)
 //               canTakeDamage.TakeDamage(damageOnHead, Vector2.zero, gameObject, transform.position);

 //           return;
 //       }
 //       else if (!canBeKillOnHead &&  GameManager.Instance.Player.GodMode)
 //       {
 //           var canTakeDamage = (ICanTakeDamage)GetComponent(typeof(ICanTakeDamage));
 //           if (canTakeDamage != null)
 //               canTakeDamage.TakeDamage(float.MaxValue, Vector2.zero, gameObject,transform.position);
 //           return;
 //       }
 //       else if (GameManager.Instance.Player.isBlinking)
 //       {
 //           if (Player.feetPosition.transform.position.y > (headPoint != null ? headPoint.position.y : transform.position.y))
 //           {
 //               var facingDirectionX1 = Mathf.Sign(Player.transform.position.x - transform.position.x);
 //               var facingDirectionY1 = Mathf.Sign(Player.velocity.y);

 //               Player.SetForce(new Vector2(Mathf.Clamp(Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX1,
 //                   Mathf.Clamp(Mathf.Abs(Player.velocity.y), 5, 15) * facingDirectionY1 * -1));
 //           }
 //           return;
 //       }
        
	//	if (DamageToPlayer == 0)
	//		return;

	//	var facingDirectionX = Mathf.Sign (Player.transform.position.x - transform.position.x);
	//	var facingDirectionY = Mathf.Sign (Player.velocity.y);

	//	Player.SetForce(new Vector2 (Mathf.Clamp (Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
	//		Mathf.Clamp (Mathf.Abs(Player.velocity.y), 5, 15) * facingDirectionY * -1));

	//	Player.TakeDamage (DamageToPlayer, Vector2.zero, gameObject, Player.transform.position);

	//	if (isDestroyWhenHitPlayer) {
	//		if (DestroyFx != null)
	//			Instantiate (DestroyFx, transform.position, Quaternion.identity);

	//		Destroy (gameObject);
	//	}
	//}
}
