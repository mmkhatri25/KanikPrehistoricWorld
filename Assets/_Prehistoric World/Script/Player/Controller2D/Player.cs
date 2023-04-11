using UnityEngine;
using System.Collections;
public enum PlayerState { Ground, Water, Jetpack }
public enum DoggeType { OverObject, HitObject }
[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour, ICanTakeDamage, IListener
{
    public bool playerWithPartner = false;
    public SwitchPlayerItem partnerItem;
    public LayerMask contactLayer;
    public int ID = 1;
    [ReadOnly] public PlayerState PlayerState = PlayerState.Ground;        //set what state the player in
   
    [HideInInspector] public Color godBlinkColor = new Color(0.2f, .2f, .2f, 1f);     //blink colour
    public SpriteRenderer imageCharacterSprite;     //the Image of the character
    [Header("Knock Back When Be Hit Option")]
    public bool knockBackBeHit = true;
    [Tooltip("If player take the damage >= this value, knock player back a lillte")]
    public float damageKnockBackBeHit = 30;
    public float knockbackForce = 10f;
    [HideInInspector] public bool GodMode = false;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isSliding = false;
    [Header("Setup Parameter")]
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float accGrounedOverride { get; set; }


    [Header("Setup parameter on ground")]
    public PlayerParameter GroundParameter;     //Ground parameters
    [Header("Setup parameter in water zone")]
    public PlayerParameter WaterZoneParameter;      //Water parameters

    private float moveSpeed;        //the moving speed, changed evey time the player on ground or in water
    private float maxJumpHeight;
    private float minJumpHeight;
    private float timeToJumpApex;

    public int numberOfJumpMax = 2;     //number jump allowed
    private int numberOfJumpLeft;
    public GameObject JumpEffect;       //spawn the object when jump if it is placed

    [Header("Health")]
    [Range(0, 50)]
    public int maxHealth = 1000;     //limit the health by 100
    public int Health { get; private set; }
    public GameObject HurtEffect;       //spawn the effect object when get hit

    [ReadOnly]
    public bool isBlinking = false;
    public float blinking = 3;      //blinking time allowed
    public Color blinkColor = new Color(0.2f, .2f, .2f, 1f);        //blink colour

    [Header("Sound")]
    public AudioClip showUpSound;
    public AudioClip finishSound;
    public AudioClip walkSound;
    public float walkSoundSpeed = 1.5f;
    [Range(0, 1)]
    public float walkSoundVolume = 0.5f;
    AudioSource walkSoundSrc;
    [Space]
    public AudioClip jumpSound;
    [Range(0, 1)]
    public float jumpSoundVolume = 0.5f;
    public AudioClip landSound;
    [Range(0, 1)]
    public float landSoundVolume = 0.5f;
    public AudioClip hurtSound;
    [Range(0, 1)]
    public float hurtSoundVolume = 0.5f;
    public AudioClip blowSound;
    public AudioClip deadSoundSlowMotion;
    public AudioClip deadSound;
    [Range(0, 1)]
    public float deadSoundVolume = 0.5f;
    public AudioClip waterIN;
    public AudioClip waterOUT;
    public GameObject waterFX;
    bool isPlayedLandSound;

    [Header("Option")]
    public bool bloodScreenFXHit = true;
     bool goThroughEnemy = false;
   [ReadOnly] public  bool allowHangingRopeShoot = false;      //allow player do this action
    [ReadOnly] public bool allowRopeType2Shoot = false;       //allow player do this action
    [ReadOnly] public bool allowWaterZoneShoot = true;
    [ReadOnly] public bool allowPipeShoot = true;
    bool allowHangingRopeGrenade = false;        //allow player do this action
    bool allowRopeType2Grenade = false;
    bool allowWaterZoneGrenade = false;
    bool allowPipeGrenade = true;

    public RangeAttack rangeAttack { get; set; }
    public MeleeAttack meleeAttack { get; set; }
    public RopeGrabFowardPlayer ropeGrabFoward { get; set; }

    [ReadOnly] public PlayerCheckLadderZone playerCheckLadderZone;

    float gravity;
    float originalGravity;
    [HideInInspector]
    public float maxJumpVelocity;
    float minJumpVelocity;
    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    [HideInInspector]
    public bool isFacingRight
    {
        get
        {
            bool _faceRight = transform.localScale.x > 0;
            if (isHaningRope && transform.root.transform.localScale.x < 0)
                _faceRight = !_faceRight;

            return _faceRight;
        }
        set { }
    }
    [HideInInspector]
    public bool wallSliding;
    int wallDirX;

    bool allowClimpOnTopAgain;
    public bool isClimbOnTop { get; set; }

    [ReadOnly] public Vector2 input;

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public Animator anim;

    public bool isPlaying { get; set; }
    public bool isInStayZone { get; set; }
    public bool isFinish { get; set; }
    public bool isInCannon { get; set; }
    public bool canJumpWhenHidingZone { get; set; }
    public bool canRunWhenHidingZone { get; set; }


    public GhostSprites ghostSprite { get; set; }

    public bool allowMoving { get; set; }
    public bool isFrozen { get; set; }  //player will be frozen

    [HideInInspector]
    public PushPullObject pushPullObj;
    public bool isDragging { get; set; }
    private GameObject currentDragObj;


    bool isBoostSpeed = false;
    bool forceShadowFX = false;

    public bool isJumpPropActive { get; set; }

    public bool isUsingPowerBombAction { get; set; }

    //check if Player is using: Shield or God or Fly or TimeStop or TimeSlow or Power or Partner
    public bool isUsingActions()
    {
        return (FindObjectOfType<Shield>() || isUsingPowerBombAction) || GameManager.Instance.isInDialogue;
    }

    public bool isGrounded { get { return controller.collisions.below; } }

    private void OnEnable()
    {
        ControllerInput.partnerEvent += ControllerInput_partnerEvent;
    }

    private void ControllerInput_partnerEvent()
    {
        CharacterHolder.Instance.SwichBackToOriginalPlayer();
        var obj = Instantiate(partnerItem, transform.position, Quaternion.identity);
        GameManager.Instance.partnerTempItem = obj.gameObject;
        obj.transform.localScale = transform.localScale; ;
    }

    private void OnDisable()
    {
        ControllerInput.partnerEvent -= ControllerInput_partnerEvent;


        if (GameManager.Instance.partnerTempItem != null)
            Destroy(GameManager.Instance.partnerTempItem);
    }

    void Awake()
    {
        ropeGrabFoward = GetComponent<RopeGrabFowardPlayer>();
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        ghostSprite = GetComponent<GhostSprites>();
        Health = maxHealth;
        controller = GetComponent<Controller2D>();      //get the controller
        anim = GetComponent<Animator>();        //get the animator
        playerCheckLadderZone = GetComponent<PlayerCheckLadderZone>();
        SetupParameter();       //setup the parameters: speed, jump,...

        isPlaying = false;      //not allow the player woring at the beginning
        
        pushPullObj = GetComponentInChildren<PushPullObject>();
        allowMoving = true;

        walkSoundSrc = gameObject.AddComponent<AudioSource>();
        walkSoundSrc.clip = walkSound;
        walkSoundSrc.pitch = walkSoundSpeed;
        walkSoundSrc.Play();
        walkSoundSrc.loop = true;
        walkSoundSrc.volume = 0;
    }

    void Start()
    {
        isFacingRight = transform.localScale.x > 0;     //check which direction the player are facing?

        numberOfJumpLeft = numberOfJumpMax;     //the number of jumping

        rangeAttack = GetComponent<RangeAttack>();      //get the RangeAttack and MeleeAttack scripts
        meleeAttack = GetComponent<MeleeAttack>();

        GameManager.Instance.lastJumpPos = transform.position;
        GameManager.Instance.AddListener(this);
    }

    [ReadOnly] public bool isInTheCannon = false;
    bool isCannonFiring = false;

    public void GetInCannon(bool getIn)
    {
        StopMove();
        isInTheCannon = getIn;
        imageCharacterSprite.enabled = !getIn;
        isCannonFiring = !getIn;
    }

    private bool CheckRopeGrabFoward()
    {
        if (ropeGrabFoward.isAvailable())
        {
            ropeGrabFoward.GrabRopeAction();
            isCannonFiring = false;

            return true;
        }

        if (ropeGrabFoward.isWorking)
            return true;

        return false;
    }

    protected PlayerParameter overrideZoneParameter = null; //null mean no override
    protected bool useOverrideParameter = false;

    public void SetOverrideParameter(PlayerParameter _override, bool isUsed, PlayerState _zone = PlayerState.Ground)
    {
        overrideZoneParameter = _override;
        useOverrideParameter = isUsed;
        PlayerState = _zone;
    }

    public void InitGodmode(GodmodeType _type, float useTime, float damage)
    {
        return;
    }

    Transform ropePoint;
    public Transform GrabRopePoint;
    public bool isGrabRopePoint { get; set; }
    float catchRopePointRate = 0.3f;
    float lastTimeGrabRopePoint;

    public void CatchRopePoint(Transform _ropePoint)
    {
        if (Time.time < lastTimeGrabRopePoint + catchRopePointRate)
            return;

        ropePoint = _ropePoint;
        isGrabRopePoint = true;
    }

    public void SetupParameter()
    {
        PlayerParameter _tempParameter;

        switch (PlayerState)
        {
            case PlayerState.Ground:
                _tempParameter = GroundParameter;
                break;
            case PlayerState.Water:
                _tempParameter = WaterZoneParameter;
                break;
            default:
                _tempParameter = GroundParameter;
                break;
        }

        if (useOverrideParameter)
            _tempParameter = overrideZoneParameter;

        moveSpeed = _tempParameter.moveSpeed;
        maxJumpHeight = _tempParameter.maxJumpHeight;
        minJumpHeight = _tempParameter.minJumpHeight;
        timeToJumpApex = _tempParameter.timeToJumpApex;

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        originalGravity = gravity;

        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

        if (PlayerState == PlayerState.Ground)
            velocity.y = minJumpVelocity;
        else
            velocity.y *= 0.1f;     //descrease the gravity

        if (!isPlaying)
            velocity.y = 0;
    }
    [HideInInspector]
    public float mulSpeedc = 1;

    float XXspeed, XXtime;
    public void SpeedBoost(float Xspeed, float time, bool allowEffect)
    {
        XXspeed = Xspeed;
        XXtime = time;
        StartCoroutine(SpeedBoostCo(allowEffect));
    }

    IEnumerator SpeedBoostCo(bool allowEffect)
    {
        mulSpeedc = XXspeed;
        isBoostSpeed = true;
        forceShadowFX = allowEffect;
        if (ghostSprite)
            ghostSprite.allowGhost = allowEffect;

        yield return new WaitForSeconds(XXtime);
        mulSpeedc = 1;
        isBoostSpeed = false;
    }

    bool isHoldJump = false;
    public bool forceGhostFX { get; set; }

    void Update()
    {
        if (isFrozen || isInTheCannon)
            return;

        if (isPlaying)
            imageCharacterSprite.enabled = (!isInCannon);


        if (playerCheckLadderZone.fallingOffFromLadder)
        {
            var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
            if (hits.Length == 0)
            {
                playerCheckLadderZone.fallingOffFromLadder = false;
                controller.HandlePhysic = true;
            }
        }
        HandleAnimation();      //set the animation state for the Player

        if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
        {
            controller.collisionMask = controller.collisionMask & ~(1 << 10);
            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
            if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
            {
                imageCharacterSprite.sortingLayerName = "HidingZone";
                imageCharacterSprite.sortingOrder = 1;
            }
        }
        else
        {
            imageCharacterSprite.sortingLayerName = "Player";
            imageCharacterSprite.sortingOrder = 1;
  
                if ((goThroughEnemy) || isBlinking)
                {
                    
                        if (isBlinking)
                        {
                            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
                            if (controller.collisions.below)
                                controller.collisionMask = controller.collisionMask & ~(1 << LayerMask.NameToLayer("Enemy"));
                            else
                                controller.collisionMask = controller.collisionMask | (1 << LayerMask.NameToLayer("Enemy"));
                        }
                        else
                        {
                            controller.collisionMask = controller.collisionMask & ~(1 << LayerMask.NameToLayer("Enemy"));
                            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
                        }

                        if ((goThroughEnemy) || isBlinking)
                            gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
                        else
                            gameObject.layer = LayerMask.NameToLayer("Player");
                    
                }
                else
                {
                    controller.collisionMask = controller.collisionMask | (1 << 10);
                    controller.collisionMaskHorizontal = controller.collisionMask;
                    gameObject.layer = LayerMask.NameToLayer("Player");
                }
        }

        wallDirX = controller.collisions.faceDir;       //get the current direction

        float targetVelocityX = input.x *  moveSpeed * mulSpeedc;     //get the current moving state, Walk or Run?

        accGrounedOverride = 0;
        SurfaceModifier isUseSurfaceMod = null;
        bool useOverrideAcc = false;
        //Check ICE platform
        if (controller.collisions.ClosestHit)
        {
            isUseSurfaceMod = controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>();
            if (isUseSurfaceMod && isUseSurfaceMod.Friction > 0 && isUseSurfaceMod.Friction < 1)
            {
                accGrounedOverride = 1f / isUseSurfaceMod.Friction;
                useOverrideAcc = true;
            }
        }

        if (!isCannonFiring)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below) ? (useOverrideAcc ? accGrounedOverride : accelerationTimeGrounded) : accelerationTimeAirborne);       //make the smooth movement
        }

        if (controller.collisions.ClosestHit && controller.collisions.below)
        {
            isUseSurfaceMod = controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>();
            if (isUseSurfaceMod && isUseSurfaceMod.Friction > 1)
            {
                velocity.x /= isUseSurfaceMod.Friction;
            }
        }

        if (isPlaying)
            CheckWall();


        if (playerCheckLadderZone.isClimbing)
        {
            velocity.x = 0;

            if (velocity.y > 0 && !playerCheckLadderZone.checkLadderWithPoint(controller.boxcollider.bounds.center))
            {
                playerCheckLadderZone.isClimbing = false;
                velocity.y = minJumpVelocity;

            }
        }

        velocity.y += gravity * Time.deltaTime;

        if (isClimbOnTop)
            velocity.y = 0;

        if (controller.collisions.below && !isPlayedLandSound)
        {        //check to play land sound
            isPlayedLandSound = true;
            isCannonFiring = false;

            playerCheckLadderZone.isClimbing = false;
            isJumpPropActive = false;
            SoundManager.PlaySfx(landSound, landSoundVolume);

            //check enemy below
            var contactEventObj = (IPlayerContactEvent)controller.collisions.ClosestHit.collider.gameObject.GetComponent(typeof(IPlayerContactEvent));
            if (contactEventObj != null)
            {
                contactEventObj.OnPlayerContact(CONTACT_POS.Above, transform.position);
                AddForce(Vector2.up * minJumpVelocity * 1.5f);
            }
        }
        else if (!controller.collisions.below && isPlayedLandSound)
        {
            if (Mathf.Abs(velocity.y) > 0.5f)
                isPlayedLandSound = false;
        }

        CheckContactEnemy();

        if (isBoostSpeed)
        {
            ghostSprite.allowGhost = forceShadowFX && !isInCannon;
        }
        else
        {
            ghostSprite.allowGhost = forceGhostFX;
        }


        walkSoundSrc.volume = GlobalValue.isSound ? ((Mathf.Abs(velocity.x) > 0.1f && !isRunning && controller.collisions.below) ? walkSoundVolume : 0) : 0;
    }

    void CheckContactEnemy()
    {
        var hit = Physics2D.BoxCast(controller.boxcollider.bounds.center, controller.boxcollider.bounds.size * 0.9f, 0, Vector2.zero, 0, contactLayer);
        if (hit)
        {
            var contactEventObj = (IPlayerContactEvent)hit.collider.gameObject.GetComponent(typeof(IPlayerContactEvent));
            if (contactEventObj != null)
            {
                if (transform.position.x > hit.collider.gameObject.transform.position.x)
                    contactEventObj.OnPlayerContact(CONTACT_POS.Right, transform.position);
                else if (transform.position.x < hit.collider.gameObject.transform.position.x)
                    contactEventObj.OnPlayerContact(CONTACT_POS.Left, transform.position);
            }
        }
    }


    void StopClimbLadderCo()
    {
        velocity.y = 0f;
    }

    public void SetEmissionRate(ParticleSystem particleSystem, float emissionRate)
    {
        var emission = particleSystem.emission;
        var rate = emission.rate;
        rate.constantMax = emissionRate;
        emission.rate = rate;
    }

    bool allowGrabNextWall = true;

    bool allowCheckWall = true;

    void AllowCheckWall()
    {
        allowCheckWall = true;
    }

    void CheckWall()
    {
        wallSliding = false;
    }

    public bool isRoping { get; set; }
    public bool isHaningRope { get; set; }

    [HideInInspector]
    public bool inverseGravity = false;

    void LateUpdate()
    {
        if (isFrozen || isInTheCannon)
            return;

        if (GameManager.Instance.State == GameManager.GameState.GameOver)
            return;

        CheckClimbOnTop();

        if (!isPlaying || GameManager.Instance.State != GameManager.GameState.Playing)
            velocity.x = 0;     //stop right here if the game is not in playing mode

        if (isInCannon)
        {
            velocity = Vector2.zero;
        }

        if (isRoping || isHaningRope || isGrabRopePoint)
            velocity = Vector2.zero;

        if (!allowMoving || wallSliding)
            velocity.x = 0;

        if ((controller.raycastOrigins.bottomLeft.x < CameraFollow.Instance._min.x && velocity.x < 0) || (controller.raycastOrigins.bottomRight.x > CameraFollow.Instance._max.x && velocity.x > 0))
            velocity.x = 0;

        if (controller.raycastOrigins.topRight.y > CameraFollow.Instance._max.y && velocity.y > 0)
            velocity.y = 0;

        if (controller.raycastOrigins.topRight.y < CameraFollow.Instance._min.y && velocity.y < 0)
            LevelManager.Instance.KillPlayer();

        Vector3 finalVelocity = velocity;
        Vector3 finalInput = input;

        if (inverseGravity)
        {
            finalVelocity.y *= -1;
            finalInput.y *= -1;
        }

        if (isDragging)
        {
            if ((currentDragObj.GetComponent<BoxSetup>().BoxHitRight() && input.x == 1) || (currentDragObj.GetComponent<BoxSetup>().BoxHitLeft() && input.x == -1))
            {
                velocity.x = 0;
                finalVelocity.x = 0;
            }

            if ((isFacingRight && input.x == 1) || (!isFacingRight && input.x == -1))
                ;
            else
            {
                RaycastHit2D checkHitWall = Physics2D.Raycast(transform.position, (isFacingRight ? Vector2.left : Vector2.right), 0.7f, controller.collisionMask);
                if (checkHitWall)
                {
                    velocity.x = 0;
                    finalVelocity.x = 0;
                }
            }


            currentDragObj.GetComponent<BoxSetup>().MoveBox(finalVelocity * 0.5f, finalInput);
        }

        controller.Move(finalVelocity * Time.deltaTime * (isDragging ? 0.5f : 1), finalInput, false, true);      //move the player

        if (isGrabRopePoint)
        {
            transform.position = ropePoint.position - (GrabRopePoint.position - transform.position);
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;     //stop the gravity when the player colide with the Top/Bottom 
        }

        if (controller.collisions.above)
        {
            CheckBox();     //check if the brick on the head
        }

        if (controller.collisions.below && velocity.y <= 0)
        {
            allowClimpOnTopAgain = true;     //allow the player h		old the pipe or not depend on the allowClimbOnTop value
           

            if (isPlaying)
            {
                CheckSpring();      //check the Spring on the feet 
             
                CheckBridge();
            }
        }

        if (wallSliding && (controller.collisions.left || controller.collisions.right))
            CheckBridge();
    }

    private void CheckBridge()
    {
        if (controller.collisions.ClosestHit.collider == null)
            return;
        var bridge = controller.collisions.ClosestHit.collider.gameObject.GetComponent<Bridge>();
        if (bridge)
        {
            bridge.Work();
        }
    }
    
    //check the brick when the player jump, if there are the brick then break it
    private void CheckBox()
    {
        CheckBlockBrick();

        if (controller.collisions.ClosestHit.collider.gameObject.GetComponent<CanBeHitByPlayerHead>())
        {
            var takeDamage = (ICanTakeDamage)controller.collisions.ClosestHit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                takeDamage.TakeDamage(int.MaxValue, Vector2.zero, gameObject, transform.position);
            }
        }
    }

    void CheckBlockBrick()
    {
        Block isBlock;
        var bound = controller.boxcollider.bounds;

        //check middle
        var hit = Physics2D.Raycast(new Vector2((bound.min.x + bound.max.x) / 2f, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Brick"));

        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }

        //check left
        hit = Physics2D.Raycast(new Vector2(bound.min.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Brick"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }

        hit = Physics2D.Raycast(new Vector2(bound.max.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Brick"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }
    }

    //check if the top of player collide with the Pipe, if yes then allow it hold the pipe and moving
    private void CheckClimbOnTop()
    {
        if (isClimbOnTop)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 2, controller.collisionMask);
            if (hit)
                isClimbOnTop = hit.collider.CompareTag("ClimbOnTop");
            else
                isClimbOnTop = false;
        }

        if (controller.collisions.above && allowClimpOnTopAgain)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 2, controller.collisionMask);
            if (hit)
            {

                if (hit.collider.CompareTag("ClimbOnTop"))
                {
                    isClimbOnTop = true;
                    allowClimpOnTopAgain = false;
                }
            }
        }
    }

    //check if the player stand on the Spring, if yes then push the player up with the given value height from the Spring script
    private void CheckSpring()
    {
        if (controller.collisions.ClosestHit)
        {
            var spring = controller.collisions.ClosestHit.collider.GetComponent<Spring>();
            if (spring)
            {
                velocity.y = Mathf.Abs((2 * spring.pushHeight) / Mathf.Pow(timeToJumpApex, 2)) * timeToJumpApex;
                spring.Push();
            }
        }
    }

    //Flip the play to facing Right or Left base on its direction and the X velocity
    public void Flip()
    {
        if (wallSliding)
            return;

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void DragBegin()
    {
        isDragging = true;
        Debug.Log("CC");
        //move player back a little bit
        if (pushPullObj.hit)
            currentDragObj = pushPullObj.hit.transform.gameObject;
    }

    public void DragStop()
    {
        isDragging = false;
        if (currentDragObj)
            currentDragObj.transform.parent = null;
    }

    //This action is called by the Input/ControllerInput
    public void MoveLeft()
    {
        if (isPlaying)
        {
            if ((playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>()) || playerCheckLadderZone.fallingOffFromLadder)
            {
                var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
                if (hits.Length > 0)
                    return;
            }
            else if (playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
            {
                velocity.x = -playerCheckLadderZone.climbSpeed;
            }

            input = new Vector2(-1, 0);

            if (isDragging)
                return;

            if (isHaningRope)
            {
                if (transform.root.transform.localScale.x < 0 && transform.localScale.x < 0)
                    Flip();
                else if (transform.root.transform.localScale.x > 0 && isFacingRight)
                    Flip();
            }

            else
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    public void MoveRight()
    {
        if (isPlaying)
        {
            if ((playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>()) || playerCheckLadderZone.fallingOffFromLadder)
            {
                var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
                if (hits.Length > 0)
                    return;
            }
            else if (playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
            {
                velocity.x = playerCheckLadderZone.climbSpeed;
            }
            input = new Vector2(1, 0);

            //don't allow flip character if dragging
            if (isDragging)
                return;

            if (isHaningRope)
            {
                if (transform.root.transform.localScale.x < 0 && transform.localScale.x > 0)
                    Flip();
                else if (transform.root.transform.localScale.x > 0 && !isFacingRight)
                    Flip();
            }

            else if (!isFacingRight)
            {
                Flip();
            }
        }
    }

    //This action is called by the Input/ControllerInput
    public void MoveUp()
    {
        if (isPlaying && playerCheckLadderZone.isInLadderZone)
        {
            if (isGrounded && !playerCheckLadderZone.isLadderAbove)
                return;

            transform.position = new Vector2(playerCheckLadderZone.currentLadder.transform.position.x, transform.position.y);
            playerCheckLadderZone.isClimbing = true;

            velocity.y = playerCheckLadderZone.climbSpeed;
        }

        isClimbOnTop = false;

        input = new Vector2(0, 1);
    }

    //This action is called by the Input/ControllerInput
    public void MoveDown()
    {

        if (isPlaying && playerCheckLadderZone.isInLadderZone)
        {
            if (isGrounded && !playerCheckLadderZone.isLadderBelow)
            {
                input = new Vector2(0, -1);
                return;
            }

            transform.position = new Vector2(playerCheckLadderZone.currentLadder.transform.position.x, transform.position.y);
            playerCheckLadderZone.isClimbing = true;
            velocity.y = -playerCheckLadderZone.climbSpeed;
        }

        isClimbOnTop = false;

        input = new Vector2(0, -1);
    }

    //This action is called by the Input/ControllerInput
    public void StopLadder()
    {
        if (!isPlaying)
            return;

        if (!isGrounded)
        {
            velocity.y = 0;
            velocity.x = 0;
        }
    }

    //This action is called by the Input/ControllerInput
    public void StopMove()
    {
        input = Vector2.zero;

        if (!isPlaying)
            return;

        if (playerCheckLadderZone.isClimbing)
        {
            velocity.y = 0;
            velocity.x = 0;
        }
    }

    public void Run()
    {
        if (isDragging)
            return;
    }

    public void Forzen(bool is_enable)
    {
        isFrozen = is_enable;
        anim.enabled = !is_enable;
    }

    public void StopRunning()
    {

    }

    //allow recharge the running enegry after 1s
    IEnumerator RunningBreak()
    {
        yield return new WaitForSeconds(1);
    }

    Vector2 oriSize;
    Vector2 oriOffet;
    public bool isSlidingInTurnel { get; set; }     //mean have a platform on player's head when he sliding
    //This action is called by the Input/ControllerInput, slide the player with the parameters can be set in the Hierarchy

    public void Slide()
    {

    }

    private void SlideOff()
    {

    }

    bool HitWallOnHeadWhenSlide()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1, controller.collisionMask);
        return hit ? true : false;
    }

    //This action is called by the Input/ControllerInput
    public void UseJetpack()
    {

    }

    //This action is called by the Input/ControllerInput
    public void StopUseJetpack()
    {
     
    }

    public void Jump()
    {
        //Debug.LogError(input);
        isHoldJump = true;
        allowGrabNextWall = true;

        Debug.Log("Jump");
        if (isInStayZone)
        {
            Flip();
            input.x *= -1;      //change the direction
            isPlaying = true;
            isInStayZone = false;
            return;
        }

        if (!isPlaying)
            return;

        if (isSlidingInTurnel)
            return;

        if (isJumpPropActive)
            return;

        if (ropeGrabFoward && CheckRopeGrabFoward())
            return;


        if (isRoping || isHaningRope)
        {
            ExitTheRope();
            RopeUI.instance.ExitRope();
        }

        if (isGrabRopePoint)
        {
            isGrabRopePoint = false;
            numberOfJumpLeft = numberOfJumpMax;
            lastTimeGrabRopePoint = Time.time;
        }

        //do not allow the player jump when the player are climbing the ladder and are in the the ground
        if (playerCheckLadderZone.isClimbing)
        {
            //check if can jump off from Ladder
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1.25f, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Ground"));

            if (!hit && GameManager.Instance.controllerInput.isHoldingRight)
            {
                playerCheckLadderZone.fallingOffFromLadder = true;
                playerCheckLadderZone.isInLadderZone = false;
                playerCheckLadderZone.currentLadder = null;
                playerCheckLadderZone.isClimbing = false;
                input.x = 1;

                if (!isFacingRight)
                    Flip();

                velocity.y = maxJumpVelocity;
                velocity.x = moveSpeed;
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
                numberOfJumpLeft = numberOfJumpMax;
                GameManager.Instance.lastJumpPos = transform.position;
            }
            else if (!hit && GameManager.Instance.controllerInput.isHoldingLeft)
            {
                playerCheckLadderZone.fallingOffFromLadder = true;
                playerCheckLadderZone.isInLadderZone = false;
                playerCheckLadderZone.currentLadder = null;
                playerCheckLadderZone.isClimbing = false;
                input.x = -1;

                if (isFacingRight)
                    Flip();

                velocity.y = maxJumpVelocity;
                velocity.x = -moveSpeed;
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
                numberOfJumpLeft = numberOfJumpMax;
                GameManager.Instance.lastJumpPos = transform.position;
            }
            else
            {
                JumpOffFromLadder();
                return;
            }
        }
        if (controller.collisions.below && PlayerState == PlayerState.Ground)
        {
            velocity.y = maxJumpVelocity;

            if (JumpEffect != null)
                Instantiate(JumpEffect, transform.position, transform.rotation);
            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            numberOfJumpLeft = numberOfJumpMax;
            GameManager.Instance.lastJumpPos = transform.position;
        }
        else if (PlayerState == PlayerState.Water)
        {
            velocity.y = minJumpVelocity;
            SoundManager.PlaySfx(waterIN, 0.25f);
        }
        else
        {
            numberOfJumpLeft--;
            if (numberOfJumpLeft > 0)
            {
                velocity.y = minJumpVelocity;

                anim.SetTrigger("doubleJump");

                if (JumpEffect != null)
                    Instantiate(JumpEffect, transform.position, transform.rotation);
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            }
        }
        isClimbOnTop = false;
    }

    //This action is called by the Input/ControllerInput
    public void JumpOff()
    {
        isHoldJump = false;

        if (!isPlaying || isJumpPropActive)
            return;
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    //This action is called by the Input/ControllerInput
    public void MeleeAttack()
    {
        if (!isPlaying || isClimbOnTop || isRoping || isHaningRope || isGrabRopePoint || isSlidingInTurnel)
            return;

        if (meleeAttack != null)
        {
            if (meleeAttack.CanAttack())
            {
                if (wallSliding)
                {
                    anim.SetTrigger("wallAttack");
                }
                else
                {
                    anim.SetTrigger("meleeAttack");
                }
            }
        }
    }

    //This action is called by the Input/ControllerInput
    //buttonUp means allow fire
    public void RangeAttack(bool powerBullet)
    {
        if (GameManager.Instance.Player.isInCannon)
            return;
        if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
            return;
        if (GameManager.Instance.Player.isSliding)
            return;
        if (wallSliding)
            return;
        if (PlayerState == PlayerState.Water && !allowWaterZoneShoot)
            return;
        if ((isHaningRope) && !allowHangingRopeShoot)
            return;
        if ((isGrabRopePoint) && !allowRopeType2Shoot)
            return;
        if (isClimbOnTop && !allowPipeShoot)
            return;
        if (!isPlaying)
            return;

        if (rangeAttack != null)
        {
            if (rangeAttack.Fire(false))
            {
                SoundManager.PlaySfx(rangeAttack.shootSound);
            }
        }
    }

    public void CancelRangeHolding()
    {
        if (rangeAttack)
            rangeAttack.CancleHolding();
    }

    //This action is called by the Input/ControllerInput
    public void ThrowGrenade()
    {

    }

    public void SetForce(Vector2 force)
    {
        if (meleeAttack.DetectEnemies.activeInHierarchy)
            return;

        velocity = (Vector3)force;
    }

    public void AddForce(Vector2 force)
    {
        Debug.Log("AddForce");
        velocity += (Vector3)force;
    }

    public void AddHorizontalForce(float _speed)
    {
        //if (controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>())
            transform.Translate(_speed * Time.deltaTime, 0, 0, Space.Self);
    }

    //Called by Gamemanager script if the player is dead and there are lives
    public void RespawnAt(Vector2 pos, int checkpointDir)
    {
        Health = maxHealth;
        transform.position = new Vector3(pos.x, pos.y + 1, transform.position.z);
        gameObject.SetActive(true);

        isClimbOnTop = false;

        ResetAnimation();

        StartCoroutine(BlinkEffecrCo());

        transform.localScale = new Vector3(checkpointDir, transform.localScale.y, transform.localScale.z);
        isFacingRight = transform.localScale.x > 0;


        StartCoroutine(RestartGameCo());
    }

    //only allow the player playing after 1s
    IEnumerator RestartGameCo()
    {

        yield return new WaitForSeconds(1);
        isPlaying = true;
    }

    public void SetFinishAnim()
    {
        AnimSetTrigger("showUp");
    }

    public void AnimSetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void AnimSetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    //Called every frame in the Update()
    void HandleAnimation()
    {
        anim.SetFloat("speed", Mathf.Abs(velocity.x));
        anim.SetFloat("height_speed", velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWall", wallSliding);
        anim.SetBool("isInWater", PlayerState == PlayerState.Water);
        anim.SetBool("isClimbLadder", playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>());
        playerCheckLadderZone.isClimbingLadder8Dir = playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>();
        anim.SetBool("isClimbing8Ladder", playerCheckLadderZone.isClimbingLadder8Dir);
        anim.SetBool("running", isRunning);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isClimbOnTop", isClimbOnTop);
        anim.SetBool("hangingRope", isRoping || isHaningRope || isGrabRopePoint);
        //anim.SetBool("isGrabLedge", isGrabbingLedge);
        anim.SetBool("isDragging", isDragging);
        anim.SetInteger("inputX", (int)input.x);
        anim.SetInteger("inputY", (int)input.y);
    }

    //To make the player change to the Climb on top state, then it must be disactive the Animator after the moment
    IEnumerator DisableAnimatorCo()
    {
        yield return new WaitForSeconds(0.1f);
        anim.enabled = input.x != 0 || anim.GetBool("range_attack") || anim.GetBool("throw") || anim.GetBool("specialPower");
    }

    //Reset the animation state when the player is respawned
    void ResetAnimation()
    {
        if (anim != null)
        {
            anim.ResetTrigger("idleFront");
            anim.ResetTrigger("idleBack");
            anim.SetFloat("speed", 0);
            anim.SetFloat("height_speed", 0);
            anim.SetBool("isGrounded", true);
            anim.SetBool("isWall", false);
            anim.SetTrigger("reset");
            anim.SetBool("hangingRope", false);
        }
    }

    public void TakeDamageFromContactEnemy(float damage, Vector2 force, GameObject instigator, bool ignorePlayerThroughEnemy = false)
    {
        if (goThroughEnemy && !ignorePlayerThroughEnemy)
            return;

        TakeDamage(damage, force, instigator, transform.position);

        SetForce(force);
    }


    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        bool ignoreBlinking = false;

        if (!isPlaying || (isBlinking && !ignoreBlinking) || GodMode || isInCannon)
            return;

        if (GameManager.Instance.isUsingShield)
        {
            FindObjectOfType<Shield>().Hit(instigator);
            StartCoroutine(BlinkEffecrCo());
            return;
        }

        SoundManager.PlaySfx(hurtSound, hurtSoundVolume);
        if (HurtEffect != null)
            Instantiate(HurtEffect, hitPoint, Quaternion.identity);

        Health -= (int)damage;

        if (Health <= 0)
        {
            if (playerWithPartner)
                CharacterHolder.Instance.SwichBackToOriginalPlayer();
            //ControllerInput_partnerEvent();
            else
            LevelManager.Instance.KillPlayer();

            return;
        }

        if (!playerCheckLadderZone.isClimbing)
        {
            anim.SetTrigger("hurt");        //set the animation to hurt state
            //set force to player, push the player back with the current direction
            if (knockBackBeHit && damage >= damageKnockBackBeHit)
            {
                if (instigator != null)
                {
                    int dirKnockBack = (instigator.transform.position.x > transform.position.x) ? -1 : 1;
                    SetForce(new Vector2(knockbackForce * dirKnockBack, 0));
                }
            }

            if (!ignoreBlinking)
                DoBlinking();        //begin the blink effect
        }
        else
            JumpOffFromLadder();

        if (bloodScreenFXHit)
            BloodScreenUI.instance.Work();
    }

    public void DoBlinking()
    {
        StartCoroutine(BlinkEffecrCo());
    }

    //Do the blink effect with the blinking timer
    IEnumerator BlinkEffecrCo()
    {
        isBlinking = true;
        int blink = (int)(blinking * 0.5f / 0.2f);
        for (int i = 0; i < blink; i++)
        {
            imageCharacterSprite.color = blinkColor;
            yield return new WaitForSeconds(0.2f);
            imageCharacterSprite.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }

        imageCharacterSprite.color = Color.white;
        isBlinking = false;
    }

    //Called by GiveHealth script item after collect it
    public void GiveHealth(int hearthToGive, GameObject instigator)
    {
        Health = Mathf.Min(Health + hearthToGive, maxHealth);
    }
    public GameObject deadFX;
    //Call by Level Manager
    public void Kill()
    {
        if (isPlaying)
        {
            anim.enabled = true;
            isPlaying = false;
            StopMove();
            anim.SetTrigger("dead");
            anim.SetBool("doorHideInOut", false);
            Health = 0;
            StopUseJetpack();
            Debug.Log("DEAD");
            isRoping = false;
            isHaningRope = false;
            isClimbOnTop = false;
            isGrabRopePoint = false;
            forceGhostFX = false;
            ghostSprite.allowGhost = false;
            velocity = Vector2.zero;

            if (GameManager.Instance.isUsingShield)
            {
                FindObjectOfType<Shield>().StopShield();
            }

            SoundManager.PlaySfx(deadSound, deadSoundVolume);
        }
        else if (isUsingPowerBombAction)
        {
            anim.SetTrigger("dead");
        }
    }

    void WaterIn()
    {
        if (waterFX)
            Instantiate(waterFX, transform.position, Quaternion.identity);

        SoundManager.PlaySfx(waterIN);
        PlayerState = PlayerState.Water;
        StopRunning();
        SetupParameter();
    }

    bool isWiding = false;
    bool isInJumpWallZone = false;

    /// <summary>
    /// JUMP FALL OFF FROM LADDER
    /// </summary>
    void JumpOffFromLadder()
    {
        playerCheckLadderZone.fallingOffFromLadder = true;
        controller.HandlePhysic = false;
        playerCheckLadderZone.isInLadderZone = false;
        playerCheckLadderZone.currentLadder = null;
        playerCheckLadderZone.isClimbing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPlaying || isInTheCannon)
            return;

        if (other.CompareTag("Water"))
        {
            if (PlayerState != PlayerState.Water)
            {
                WaterIn();
            }
        }

        var isTriggerEvent = other.GetComponent<TriggerEvent>();
        if (isTriggerEvent != null)
            isTriggerEvent.OnContactPlayer();

        var isTrigger = (ITriggerPlayer)other.GetComponent(typeof(ITriggerPlayer));
        if (isTrigger != null)
            isTrigger.OnTrigger();

        if (!isPlaying)
            return;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<WindPhysic2D>())
        {
            gravity = other.GetComponent<WindPhysic2D>().Gravity;
            isWiding = true;
        }
        else if (other.CompareTag("Water"))
        {
            if (PlayerState != PlayerState.Water)
            {
                WaterIn();
            }
        }
        else if (!playerWithPartner && (other.CompareTag("SpikeZone") || other.CompareTag("KillZone")))
        {
            LevelManager.Instance.KillPlayer();
        }else if (playerWithPartner && other.CompareTag("KillZone"))
        {
            //LevelManager.Instance.KillPlayer();
            CharacterHolder.Instance.SwichBackToOriginalPlayer();
        }

        if (other.CompareTag("ContinueJumpWall"))
        {
            isInJumpWallZone = true;
        }
    }

    //Detect when the player move out of the water
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (waterFX && isPlaying)
            {
                Instantiate(waterFX, transform.position, Quaternion.identity);
                SoundManager.PlaySfx(waterOUT);
            }
            PlayerState = PlayerState.Ground;
            SetupParameter();
        }

        if (other.GetComponent<WindPhysic2D>())
        {
            gravity = originalGravity;
            isWiding = false;
        }

        if (other.CompareTag("ContinueJumpWall"))
        {
            isInJumpWallZone = false;
        }
    }

    /// <summary>
    /// Teleport Effect between 2 points
    /// Called by the Teleport script
    /// </summary>

    public void Teleport(Transform newPos, float timer, float smoothTranparent)
    {
        StartCoroutine(TeleportCo(newPos, timer, smoothTranparent));
    }


    IEnumerator TeleportCo(Transform newPos, float timer, float smoothTranparent)
    {
        StopMove();
        isPlaying = false;
        Color color = imageCharacterSprite.color;

        float delay = timer / smoothTranparent;
        float smooth = 1f / smoothTranparent;
        for (int j = 0; j < smoothTranparent; j++)
        {
            color.a = Mathf.Clamp01(color.a - smooth);
            imageCharacterSprite.color = color;
            yield return new WaitForSeconds(delay);
        }

        transform.position = newPos.position;

        for (int j = 0; j < smoothTranparent; j++)
        {
            color.a = Mathf.Clamp01(color.a + smooth);
            imageCharacterSprite.color = color;
            yield return new WaitForSeconds(delay);
        }

        color.a = 1;
        imageCharacterSprite.color = Color.white;

        isPlaying = true;
    }

    public void GoDoor(Transform newPos, float timer)
    {
        StartCoroutine(GoDoorCo(newPos, timer));
    }


    IEnumerator GoDoorCo(Transform newPos, float timer)
    {
        StopMove();
        BlackScreenUI.instance.Show(timer);
        MenuManager.Instance.HideController();

        yield return new WaitForSeconds(timer);

        anim.SetTrigger("idleFront");
        transform.position = newPos.position;
        BlackScreenUI.instance.Hide(timer);
        yield return new WaitForSeconds(timer);
        MenuManager.Instance.ShowController();
    }

    void OnBecameInvisible()
    {
    }

    //Use the IListener script to get the state of the game via GameManager
    #region IListener implementation

    public void IPlay()
    {

    }

    public void ISuccess()
    {
        StopMove();		//
        if(walkSoundSrc)
        walkSoundSrc.Stop();
        isPlaying = false;
        if (anim)
            anim.SetTrigger("finish");
        SoundManager.PlaySfx(finishSound);
    }

    public void IPause()
    {
        StopMove();
    }

    public void IUnPause()
    {

    }

    public void IGameOver()
    {
        if (this)
        {
            mulSpeedc = 1;
            isBoostSpeed = false;
            allowCheckWall = true;

        }
    }

    public void IOnRespawn()
    {
        if (this)
        {
            gameObject.SetActive(true);
            imageCharacterSprite.enabled = true;
            isRoping = false;
            isGrabRopePoint = false;
            isHaningRope = false;
            transform.parent = null;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            anim.speed = 1;
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
           
            imageCharacterSprite.color = Color.white;
            GameManager.Instance.Player.gameObject.layer = LayerMask.NameToLayer("Player");
            if (isFrozen)
            {
                Forzen(false);
            }
        }
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }
    #endregion

    public void ExitTheRope()
    {
        isRoping = false;
        isHaningRope = false;
        transform.parent = null;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Dogge this instance.
    /// </summary>
    /// 
    bool enterGravityZoneTemp = false;
    public void EnterGravityZone()
    {
        enterGravityZoneTemp = true;
        GameManager.Instance.Player.inverseGravity = true;
        GameManager.Instance.Player.controller.inverseGravity = true;
        GameManager.Instance.Player.transform.localScale
        = new Vector3(GameManager.Instance.Player.transform.localScale.x, -Mathf.Abs(GameManager.Instance.Player.transform.localScale.y), GameManager.Instance.Player.transform.localScale.z);
    }


    public void ExitGravityZone()
    {
        GameManager.Instance.Player.inverseGravity = false;
        GameManager.Instance.Player.controller.inverseGravity = false;

      
            GameManager.Instance.Player.transform.localScale
        = new Vector3(GameManager.Instance.Player.transform.localScale.x, Mathf.Abs(GameManager.Instance.Player.transform.localScale.y), GameManager.Instance.Player.transform.localScale.z);
        
    }

    public void PausePlayer(bool pause)
    {
        StopMove();
        isPlaying = !pause;
    }
}

[System.Serializable]
public class PlayerParameter
{
    public float moveSpeed = 3;
    public float maxJumpHeight = 3;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
}