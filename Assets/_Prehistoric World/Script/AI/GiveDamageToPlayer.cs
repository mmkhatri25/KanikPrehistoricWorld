using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour, ICanTakeDamage {
	[Header("Option")]
	[Tooltip("destroy this object when hit player?")]
	public bool isDestroyWhenHitPlayer = false;
	public GameObject DestroyFx;

	[Header("Make Damage")]
	public int DamageToPlayer = 20;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2 (0, 10);
	float nextDamage;
	public bool canBeHitByOther = true;

	IEnumerator OnTriggerEnter2D(Collider2D other){
		if (this) {
			var Player = other.GetComponent<Player> ();
			if (Player == null)
				yield break;
			
			if (!Player.isPlaying)
				yield break;

            if (Player.GodMode)
                yield break;

            if (Player.gameObject.layer == LayerMask.NameToLayer ("HidingZone"))
				yield break;

            if (GetComponent<CanBeJumpOn>() && transform.position.y < GameManager.Instance.Player.transform.position.y)
                yield break;
			
			if (Time.time < nextDamage + rateDamage)
				yield break;
			
			nextDamage = Time.time;
			if (DamageToPlayer == 0)
				yield break;
			
			var facingDirectionX = Mathf.Sign (Player.transform.position.x - transform.position.x);
			var facingDirectionY = Mathf.Sign (Player.velocity.y);

			Player.SetForce (new Vector2 (Mathf.Clamp (Mathf.Abs (Player.velocity.x), 10, 15) * facingDirectionX,
				Mathf.Clamp (Mathf.Abs (Player.velocity.y), 5, 9) * facingDirectionY * -1));

			Player.TakeDamage (DamageToPlayer, Vector2.zero, gameObject, other.transform.position);

			if (isDestroyWhenHitPlayer) {
				if (DestroyFx != null)
					Instantiate (DestroyFx, transform.position, Quaternion.identity);

				Destroy (gameObject,0.1f);
			}
		}
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (canBeHitByOther)
			Destroy (gameObject);
	}

	#endregion
}
