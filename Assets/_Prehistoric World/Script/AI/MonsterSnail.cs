using UnityEngine;
using System.Collections;

public class MonsterSnail : EnemyAI, IListener {
	[Header("Owner")]
	public Animator anim;
	public float timeBackToAlive = 3f;
	public bool dropBonusItem = true;
	public GameObject dropItem;

	public override void Start ()
	{
		base.Start ();
		healthType = HealthType.HitToKill;		//force to HitToKill
	}

	protected override void HitEvent ()
	{
		base.HitEvent ();

		if (currentHitLeft == 1) {
			anim.SetBool ("hit", true);
			isPlaying = false;
			SoundManager.PlaySfx (hurtSound, hurtSoundVolume);
			if (HurtEffect != null)
				Instantiate (HurtEffect, transform.position, transform.rotation);

			StartCoroutine (BackToAliveCo (timeBackToAlive));
		} else if (isDead)
			Dead ();
	}

	protected override void Dead ()
	{
		StopAllCoroutines ();
		base.Dead ();

		SetForce (0, 5);
		controller.HandlePhysic = false;
		if (dropBonusItem && dropItem)
			Instantiate (dropItem, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        Destroy(gameObject);
	}

	protected override void OnRespawn ()
	{
		anim.SetBool ("hit", false);
		controller.HandlePhysic = true;
	}

	IEnumerator BackToAliveCo(float time){
		isSocking = true;
		yield return new WaitForSeconds (time - 1f);

		anim.SetTrigger ("shake");

		yield return new WaitForSeconds (1f);
		anim.SetBool ("hit", false);
		currentHitLeft = maxHitToKill;		//reset hit
		isSocking = false;
	}

    [Header("Contact Player")]
    public float makeDamage = 30;
    [Tooltip("delay a moment before give next damage to Player")]
    public float rateDamage = 0.2f;
    public Vector2 pushPlayer = new Vector2(15, 10);
    float nextDamage;

    IEnumerator OnTriggerStay2D(Collider2D other)
    {
        if (this)
        {
            var Player = other.GetComponent<Player>();
            if (Player == null)
                yield break;

            if (!Player.isPlaying)
                yield break;

            if (Player.GodMode)
                yield break;

            if (Player.gameObject.layer == LayerMask.NameToLayer("HidingZone"))
                yield break;

            if (GetComponent<CanBeJumpOn>() && transform.position.y + 1 < GameManager.Instance.Player.transform.position.y)
                yield break;

            if (Time.time < nextDamage + rateDamage)
                yield break;

            nextDamage = Time.time;

            if (makeDamage == 0)
                yield break;

            var facingDirectionX = Mathf.Sign(Player.transform.position.x - transform.position.x);
            var facingDirectionY = Mathf.Sign(Player.velocity.y);

            //Player.SetForce(new Vector2(Mathf.Clamp(Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
            //    Mathf.Clamp(Mathf.Abs(Player.velocity.y), 5, 9) * facingDirectionY * -1));
            Vector2 _makeForce = new Vector2(Mathf.Clamp(Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
                Mathf.Clamp(Mathf.Abs(Player.velocity.y), 5, 9) * facingDirectionY * -1);


            Player.TakeDamageFromContactEnemy(makeDamage, _makeForce, gameObject, true);

        }
    }
    #region IListener implementation

    public void IPlay ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void ISuccess ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IUnPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
				anim.enabled = false;
		isStop = true;

	}

	public void IOnStopMovingOff ()
	{
		anim.enabled = true;
		isStop = false;
	}

	#endregion
}
