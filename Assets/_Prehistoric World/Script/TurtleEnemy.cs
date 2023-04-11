using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Controller2D))]

public class TurtleEnemy : Enemy
{
    [HideInInspector]
    private Vector2 _direction;
    private float _directionFace;
    public int maxHitWallTimes = 6;
    int hitWallCounts = 0;

    bool isHiding = false;
    bool isRushing = false;

    public AudioClip soundRush;
    public AudioClip soundRushHitWall;

    public override void Start()
    {
        base.Start();

        controller = GetComponent<Controller2D>();
        //_direction = Vector2.right;
        //if (moveSpeed == 0) {
        _direction = isFacingRight() ? Vector2.right : Vector2.left;
        isPlaying = true;

        controller.collisions.faceDir = 1;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        HandleAnimation();

        if (!isPlaying || !GameManager.Instance.Player.isPlaying)
        {
            velocity.x = 0;
            return;
        }

        if (!isPlayerDetected)
        {
            if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
                DetectPlayer(delayAttackWhenDetectPlayer);
        }

        if (isWaiting)
        {
            waitingTime += Time.deltaTime;
            if ((waitingTime >= waitingTurn && !isPlayerDetected))
            {
                isWaiting = false;
                waitingTime = 0;
                Flip();
            }
        }
        else
        {
            if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
               || (!canBeFallDown && !controller.isGrounedAhead(isFacingRight()) && controller.collisions.below) || (!isPlayerDetected && ((!isFacingRight() && usePatrolPoint && transform.position.x <= _patrolLimitLeft) || (isFacingRight() && usePatrolPoint && transform.position.x > _patrolLimitRight))))
            {
                if (isRushing && controller.collisions.right)
                {
                    var dmg = (ICanTakeDamage)controller.collisions.hitRight.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (dmg != null)
                        dmg.TakeDamage(1000, Vector2.zero, gameObject, Vector3.zero);
                    else if (isRushing)
                        Flip();
                    else
                        isWaiting = true;
                }
                else if (isRushing && controller.collisions.left)
                {
                    var dmg = (ICanTakeDamage)controller.collisions.hitLeft.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (dmg != null)
                        dmg.TakeDamage(1000, Vector2.zero, gameObject, Vector3.zero);
                    else if (isRushing)
                        Flip();
                    else
                        isWaiting = true;
                }
                else if (isRushing)
                    Flip();
                else
                    isWaiting = true;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRushing)
        {
            var dmg = (ICanTakeDamage)collision.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (dmg != null)
                dmg.TakeDamage(1000, Vector2.zero, gameObject, Vector3.zero);
        }
    }

    public virtual void LateUpdate()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            if (isRushing)
                Die();

