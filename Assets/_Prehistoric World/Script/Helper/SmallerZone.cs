using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallerZone : MonoBehaviour {

	public float smallerSize = 0.5f;
	public bool useGhostFX = true;
	float normalSize = 0;
	bool allow = true;
	// Use this for initialization
	void Start () {
		normalSize = Mathf.Abs (GameManager.Instance.Player.transform.localScale.x);
       
    }
	
	IEnumerator OnTriggerEnter2D(Collider2D other){
        while (!allow) { yield return null; }

        var bounds = GetComponent<BoxCollider2D>().bounds;
        if(!Physics2D.BoxCast(bounds.center, bounds.size,0,Vector2.zero,0, 1 << (LayerMask.NameToLayer("Player"))))
        {
            yield break;
        }

        if (!other.gameObject.GetComponent<Player> ())
            yield break;

        if (Mathf.Abs (GameManager.Instance.Player.transform.localScale.x) == smallerSize) {
            yield break;
            //			GameManager.Instance.Player.transform.localScale = new Vector3 (Mathf.Sign (GameManager.Instance.Player.transform.localScale.x) * normalSize,
            //				Mathf.Sign (GameManager.Instance.Player.transform.localScale.y) * normalSize,
            //				Mathf.Sign (GameManager.Instance.Player.transform.localScale.z) * normalSize);
            //
            //			GameManager.Instance.Player.ghostSprite.allowGhost = false;
        } else {
//			normalSize = GameManager.Instance.Player.transform.localScale.x;
			GameManager.Instance.Player.transform.position += Vector3.right * 0.3f * (GameManager.Instance.Player.isFacingRight?1:-1);
			GameManager.Instance.Player.transform.localScale = new Vector3 (
				Mathf.Sign (GameManager.Instance.Player.transform.localScale.x) * smallerSize,
				Mathf.Sign (GameManager.Instance.Player.transform.localScale.y) * smallerSize, 
				Mathf.Sign (GameManager.Instance.Player.transform.localScale.z) * smallerSize);

			GameManager.Instance.Player.forceGhostFX = useGhostFX;
		}

//		allow = false;
//		Invoke ("Delay", 1);
	}

	void OnTriggerStay2D(Collider2D other){
        if (!allow)
            return;

        if (!other.gameObject.GetComponent<Player> ())
			return;

		GameManager.Instance.Player.forceGhostFX = useGhostFX;
	}

	void OnTriggerExit2D(Collider2D other){
		if (!other.gameObject.GetComponent<Player> ())
			return;

		if (Mathf.Abs (GameManager.Instance.Player.transform.localScale.x) == smallerSize) {
			GameManager.Instance.Player.transform.position += Vector3.right * 0.3f * (GameManager.Instance.Player.isFacingRight?1:-1);
			GameManager.Instance.Player.transform.position += Vector3.one * 0.5f;
			GameManager.Instance.Player.transform.localScale = new Vector3 (Mathf.Sign (GameManager.Instance.Player.transform.localScale.x) * normalSize,
				Mathf.Sign (GameManager.Instance.Player.transform.localScale.y) * normalSize,
				Mathf.Sign (GameManager.Instance.Player.transform.localScale.z) * normalSize);

			GameManager.Instance.Player.forceGhostFX = false;
		} else {
			return;
			//			normalSize = GameManager.Instance.Player.transform.localScale.x;
//			GameManager.Instance.Player.transform.localScale = new Vector3 (
//				Mathf.Sign (GameManager.Instance.Player.transform.localScale.x) * smallerSize,
//				Mathf.Sign (GameManager.Instance.Player.transform.localScale.y) * smallerSize, 
//				Mathf.Sign (GameManager.Instance.Player.transform.localScale.z) * smallerSize);
//
//			GameManager.Instance.Player.ghostSprite.allowGhost = useGhostFX;
		}

        //		allow = false;
        //		Invoke ("Delay", 1);

        allow = false;
        Invoke("Delay", 0.1f);
	}

    void Delay()
    {
        allow = true;
    }
}
