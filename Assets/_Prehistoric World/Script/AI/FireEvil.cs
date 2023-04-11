using UnityEngine;
using System.Collections;

public class FireEvil : MonoBehaviour, ICanTakeDamage, IListener {
	public float speed = 5;
	float _direction;
	Vector3 originalPos;
	public bool canBeKill = true;

	public GameObject endPoint;
	Vector3 endPos;

	public AudioClip sound;
	AudioSource EngineAudio;
	void Awake(){
		originalPos = transform.position;
		if (endPoint)
			endPos = endPoint.transform.position;
	}

	// Use this for initialization
	void Start () {
		_direction = transform.localScale.x > 0 ? -1 : 1;
		GameManager.Instance.listeners.Add (this);

		EngineAudio = gameObject.AddComponent<AudioSource> ();
		EngineAudio.clip = sound;
		EngineAudio.Play ();
		EngineAudio.loop = true;

	}

    bool isMovingBack = false;
    // Update is called once per frame
    void Update () {
        if (isStop)
            return;

		transform.Translate (speed  * Time.deltaTime * _direction, 0, 0);
        if (!isMovingBack)
        {
            if (endPoint)
            {
                if (Mathf.Abs(transform.position.x - endPos.x) < 0.5f)
                {
                    speed = -speed;
                    EngineAudio.Stop();
                    isMovingBack = true;
                }
            }

            EngineAudio.volume = GlobalValue.isSound ? 0.85f : 0;
        }
        else
        {
            if (Mathf.Abs(transform.position.x - originalPos.x) < 0.5f)
            {
                gameObject.SetActive(false);
            }
        }
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if(canBeKill)
		gameObject.SetActive (false);
	}

    #endregion

    bool isStop = false;
	#region IListener implementation

	public void IPlay ()
	{
	}

	public void ISuccess ()
	{
	}

	public void IPause ()
	{
	}

	public void IUnPause ()
	{
	}

	public void IGameOver ()
	{
	}

	public void IOnRespawn ()
	{
		Debug.Log ("RRR");
		transform.position = originalPos;
		gameObject.SetActive (false);
        isMovingBack = false;
        speed = -Mathf.Abs (speed);
		EngineAudio.Play ();
	}


	public void IOnStopMovingOn ()
	{
        isStop = true;

    }

	public void IOnStopMovingOff ()
	{
        isStop = false;

    }
    #endregion

    //[Header("Push player if Godmod or Shield")]
    //public float pushForce = 10;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Instance.Player.gameObject)
        {
            if (Shield.Instance != null)
            {
                //GameManager.Instance.Player.velocity.x = pushForce * (GameManager.Instance.Player.transform.position.x > transform.position.x ? 1 : -1);
                GameManager.Instance.Player.transform.Translate(Mathf.Abs(speed) * (GameManager.Instance.Player.transform.position.x > transform.position.x ? 1 : -1) * Time.deltaTime, 0, 0);
            }
            else
            {
                GameManager.Instance.Player.TakeDamage(100000, Vector2.zero, gameObject, collision.transform.position);
            }
        }
    }

    void OnDrawGizmos(){
		if(endPoint)
			Gizmos.DrawLine (transform.position, endPoint.transform.position);
	}
}
