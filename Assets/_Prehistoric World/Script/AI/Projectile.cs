using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {
	public float Speed;

	public LayerMask LayerCollision;

	public GameObject Owner{ get; private set; }
	public Vector2 Direction{ get; private set; }
	public Vector2 InitialVelocity{ get; private set; }
	public float NewDamage{ get; private set; }

	[HideInInspector]
	public bool Explosion;

	// Use this for initialization
	public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity, bool powerBullet, bool _isUseRadar = false, int damage = -1, float speed = -1)
	{
	
		if (damage > -1)
			NewDamage = damage;

		transform.right = direction;    //turn the X asix to the direction
		Owner = owner;
		if (_isUseRadar)
			Invoke("SetUseRadarDelay", 0.3f);
		Direction = direction;
		InitialVelocity = initialVelocity;
		OnInitialized();
	}

	// Use this for initialization
	public void Initialize (GameObject owner, Vector2 direction, Vector2 initialVelocity,bool isExplosion = false, bool canGoBackToOwner = false, float _newDamage = 0) {
		transform.right = direction;	//turn the X asix to the direction
		Owner = owner;
		Direction = direction;
		InitialVelocity = initialVelocity;
		NewDamage = _newDamage;

		Explosion = isExplosion;

		OnInitialized ();
	}

	public virtual void OnInitialized(){
	}


	Vector2 lastPosition;
	public virtual void Start()
	{
		lastPosition = transform.position;
	}

	public virtual void Update()
	{
		var hits = Physics2D.LinecastAll(lastPosition, transform.position, LayerCollision);
		if (hits.Length > 0)
		{
			ContactTarget(hits);
		}
	}

	public virtual void LateUpdate()
	{
		lastPosition = transform.position;
	}

	void ContactTarget(RaycastHit2D[] hits)
	{
		foreach (var hit in hits)
		{
			bool isOwner = false;
			var anotherSimpleProjectile = hit.collider.gameObject.GetComponent<SimpleProjectile>();
			if (anotherSimpleProjectile != null)
			{
				isOwner = Owner == anotherSimpleProjectile.Owner;
			}
			if (isOwner)
			{
				OnCollideOwner();
			}
			else
			{
				var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
				if (takeDamage != null)
				{
					if (hit.collider.gameObject.GetComponent(typeof(Projectile)) != null)
						OnCollideOther(hit);
					else
						OnCollideTakeDamage(hit, takeDamage);
				}
				else
				{
					OnCollideOther(hit);
				}
			}
		}
	}

	protected virtual void OnNotCollideWith(RaycastHit2D other){
	}

	protected virtual void OnCollideOwner (){}

	protected virtual void OnCollideTakeDamage(RaycastHit2D other, ICanTakeDamage takedamage){}

	protected virtual void OnCollideOther(RaycastHit2D other){}
}
