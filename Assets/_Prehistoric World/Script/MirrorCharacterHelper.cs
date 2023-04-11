using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCharacterHelper : MonoBehaviour {
    public SpriteRenderer ownerSpriteRenderer;
    public GameObject damageTrigger;
    float mirrorXPoint;
    SpriteRenderer targetSpriteRenderer;
    GhostSprites ghostFX;
    Transform targetTransform;
    Transform fireTransformPoint;
    bool isWorking = false;
    bool gotoOwner = false;
    float moveX = 0;
    Vector3 moveXPos;
    float moveToOwnerSpeed = 15;

    // Use this for initialization
    void Start () {
        ghostFX = GetComponent<GhostSprites>();
        ghostFX.allowGhost = true;
        damageTrigger.SetActive(false);
    }

    

    public void Init(SpriteRenderer _targetSpriteRenderer, Transform _targetTransform, Transform _fireTransformPoint)
    {

        targetSpriteRenderer = _targetSpriteRenderer;
        targetTransform = _targetTransform;
        fireTransformPoint = _fireTransformPoint;
        mirrorXPoint = _targetTransform.position.x;
        
        isWorking = true;
       
    }

    public void TurnDamageTrigger(bool turn)
    {
        damageTrigger.SetActive(turn);
    }
    
    public void GoToOwner()
    {
        gotoOwner = true;
        moveXPos = transform.position;
    }

    public void FinishUse()
    {
        if (this)
        {
            ghostFX.ClearTrail();
            Destroy(gameObject, 0.1f);
        }
    }

    public Vector3 GetFirePoint()
    {
        return new Vector3(fireTransformPoint.position.x - (fireTransformPoint.position.x - mirrorXPoint) * 2, fireTransformPoint.position.y, fireTransformPoint.position.z);
    }

    
    // Update is called once per frame
    void Update () {
        if (targetSpriteRenderer == null)
            return;

        ownerSpriteRenderer.sprite = targetSpriteRenderer.sprite;
        if (gotoOwner)
        {
            moveX += moveToOwnerSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(moveXPos, targetTransform.position, moveX);

            if (transform.position == targetTransform.position)
                FinishUse();
        }
        else
        {
            transform.position = new Vector3(targetTransform.position.x - (targetTransform.position.x - mirrorXPoint) * 2, targetTransform.position.y, targetTransform.position.z);
            transform.localScale = targetTransform.localScale;
        }
        ghostFX.allowGhost = true;
    }
}
