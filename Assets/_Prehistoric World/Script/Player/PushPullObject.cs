using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullObject : MonoBehaviour {

	public LayerMask dragableObjectLayer;
	public float distance = 2;
	public Transform leflfPos, rightPos;

	public float speed = 1;

	public bool isDetectObject{ get; set; }
	public RaycastHit2D hit { get; set; }
	private Player player;
	public AudioClip dragSound;
	private AudioSource dragASource;

	public GameObject dragObj;
	// Use this for initialization
	void Start () {
//		yield return new WaitForSeconds (0);



		dragASource = gameObject.AddComponent<AudioSource> ();
		dragASource.loop = true;
		dragASource.volume = 0f;
		dragASource.clip = dragSound;
		dragASource.Play ();
//		player = GameManager.Instance.Player;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.Player) {
			isDetectObject = CheckObject ();

//			Debug.Log ("isDetectObject = " + isDetectObject);

			if (!isDetectObject && GameManager.Instance.Player.isDragging)
				GameManager.Instance.Player.DragStop ();

			if(GameManager.Instance.Player.isDragging && Mathf.Abs( GameManager.Instance.Player.velocity.x) >0.1f)
				dragASource.volume = GlobalValue.isSound ? 1 : 0;
			else
				dragASource.volume = 0;

//			Debug.Log (isDetectObject + "/");
			Debug.DrawRay (transform.position, (GameManager.Instance.Player.isFacingRight? 1:-1) * Vector2.right * distance);
		}

	}

	private bool CheckObject(){
		hit = Physics2D.Raycast (transform.position, (GameManager.Instance.Player.isFacingRight? 1:-1) * Vector2.right, distance, dragableObjectLayer);
		if (hit && hit.collider.gameObject.GetComponent<BoxSetup> ()) {
			dragObj = hit.collider.gameObject;
		}
		return hit;
	}

//	void OnDrawGizmos(){
//		Gizmos.color = Color.yellow;
//		Gizmos.DrawRay (transform.position, distance*Vector2.right);
//	}
}
