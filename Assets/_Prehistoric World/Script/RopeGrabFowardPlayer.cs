using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class RopeGrabFowardPlayer : MonoBehaviour {
    public static RopeGrabFowardPlayer Instance;
    public LineRenderer lineRen;
    public LineRenderer lineRenDetect;
    [ReadOnly] public RopeGrabFoward currentRopeInRange;

    public Transform startRopePoint;
    public float moveSpeed = 20;
    public float pushForceWhenReached = 15;

    public AudioClip throwSound, hookGrabPointSound;

    public GameObject ropeHookPrefab;
    GameObject ropeHook;

    public bool isWorking { get; set; }

    private void Update()
    {
        if (!isWorking && currentRopeInRange != null)
        {
            lineRenDetect.positionCount = 2;
            lineRenDetect.SetPosition(0, startRopePoint.position);
            lineRenDetect.SetPosition(1, currentRopeInRange.transform.position);
        }
        else
            lineRenDetect.positionCount = 0;
    }

    public void SetRope(RopeGrabFoward _ropeInTarget)
    {
        if (isWorking)
            return;

        //check if player face to this rope point
        if ((GameManager.Instance.Player.isFacingRight && (_ropeInTarget.transform.position.x < GameManager.Instance.Player.transform.position.x)) ||
            (!GameManager.Instance.Player.isFacingRight && (_ropeInTarget.transform.position.x > GameManager.Instance.Player.transform.position.x)))
        {
            UnSetRope(_ropeInTarget);
            return;
        }

        if (currentRopeInRange != null)
        {
            if(Vector2.Distance(GameManager.Instance.Player.transform.position, _ropeInTarget.transform.position) < Vector2.Distance(GameManager.Instance.Player.transform.position, currentRopeInRange.transform.position))
            {
                currentRopeInRange = _ropeInTarget;
            }
        }else
            currentRopeInRange = _ropeInTarget;
    }

    public void UnSetRope(RopeGrabFoward _ropeInTarget)
    {
        if (isWorking)
            return;

        if (currentRopeInRange == _ropeInTarget)
            currentRopeInRange = null;
    }

    // Use this for initialization
    void Start () {
        Instance = this;

        ropeHook = Instantiate(ropeHookPrefab) as GameObject;
        ropeHook.SetActive(false);

        lineRen.positionCount = 0;
    }

    public bool isAvailable()
    {
        //Debug.LogError(currentRopeInRange != null && !isWorking ? true : false);
        
        return currentRopeInRange != null && !isWorking ? true : false;
    }

    public void GrabRopeAction()
    {
        if (isWorking)
            return;

        if (currentRopeInRange == null)
        {
            Debug.LogError("currentRopeInRange NULL");
            return;
        }

        StartCoroutine(GrabRopeActionCo());
    }

    IEnumerator GrabRopeActionCo()
    {
        //Debug.LogError("GrabRopeActionCo");
        GameManager.Instance.Player.AnimSetTrigger("throwRope");

        isWorking = true;
        SoundManager.PlaySfx(throwSound);

        Vector3 targetPoint = currentRopeInRange.transform.position;
        float movePercent = 0;

        ropeHook.SetActive(true);

        var dir = targetPoint - GameManager.Instance.Player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ropeHook.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        ropeHook.transform.position = startRopePoint.position;
        ropeHook.GetComponent<Animator>().SetBool("grab", false);

        GameManager.Instance.Player.isPlaying = false;
        GameManager.Instance.Player.isRoping = true;

        while (Vector2.Distance(ropeHook.transform.position, targetPoint) > 0.05f)
        {
            //Debug.LogError("Loop 1");
            //Debug.LogError(ropeHook.transform.gameObject.name + "/" + targetPoint);
            movePercent += moveSpeed * Time.deltaTime;
            ropeHook.transform.position = Vector2.MoveTowards(startRopePoint.position, targetPoint, movePercent);

            lineRen.positionCount = 2;
            lineRen.SetPosition(0, startRopePoint.position);
            lineRen.SetPosition(1, ropeHook.transform.position);

            yield return null;
        }

        GameManager.Instance.Player.AnimSetBool("isHodingRope", true);

        lineRen.positionCount = 0;
        movePercent = 0;
        Vector3 beginMovePlayerPos = GameManager.Instance.Player.transform.position;
        ropeHook.GetComponent<Animator>().SetBool("grab", true);
        SoundManager.PlaySfx(hookGrabPointSound);

        //Debug.LogError(playerMoveDirection);

        while (Vector2.Distance(GameManager.Instance.Player.transform.position, targetPoint) > 0.05f)
        {
            //Debug.LogError("Loop 2");
            movePercent += moveSpeed * Time.deltaTime;
            GameManager.Instance.Player.transform.position = Vector2.MoveTowards(beginMovePlayerPos, targetPoint, movePercent);

            lineRen.positionCount = 2;
            lineRen.SetPosition(0, startRopePoint.position);
            lineRen.SetPosition(1, targetPoint);


            yield return null;
        }

        

        lineRen.positionCount = 0;
        ropeHook.SetActive(false);

        GameManager.Instance.Player.isPlaying = true;
        GameManager.Instance.Player.isRoping = false;

        Vector2 playerMoveDirection = (targetPoint - beginMovePlayerPos).normalized;
        GameManager.Instance.Player.SetForce(playerMoveDirection * pushForceWhenReached);

        if (currentRopeInRange)
            currentRopeInRange.Used();
        
        currentRopeInRange = null;
        
        isWorking = false;
        GameManager.Instance.Player.AnimSetBool("isHodingRope", false);
        //Debug.LogError("GrabRopeActionCo Finish");
    }
}
