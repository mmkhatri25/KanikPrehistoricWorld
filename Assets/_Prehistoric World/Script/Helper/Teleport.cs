using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {
	public Transform position1;
	public Transform position2;

	public float teleportTimer = 1.2f;
	public float smoothTranparent = 20f;

	public AudioClip sound;

	public void TeleportPlayer(Vector3 currentPos){
		SoundManager.PlaySfx (sound);
		if (currentPos == position1.position) {
			GameManager.Instance.Player.Teleport (position2, teleportTimer,smoothTranparent);
		} else {
			GameManager.Instance.Player.Teleport (position1, teleportTimer,smoothTranparent);
		}
	}

	GameObject lastObj;
	public void TeleportObj(Vector3 currentPos, GameObject obj){
		SoundManager.PlaySfx (sound);
		if (obj == lastObj) {
			lastObj = null;
			return;
		}

		lastObj = obj;
		if (currentPos == position1.position) {
			obj.transform.position = position2.position;
		} else {
			obj.transform.position = position1.position;
		}
	}

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(position1.position, position2.position);
    }
}
