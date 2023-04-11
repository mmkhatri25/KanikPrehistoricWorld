using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile, ICanTakeDamage, IListener
{
	public int Damage;
	public GameObject DestroyEffect;
	public float timeToLive;
	public AudioClip soundHitEnemy;
	[Range(0,1)]
	public float soundHitEnemyVolume = 0.5f;
	public AudioClip soundHitNothing;
	[Range(0,1)]
	public float soundHitNothingVolume = 0.5f;
	public GameObject ExplosionObj;
	public GameObject destroyParent;

	public override void Start(){
		GameManager.Instance.listeners.Add (this);
	}

	public override void Update()
	{
		if (isStop)
			return;

		if (destroyParent == null)
			destroyParent = gameObject;

		if ((timeToLive -= Time.deltaTime) <= 0)
		{
			DestroyProjectile();
		}

		transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime * (GameManager.Instance.Player.isRunning ? 1.5f : 1), Space.World);

		base.Update();
	}

	void DestroyProjectile(){
		if (DestroyEffect != null)
			Instantiate (DestroyEffect, transform.position, Quaternion.identity);

		if (Explosion) {
			var bullet = Instantiate (ExplosionObj, transform.position, Quaternion.identity) as GameObject;
			bullet.GetComponent<Grenade> ().DoExplosion (0);
		}
		
		Destroy (destroyParent!=null? destroyParent: gameObject);
	}


	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		SoundManager.PlaySfx (soundHitNothing, soundHitNothingVolume);
		DestroyProjectile ();
	}

	protected override void OnCollideOther (RaycastHit2D other)
	{
		SoundManager.PlaySfx (soundHitNothing, soundHitNothingVolume);
		DestroyProjectile ();
	}

	protected override void OnCollideTakeDamage (RaycastHit2D other, ICanTakeDamage takedamage)
	{
		takedamage.TakeDamage ((NewDamage == 0 ? Damage : NewDamage), Vector2.zero, Owner, other.point);
		SoundManager.PlaySfx (soundHitEnemy, soundHitEnemyVolume);
		DestroyProjectile ();
	}

	bool isStop = false;
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
//		Debug.Log ("IOnStopMovingOn");
//		anim.enabled = false;
		isStop = true;
		//		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	public void IOnStopMovingOff ()
	{
//		anim.enabled = true;
		isStop = false;
		//		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

	#endregion
}

