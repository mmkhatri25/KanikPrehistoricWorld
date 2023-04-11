using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, ICanTakeDamage {
    public static Shield Instance;
	float time;
	int hits;
	public AudioClip soundHit; 
	public AudioClip soundUsing;
	AudioSource soundUsingAudioSource;

	GameObject hitFX;

	public Vector2 followOffset = new Vector2(0,0.3f);
	public GameObject Effect1, Effect2, Effect3;

	public void Start(){
        Instance = this;

        GameManager.Instance.isUsingShield = true;

		soundUsingAudioSource = gameObject.AddComponent<AudioSource> ();
		soundUsingAudioSource.clip = soundUsing;
		soundUsingAudioSource.loop = true;
		soundUsingAudioSource.Play ();
//		soundUsingAudioSource.volume = 0;

	}

    void OnDisable()
    {
        Instance = null;
    }

    int pick = 0;

	public void Init (float _time = 5, int _hits=3, GameObject _hitFX = null, int effect = 1) {
		time = _time;
		hits = _hits;
		soundHit = soundHit;
		hitFX = _hitFX;
		StartCoroutine (Stop (time));

        pick = effect;


        Effect1.SetActive (effect == 1);
		Effect2.SetActive (effect == 2);
		Effect3.SetActive (effect == 3);
	}

    private void Update()
    {
        Effect1.SetActive(pick == 1 && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone"));
        Effect2.SetActive(pick == 2 && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone"));
        Effect3.SetActive(pick == 3 && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone"));
    }

    // Update is called once per frame
    void FixedUpdate () {
		transform.position = GameManager.Instance.Player.transform.position + (GameManager.Instance.Player.inverseGravity ? (Vector3)followOffset * -1 : (Vector3)followOffset);
		transform.localScale = Vector3.one * Mathf.Abs (GameManager.Instance.Player.transform.localScale.x);
		Effect2.GetComponent<ParticleSystem> ().gravityModifier = Mathf.Abs (Effect2.GetComponent<ParticleSystem> ().gravityModifier) *(GameManager.Instance.Player.transform.localScale.y > 0? -1:1);
	}

	IEnumerator Stop(float delay){
		yield return new WaitForSeconds (delay);
		GetComponent<CircleCollider2D> ().enabled = false;
		GetComponent<Animator> ().SetTrigger ("stop");
		yield return new WaitForSeconds (0.2f);
		GameManager.Instance.isUsingShield = false;
		gameObject.SetActive (false);
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
	}

	#endregion

	public void Hit(GameObject obj = null){
		if (hits <= 0)
			return;
		
		hits--;
		Debug.Log ("HIT LEFT: " + hits);
		SoundManager.PlaySfx (soundHit);
		if (hitFX)
			Instantiate (hitFX, transform.position + new Vector3 (0.5f, 0, 0), Quaternion.identity);
	

		GetComponent<Animator> ().SetTrigger ("hit");
		if (hits <= 0) {
			StopShield ();
		}

		if (obj != null) {
			if (obj.GetComponent<ShieldCanKillObj> ())
				obj.SetActive (false);
		}
	}

	public void StopShield(){
		StopAllCoroutines ();
		StartCoroutine (Stop (0));
	}
}