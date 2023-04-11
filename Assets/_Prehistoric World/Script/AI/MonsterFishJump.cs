using UnityEngine;
using System.Collections;

public class MonsterFishJump : MonoBehaviour, ICanTakeDamage, IListener {
	public float jumpForce = 500;
	public float rotate = 60f;
	public float delayAttack = 0.35f;
	public AudioClip soundAttack;
	public AudioClip soundDead;
	public GameObject deadFx;
	public int scoreRewarded = 200;

	private bool isAttack = false;

	public bool dropBonusItem = true;
	public GameObject dropItem;

	public void Attack(){
		transform.Rotate (Vector3.forward, -rotate);
		StartCoroutine (WaitAndAttack (delayAttack));

		Destroy (gameObject, 5);
	}

	IEnumerator WaitAndAttack(float time){
		yield return new WaitForSeconds (time);
		SoundManager.PlaySfx (soundAttack);
		isAttack = true;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<Rigidbody2D> ().AddRelativeForce(new Vector2(-jumpForce,0));
	}

	public void Dead(){
		SoundManager.PlaySfx(soundDead);
		GlobalValue.SavedPoints+= scoreRewarded;
		Instantiate (deadFx, transform.position, Quaternion.identity);
		if (dropBonusItem && dropItem)
			Instantiate (dropItem, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (isAttack) {
			if (other.CompareTag ("Player")) {
				Dead ();
				//Push player up
				other.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				other.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, 300f));
			}
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
		//		anim.enabled = false;
		isStop = true;
		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	public void IOnStopMovingOff ()
	{
		//		anim.enabled = true;
		isStop = false;
		if(isAttack)
		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

	#endregion
}