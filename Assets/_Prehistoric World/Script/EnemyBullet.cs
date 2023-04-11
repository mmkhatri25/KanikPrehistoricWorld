using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour, ICanTakeDamage, IListener {
	public enum DirectionAttack{RightToLeft, LeftToRight, UpToDown, DownToUp}
	public DirectionAttack directionAttack;

	public float speed = 0.1f;
	public AudioClip soundDead;
	public GameObject deadFx;
	public int damage = 50;
    public int timeLive = 5;

	//private bool isStop = false;
	public AudioClip soundEngine;
	AudioSource _soundEngine;

	void OnEnable(){
		switch (directionAttack) {
		case DirectionAttack.RightToLeft:
			transform.right = Vector2.right;
			break;
		case DirectionAttack.LeftToRight:
			transform.right = Vector2.left;
			break;
		case DirectionAttack.DownToUp:
			transform.right = Vector2.down;
			break;
		case DirectionAttack.UpToDown:
			transform.right = Vector2.up;
			break;
		default:
			break;
		}

        if (GameManager.Instance)
            GameManager.Instance.listeners.Add(this);

        Invoke("Disable", timeLive);
	}

	void Start(){
		_soundEngine = GetComponent<AudioSource> ();
		_soundEngine.clip = soundEngine;
		_soundEngine.Play ();
		_soundEngine.loop = true;
		_soundEngine.volume = GlobalValue.isSound ? 0.85f : 0;
	}

    void Disable() {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isStop)
			transform.Translate (-speed, 0, 0, Space.Self);
	}

	void OnBecameInvisible() {
		Destroy (gameObject);	//destroy this object when invisible
	}

	public void Dead(){
		SoundManager.PlaySfx(soundDead);
//		GameManager.Score += scoreRewarded;
		Instantiate (deadFx, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == GameManager.Instance.Player.gameObject)
		{
			GameManager.Instance.Player.TakeDamage(damage, Vector2.one, gameObject, transform.position);
			Dead();
		}
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		Dead ();
	}

    #endregion
    bool isStop = false;
    #region IListener implementation

    public void IPlay()
    {
    }

    public void ISuccess()
    {
    }

    public void IPause()
    {
    }

    public void IUnPause()
    {
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
    }

    public void IOnStopMovingOn()
    {
        isStop = true;
        CancelInvoke();
    }

    public void IOnStopMovingOff()
    {
        isStop = false;
        Invoke("Disable", timeLive);
    }

    #endregion
}
