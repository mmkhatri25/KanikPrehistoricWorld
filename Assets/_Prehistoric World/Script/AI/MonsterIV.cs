using UnityEngine;
using System.Collections;

public class MonsterIV : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener, IListener
{
	public AudioClip soundDead;
	public GameObject deadFx;
	public Transform linePoint;

	private LineRenderer line;
	Vector3 oldPosition;
	SpringJoint2D springJoint;
	Rigidbody2D rig;
	public bool dropBonusItem = true;
	public GameObject dropItem;

	void Start(){
		line = GetComponent<LineRenderer> ();
		oldPosition = transform.position;
		springJoint = GetComponent<SpringJoint2D> ();
		rig = GetComponent<Rigidbody2D> ();
	}
    void Update()
    {
        line.SetPosition(0, linePoint.position);
        line.SetPosition(1, transform.position);
    }

	public void Dead(){
		SoundManager.PlaySfx(soundDead);

		if (deadFx != null)
		Instantiate (deadFx, transform.position, Quaternion.identity);

		//turn off all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = false;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = false;
		}

		springJoint.enabled = false;
		line.enabled = false;
		rig.velocity = Vector2.zero;
		rig.AddForce (new Vector2 (0, 300f));

		if (dropBonusItem && dropItem)
			Instantiate (dropItem, transform.position + Vector3.up * 0.5f, Quaternion.identity);

		gameObject.SetActive (false);
		if (transform.parent)
			transform.parent.gameObject.SetActive (false);
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		Dead ();
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		transform.position = oldPosition;
		transform.rotation = Quaternion.Euler (0, 0, 0);
		gameObject.SetActive (true);

		//turn on all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = true;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = true;
		}

		rig.isKinematic = true;
		springJoint.enabled = true;
		line.enabled = true;
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
            Vector2 _makeForce = new Vector2(Mathf.Clamp(Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
                Mathf.Clamp(Mathf.Abs(Player.velocity.y), 5, 9) * facingDirectionY * -1);

           Player.TakeDamageFromContactEnemy(makeDamage, _makeForce, gameObject, true);

        }
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
    }

    public void IOnStopMovingOff()
    {
        //isStop = false;
        rig.isKinematic = false;
    }

    public void IOnStopMovingOn()
    {
        //isStop = true;
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
    }

    public void IPause()
    {
    }

    public void IPlay()
    {
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }
}