using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveDamage : MonoBehaviour {

	[Header("Make Damage")]
	public int Damage = 20;
    public bool damageOnce = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageOnce)
            return;

        if (GetComponent<CanBeJumpOn>() && transform.position.y < GameManager.Instance.Player.transform.position.y && GameManager.Instance.Player.controller.collisions.ClosestHit && GameManager.Instance.Player.controller.collisions.ClosestHit.collider.gameObject == gameObject)
            return;

        if (other.gameObject.GetComponent(typeof(ICanTakeDamage)))
        {
            other.gameObject.GetComponent<ICanTakeDamage>().TakeDamage(Damage, Vector2.one * 5, gameObject, other.transform.position);
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if (damageOnce)
            return;

        if (GetComponent<CanBeJumpOn>() && transform.position.y < GameManager.Instance.Player.transform.position.y && GameManager.Instance.Player.controller.collisions.ClosestHit && GameManager.Instance.Player.controller.collisions.ClosestHit.collider.gameObject == gameObject)
            return;

        if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
			other.gameObject.GetComponent<ICanTakeDamage> ().TakeDamage (Damage, Vector2.one * 5, gameObject, other.transform.position);
		}
	}
}
