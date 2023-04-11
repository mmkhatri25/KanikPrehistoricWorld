using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGrabFoward : MonoBehaviour, IListener {
    public float radius;
    public LayerMask playerLayer;
    public float coolDownTime = 1;
    bool isUsed = false;
    Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
        anim.SetBool("canGrab", GameManager.Instance.Player.ropeGrabFoward.currentRopeInRange == this);

        if (isUsed)
            return;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, playerLayer);
        
        if (hit)
        {
            if (GameManager.Instance.Player.playerWithPartner)
                return;
            
            if (RopeGrabFowardPlayer.Instance)
                RopeGrabFowardPlayer.Instance.SetRope(this);
        }
        else
        {
            if (RopeGrabFowardPlayer.Instance)
                RopeGrabFowardPlayer.Instance.UnSetRope(this);
        }

        
    }

    public void Used()
    {
        isUsed = true;

        Invoke("CanReUse", coolDownTime);
    }

    void CanReUse()
    {
        isUsed = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void IPlay()
    {
        
    }

    public void ISuccess()
    {
       
    }

    public void IPause()
    {
        
    }

    public void IUnPause()
    {
        
    }

    public void IGameOver()
    {
       
    }

    public void IOnRespawn()
    {
        isUsed = false;
    }

    public void IOnStopMovingOn()
    {
        
    }

    public void IOnStopMovingOff()
    {
       
    }
}
