using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour {
	bool isWorking = true;
	public GameObject fx;

    private void Start()
    {
		GetComponent<BoxCollider2D>().enabled = isWorking;
	}

    public void Switch()
    {
		isWorking = !isWorking;
		GetComponent<BoxCollider2D>().enabled = isWorking;
	}

	void Update () {
		fx.SetActive(isWorking);
		Debug.LogWarning (GameManager.Instance.Player.inverseGravity);
	}

	void OnTriggerStay2D(Collider2D other){
		if (!isWorking)
			return;

		if (other.GetComponent<Rigidbody2D> ()) {
			if (!other.gameObject.CompareTag ("Monster"))
				other.GetComponent<Rigidbody2D> ().gravityScale = -Mathf.Abs (other.GetComponent<Rigidbody2D> ().gravityScale);

		}

		if (other.gameObject != GameManager.Instance.Player.gameObject) {
			return;
		}

		GameManager.Instance.Player.EnterGravityZone ();
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.GetComponent<Rigidbody2D> ())
			other.GetComponent<Rigidbody2D> ().gravityScale = Mathf.Abs (other.GetComponent<Rigidbody2D> ().gravityScale);
		
		if (other.gameObject != GameManager.Instance.Player.gameObject) {
			return;
		}
		GameManager.Instance.Player.ExitGravityZone ();

	}
}
