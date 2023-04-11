using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTriggerButton : MonoBehaviour {
	public enum Type{UP,DOWN

		}
	public Type type;
	public ElevatorTrigger elevator;
	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null && hit.collider.gameObject == gameObject) {
				if (type == Type.UP)
					elevator.Up ();
				else
					elevator.Down ();
			}
		}
	}
}
