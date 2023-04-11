using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushAndPullUI : MonoBehaviour {
	//public GameObject button;
	private Button but;

	public bool isDragable{ get; set; }

	bool isDetectPlayer;
	// Use this for initialization
	void Start () {
		but = GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//button.SetActive ((GameManager.Instance.Player.pushPullObj.isDetectObject || GameManager.Instance.Player.isDragging) && (Mathf.Abs (GameManager.Instance.Player.velocity.y) < 1));
		but.interactable = ((GameManager.Instance.Player.pushPullObj.isDetectObject || GameManager.Instance.Player.isDragging)
		&& (Mathf.Abs (GameManager.Instance.Player.velocity.y) < 1));
		
		isDragable = ((GameManager.Instance.Player.pushPullObj.isDetectObject || GameManager.Instance.Player.isDragging)
			&& (Mathf.Abs (GameManager.Instance.Player.velocity.y) < 1));
	}

	public void Drag(){
		if (isDragable) {
			GameManager.Instance.Player.DragBegin ();

		}
	}

	public void Stop(){
		if (isDragable) {
			GameManager.Instance.Player.DragStop ();
		}
	}
}
