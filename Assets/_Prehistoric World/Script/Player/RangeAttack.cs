using UnityEngine;
using System.Collections;

public class RangeAttack : MonoBehaviour
{
	public Transform FirePoint;
	public Projectile Projectile;
	public float normalDamage = 50;
	[Tooltip("fire projectile after this delay, useful to sync with the animation of firing action")]
	public float fireDelay;
	public float fireRate;
	[HideInInspector] public bool inverseDirection = false;
	public AudioClip shootSound;
	[Range(0, 1)]
	public float shootSoundVolume = 0.5f;

	[HideInInspector] public float holdingTime = 1;       //when hold fire button over this time, active special button
	float holdingTimeCounter;
	[HideInInspector]
	public bool isHolding = false;
	[HideInInspector]
	public bool checkHolding = false;

	float nextFire = 0;

	public bool Fire(bool powerBullet, bool isHold = false)
	{
		checkHolding = false;

		if (GlobalValue.normalBullet > 0)
		{
			if (Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;
				GlobalValue.normalBullet--;
				StartCoroutine(DelayAttack(fireDelay, false));
				return true;
			}
			else return false;
		}
		else
			return false;
	}

	void Update()
	{
		if (checkHolding)
		{
			holdingTimeCounter -= Time.deltaTime;
			if (holdingTimeCounter <= 0)
				isHolding = true;
		}
		else
		{
			holdingTimeCounter = holdingTime;
		}
	}
	public void CancleHolding()
	{
		isHolding = false;
		checkHolding = false;
		holdingTimeCounter = holdingTime;
	}

	public void CheckingHoldButton()
	{
		checkHolding = true;
	}

	[HideInInspector]
	public float bulletPointPosX;

	void Start()
	{
		bulletPointPosX = FirePoint.localPosition.x;
	}

	IEnumerator DelayAttack(float time, bool powerBullet)
	{
		GameManager.Instance.Player.anim.SetTrigger("range_attack");
		yield return new WaitForSeconds(time);

		var direction = GameManager.Instance.Player.isFacingRight ? Vector2.right : Vector2.left;

		if (inverseDirection)
			direction *= -1;

		if (GameManager.Instance.Player.wallSliding)
			direction *= -1;

		FirePoint.localPosition = new Vector2(GameManager.Instance.Player.wallSliding ? -bulletPointPosX : bulletPointPosX, FirePoint.localPosition.y);
		FirePoint.localScale = new Vector3(GameManager.Instance.Player.wallSliding ? -1 : 1, FirePoint.localScale.y, FirePoint.localScale.z);


		Vector3 FirePOS = FirePoint.position;

		Vector2 dir = direction;

		float _dealDamage = normalDamage;


		var projectile = (Projectile)Instantiate(Projectile, FirePOS, Quaternion.identity);
		projectile.Initialize(GameManager.Instance.Player.gameObject, dir, Vector2.zero, powerBullet, false, /*powerBullet?dartDamage:normalDamage*/_dealDamage);
		

		isHolding = false;
	}
}