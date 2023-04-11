using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

	public SpriteRenderer Image;
	public bool isFacingRight = true;

	// Use this for initialization
	void Start () {

		if (Image == null) {
			Image = GetComponent<SpriteRenderer> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		var playerSide = GameManager.Instance.Player.transform.position.x - transform.position.x;

		if (playerSide > 0)
			Image.flipX = !isFacingRight;
		else if (playerSide < 0)
			Image.flipX = isFacingRight;
	}
}
