using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour, IListener
{
    //	public bool needKeepContact = true; 	//need contact to keep platform move
    [Header("Move when detect player")]
    public bool moveWhenLedge = true;
    public bool moveWhenClimbVine = true;
    public bool moveWhenStandOn = true;
    bool isDetectPlayer = false;

    public float speedForward = 9;
    public float speedBackward = 3;
    public Transform movingObj;
    public Transform point1, point2;
    int nextPoint = 0;
    public bool isMoving { get; set; }
    public AudioClip clickSound;
    public AudioClip movingSound;
    AudioSource movingSoundSrc;
    public Animator chainAnim;
    public Animator platformAnim;

    public bool isLoop = true;
    bool isFinishWay = false;
    bool isMoveFoward = true;
    // Use this for initialization
    void Start()
    {
        movingObj.position = point1.position;
        movingSoundSrc = gameObject.AddComponent<AudioSource>();
        movingSoundSrc.clip = movingSound;
        movingSoundSrc.Play();
        movingSoundSrc.loop = true;
        movingSoundSrc.volume = 0;
    }

    void Move()
    {
        isMoveFoward = true;
        isMoving = true;
        isFinishWay = false;
    }

    void Stop()
    {
        isMoving = false;
        movingSoundSrc.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //		if (!needKeepContact)
        //			return;

        if (isStop)
            return;

        if (chainAnim)
            chainAnim.enabled = isMoving;
        platformAnim.SetBool("isWorking", isMoving);

        if (isMoving)
        {
            movingObj.position = Vector3.MoveTowards(movingObj.position, isMoveFoward ? point2.position : point1.position, (isMoveFoward ? speedForward : speedBackward) * Time.deltaTime);

            if (isDetectPlayer && !GameManager.Instance.Player.controller.collisions.below)
            {
                GameManager.Instance.Player.transform.position += movingObj.position - lastPos + GameManager.Instance.Player.input.y * Vector3.up * Time.deltaTime * 3;
            }

            movingSoundSrc.volume = GlobalValue.isSound ? 1 : 0;

            if (isMoveFoward && movingObj.position == point2.position)
            {
                if (isLoop)
                    isMoveFoward = false;
                else
                    isFinishWay = true;
            }
            if (!isMoveFoward && movingObj.position == point1.position)
            {

                if (isDetectPlayer)
                    Move();
                else
                    Stop();
            }
        }
        else if (isDetectPlayer)
        {
            if ((GameManager.Instance.Player.controller.collisions.below && GameManager.Instance.Player.transform.position.y > transform.position.y))
            {

                Move();
            }
        }

        if (isFinishWay && isMoveFoward && !isDetectPlayer)
        {
            isMoveFoward = false;
            isMoving = true;
        }
    }
    Vector3 lastPos;
    void LateUpdate()
    {
        lastPos = movingObj.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.LogError(other.gameObject);
        if (other.gameObject == GameManager.Instance.Player.gameObject)
        {
            
            isDetectPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == GameManager.Instance.Player.gameObject)
        {
            isDetectPlayer = false;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        float size = .3f;
        Gizmos.DrawSphere(point1.position, size);
        Gizmos.DrawSphere(point2.position, size);
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(point1.position, point2.position);

    }

    bool isStop = false;
    #region IListener implementation

    public void IPlay()
    {
        //		throw new System.NotImplementedException ();
    }

    public void ISuccess()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IUnPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IGameOver()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnRespawn()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnStopMovingOn()
    {
        Debug.Log("IOnStopMovingOn");
        //		anim.enabled = false;
        isStop = true;

    }

    public void IOnStopMovingOff()
    {
        //		anim.enabled = true;
        isStop = false;
    }

    #endregion
}
