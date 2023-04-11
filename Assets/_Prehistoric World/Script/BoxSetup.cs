using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Controller2D))]
public class BoxSetup : MonoBehaviour {
	public enum Type{Normal, Physical

		}
	public Type boxType;
	Animator anim;
    [HideInInspector]
    public Controller2D controller;

    void Start(){
		anim = GetComponent<Animator> ();

        if(boxType == Type.Normal)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        controller = GetComponent<Controller2D>();
    }
	void Update(){
		if (GameManager.Instance.Player.isDragging && GameManager.Instance.Player.pushPullObj.dragObj == gameObject) {
            if(anim)
			anim.SetBool ("isDragging", true);
		} else if(anim)
			anim.SetBool ("isDragging", false);
	}

    public void MoveBox(Vector3 velocity, Vector2 finalInput)
    {
        transform.rotation = Quaternion.identity;
        controller.Move(velocity * Time.deltaTime, finalInput);
    }

    public bool BoxHitLeft()
    {
        //Debug.LogError("LEFT" + controller.collisions.left);
        return controller.collisions.left;
    }

    public bool BoxHitRight()
    {
        //Debug.LogError("RIGHT" + controller.collisions.right);
        return controller.collisions.right;
    }
}