            return;
        }

        if (!isPlaying)
        {
            if (isDead && dieBehavior == DIEBEHAVIOR.FALLOUT)
            {
                velocity.y += -gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime, false);
            }
            else
            {
                velocity = Vector2.zero;
            }
            return;
        }

        if (!GameManager.Instance.Player.isPlaying)
            return;

        float targetVelocityX = _direction.x * moveSpeed;
        if (isRushing)
            velocity.x = targetVelocityX;
        else
        velocity.x = isWaiting ? 0 : Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);

        velocity.y += -gravity * Time.deltaTime;


        if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE || (isPlayerDetected && (_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)))
            velocity.x = 0;

        if (isPlayerDetected && (enemyType == ENEMYTYPE.INWATER || enemyType == ENEMYTYPE.FLY))
        {
            if (enemyState == ENEMYSTATE.WALK)
            {
                Vector2 targetPoint = (Vector2)GameManager.Instance.Player.transform.position + Vector2.right * (Mathf.Abs(chasingOffset.x)) * (isFacingRight() ? -1f : 1f) + Vector2.up * chasingOffset.y;

                velocity = (targetPoint - (Vector2)transform.position).normalized * moveSpeed;
            }
            else
                velocity = Vector2.zero;
        }

        if (isStopping)
            velocity = Vector2.zero;

        if (onlyMoveWhenGrounded && !controller.collisions.below)
            velocity.x = 0;

        if (isHiding && !isRushing)
            velocity.x = 0;

        //Debug.LogError(velocity);
        controller.Move(velocity * Time.deltaTime, false/*, isFacingRight ()*/);

        if (isPlayerDetected && velocity.x == 0 && enemyState == ENEMYSTATE.IDLE)
        {
            countingStanding += Time.deltaTime;
            if (isPlayerDetected && countingStanding >= dismissPlayerWhenStandSecond)
                DismissDetectPlayer();
        }
        else
            countingStanding = 0;


        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;
    }

    void Flip()
    {
        _direction = -_direction;
        if (!isRushing && !isHiding)
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 180 : 0, transform.rotation.z));
        if (isRushing)
        {
            SoundManager.PlaySfx(soundRushHitWall);
            hitWallCounts++;
            if (hitWallCounts >= maxHitWallTimes)
                Die();
        }
    }


    void HandleAnimation()
    {
        AnimSetFloat("speed", Mathf.Abs(velocity.x));
        AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);
        AnimSetBool("isHiding", isHiding);
        usePatrolPoint = false;
    }

    public void SetForce(float x, float y)
    {
        velocity = new Vector3(x, y, 0);
    }

    public override void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (GameManager.Instance.Player.GodMode)
            Die();
        else
        {
            if (isHiding)
            {
                if (instigator == GameManager.Instance.Player.gameObject)
                {
                    _direction = transform.position.x > instigator.transform.position.x ? Vector2.right : Vector2.left;
                    if (!isRushing)
                        Rushing();
                    else
                        HidingIdle();
                }
                else
                {
                    Die();
                }
            }
            else
            {
                if (instigator == GameManager.Instance.Player.gameObject)
                {
                    HidingIdle();
                }
                else
                {
                    Die();
                }
            }
        }
    }

    void HidingIdle()
    {
        isHiding = true;
        isWaiting = true;
        isRushing = false;

        SoundManager.PlaySfx(soundHit);
        //UltiHelper.AddLayerMask(controller.collisionMask, "GetHitObject");
        controller.collisionMask = UltiHelper.SubtractLayerMask(controller.collisionMask, gameObject.layer) ;
    }
    

    void Rushing()
    {
        isWaiting = false;
        isRushing = true;
        canBeFallDown = true;       //force fall out from platform
        moveSpeed = runSpeed;
        SoundManager.PlaySfx(soundRush);
        //GameManager.Instance.Player.AnimKickAction();
        //controller.collisionMask =  UltiHelper.AddLayerMask(controller.collisionMask, "Enemies");
        controller.collisionMask = controller.collisionMask | (1 << LayerMask.NameToLayer("Enemies"));
    }

    public override void OnPlayerContactRight()
    {
        if (isHiding && !isRushing)
        {
            _direction = Vector2.left;
            Rushing();
        }
        else
            base.OnPlayerContactRight();
    }

    public override void OnPlayerContactLeft()
    {
        if (isHiding && !isRushing)
        {
            _direction = Vector2.right;
            Rushing();
        }
        else
            base.OnPlayerContactLeft();
    }

    public override void Die()
    {
        if (isDead)
            return;

        base.Die();


        CancelInvoke();

        var cols = GetComponents<BoxCollider2D>();
        foreach (var col in cols)
            col.enabled = false;

        if (enemyEffect == ENEMYEFFECT.FREEZE)
        {
            gameObject.SetActive(false);
            return;
        }


        AnimSetTrigger("die");

        if (enemyEffect == ENEMYEFFECT.BURNING)
            return;

        StopAllCoroutines();

        StartCoroutine(DisableEnemy(2));
    }

    public override void Hit()
    {
        if (!isPlaying)
            return;

        base.Hit();
        if (isDead)
            return;

        AnimSetTrigger("hit");
    }

    IEnumerator DisableEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        //gameObject.SetActive (false);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (usePatrolPoint && startAction != STARTACTION.STAND && walkSpeed > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position - Vector3.right * patrolLimitLeft, 0.3f);
            Gizmos.DrawWireSphere(transform.position + Vector3.right * patrolLimitRight, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * patrolLimitRight);
            Gizmos.DrawLine(transform.position, transform.position - Vector3.right * patrolLimitLeft);
        }

        if (!Application.isPlaying)
        {
            if (enemyType == ENEMYTYPE.FLY)
                gravity = 0;

            if (startAction == STARTACTION.STAND)
            {
                walkSpeed = 0;
            }
        }
    }
}
