using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ControllerInput : MonoBehaviour, IListener
{
	public delegate void FireCannonEvent();
	public static event FireCannonEvent fireCannonEvent;

	public delegate void PartnerEvent();
	public static event PartnerEvent partnerEvent;

	public static ControllerInput Instance;

	public delegate void eventUpButton();
	public static event eventUpButton pressUpButton;
	public Text bulletText;
	public CanvasGroup ActionButtons;
	public CanvasGroup jumpButtonsCG;
	public CanvasGroup directionCG;

	public GameObject btnDialogueNext;

	public Sprite iconRun, iconSlide;

	bool isMoveLeft = false;
	bool isMoveRight = false;
	ButtonActivated butActive;
	RopeUI rope;
	Cannon[] cannons;

	public GameObject DirectionControl;

	[Header("---BUTTONS---")]
	public GameObject btnCannonFire;
	public GameObject group1;
	public GameObject movingButtons;
	public GameObject partnerButton;

	public bool isTalkingGuy { get; set; }

	void Awake()
	{
		Instance = this;
	}

	void OnEnable()
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.controllerInput = this;
		}
		StopMove();
		Invoke("SetAgain", 0.1f);
	}

	void SetAgain()
	{
		butActive = FindObjectOfType<ButtonActivated>();
		rope = FindObjectOfType<RopeUI>();
		cannons = FindObjectsOfType<Cannon>();
	}

	void Start()
	{
		GameManager.Instance.controllerInput = this;
		butActive = FindObjectOfType<ButtonActivated>();
		rope = FindObjectOfType<RopeUI>();
		cannons = FindObjectsOfType<Cannon>();
	}

	void Update()
	{
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		bulletText.text = GlobalValue.normalBullet.ToString();

		if (isMoveLeft)
			GameManager.Instance.Player.MoveLeft();
		else if (isMoveRight)
			GameManager.Instance.Player.MoveRight();

		if (delayKeepRunning > 0)
			delayKeepRunning -= Time.deltaTime;

		if (ActionButtons.alpha == 0)
		{
			delayKeepRunning = 0;
			GameManager.Instance.Player.CancelRangeHolding();
		}

		//PC INPUT
		HandleInput();

		//GAMEPAD INPUT
		ActionButtons.alpha = (GameManager.Instance.Player.gameObject.layer == LayerMask.NameToLayer("HidingZone") || GameManager.Instance.isInDialogue) ? 0 : 1;
		ActionButtons.interactable = (GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone") && !GameManager.Instance.isInDialogue);
		ActionButtons.blocksRaycasts = (GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone")) && !GameManager.Instance.isInDialogue;

		directionCG.alpha = GameManager.Instance.isInDialogue ? 0 : 0.5f;

		btnDialogueNext.SetActive(GameManager.Instance.isInDialogue);


		if (ActionButtons.alpha == 0)
		{
			jumpButtonsCG.ignoreParentGroups = GameManager.Instance.Player.canJumpWhenHidingZone;
		}
		else
		{
			jumpButtonsCG.ignoreParentGroups = false;
		}

		group1.SetActive(!GameManager.Instance.Player.isInTheCannon);
		movingButtons.SetActive(!GameManager.Instance.Player.isInTheCannon);

		btnCannonFire.SetActive(GameManager.Instance.Player.isInTheCannon);
		partnerButton.SetActive(GameManager.Instance.Player.playerWithPartner);
	}

	public void MoveLeft(bool isTouch = false)
	{
		if (GameManager.Instance.isInDialogue || GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing && GameManager.Instance.Player.playerCheckLadderZone.currentLadder && !GameManager.Instance.Player.playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
		{
			isHoldingRight = false;
			isHoldingLeft = true;
			return;
		}

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{

			isMoveLeft = true;
			isMoveRight = false;
		}
	}

	float delayKeepRunning = 0;
	[HideInInspector]
	public bool isHoldingRight = false;
	[HideInInspector]
	public bool isHoldingLeft = false;
	public void MoveRight(bool isTouch = false)
	{
		if (GameManager.Instance.isInDialogue || GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing && GameManager.Instance.Player.playerCheckLadderZone.currentLadder && !GameManager.Instance.Player.playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
		{
			isHoldingRight = true;
			isHoldingLeft = false;
			return;
		}

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			isMoveLeft = false;
			isMoveRight = true;
		}
	}

	public void MoveUp()
	{
		if (GameManager.Instance.isInDialogue || GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.Player.isSliding)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.MoveUp();
			if (pressUpButton != null)
				pressUpButton();
		}
	}

	public void StopLadder()
	{
		//if (GameManager.Instance.Player.isSliding)
		//	return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.StopLadder();
			StopMove();
		}
	}

	public void MoveDown()
	{
		if (GameManager.Instance.isInDialogue || GameManager.Instance.Player.isInCannon || GameManager.Instance.Player.isClimbOnTop)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
			GameManager.Instance.Player.MoveDown();
	}

	public void StopMove(int _dir = 0)
	{
		if (_dir == 1)
			isHoldingRight = false;
		if (_dir == -1)
			isHoldingLeft = false;

		if (GameManager.Instance && GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.StopMove();
			delayKeepRunning = 0.2f;
		}

		if (_dir == 1)
			isMoveRight = false;
		if (_dir == -1)
			isMoveLeft = false;
	}

	public void StopMoveForTouch(int _dir)
	{
		if (GameManager.Instance && GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.StopMove();
			delayKeepRunning = 0.2f;
		}
		isMoveLeft = isMoveRight = false;
	}

	public void DialogueTapToNext()
	{
		if (GameManager.Instance.isInDialogue)
			butActive.TriggerButtonAction();
	}

	public void Jump()
	{
		Debug.Log(GameManager.Instance.Player.isInCannon);
		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			if (GameManager.Instance.Player.isInCannon)
			{
				Debug.Log("F");
				foreach (var obj in cannons)
					obj.FireCannon();
				return;
			}

			if (RopeUI.instance.CurrentRope)
			{
				if (GameManager.Instance.Player.isRoping || GameManager.Instance.Player.isHaningRope)
				{
					GameManager.Instance.Player.Jump();
				}
				else
				{
					CatchRope();
				}
			}
			else if (GameManager.Instance.isInDialogue)
			{
				return;
			}
			else
				GameManager.Instance.Player.Jump();
		}
	}

	public void JumpOff()
	{
		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.JumpOff();
		}
	}

	public void UseJetpack()
	{
		if (GameManager.Instance.Player.isInCannon)
			return;
		if (GameManager.Instance.Player.isSliding)
			return;
		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
			return;
		if (GameManager.Instance.Player.isGrabRopePoint || GameManager.Instance.Player.isHaningRope || GameManager.Instance.Player.isRoping)
			return;

		if (GameManager.Instance.Player.isClimbOnTop)
			return;
		if (GameManager.Instance.Player.wallSliding)
			return;
		//if (GameManager.Instance.Player.isGrabbingLedge)
		//	return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.UseJetpack();
		}
	}

	public void StopUseJetpack()
	{
		if (GameManager.Instance.Player.isSliding)
			return;
		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
			return;
		if (GameManager.Instance.Player.isGrabRopePoint || GameManager.Instance.Player.isHaningRope || GameManager.Instance.Player.isRoping)
			return;

		if (GameManager.Instance.Player.isClimbOnTop)
			return;
		if (GameManager.Instance.Player.wallSliding)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
			GameManager.Instance.Player.StopUseJetpack();
	}

	public void MeleeAttack()
	{
		if (GameManager.Instance.Player.isInCannon)
			return;
		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
			return;
		if (GameManager.Instance.Player.isSliding)
			return;
		if (GameManager.Instance.Player.wallSliding)
			return;
		if (GameManager.Instance.Player.isInCannon)
			return;
		if (GameManager.Instance.Player.PlayerState == PlayerState.Water && !GameManager.Instance.Player.allowWaterZoneShoot)
			return;
		if ((GameManager.Instance.Player.isHaningRope) && !GameManager.Instance.Player.allowHangingRopeShoot)
			return;
		if ((GameManager.Instance.Player.isGrabRopePoint) && !GameManager.Instance.Player.allowRopeType2Shoot)
			return;
		if (GameManager.Instance.Player.isClimbOnTop && !GameManager.Instance.Player.allowPipeShoot)
			return;
		if (GameManager.Instance.Player.isRoping)
			return;
		if (GameManager.Instance.Player.isHaningRope)
			return;
		if (GameManager.Instance.Player.isClimbOnTop)
			return;
		//if (GameManager.Instance.Player.isGrabbingLedge)
		//	return;
		if (GameManager.Instance.Player.isGrabRopePoint)
			return;

		GameManager.Instance.Player.MeleeAttack();
	}

	public void RangeAttackDown(bool powerBullet)
	{
		if (GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.RangeAttack(powerBullet);
		}
	}

	public void RangeAttackUp()
	{
		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			GameManager.Instance.Player.CancelRangeHolding();
		}
	}

	public void ThrowGrenade()
	{
		if (GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
			return;

		if (GameManager.Instance.Player.isSliding)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
			GameManager.Instance.Player.ThrowGrenade();
	}

	public void RunOrSlide()
	{
		if (GameManager.Instance.Player.isInCannon)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Playing)
		{
			if (!GameManager.Instance.Player.isRunning)
				GameManager.Instance.Player.Run();
			else
				GameManager.Instance.Player.Slide();
		}
	}

	private void CatchRope()
	{
		if (GameManager.Instance.Player.isInCannon)
			return;

		rope.Click();
	}

	public void ApplyKey()
	{
		;
	}

	//GAMEPAD
	bool gamepadMoveL = false;
	bool gamepadMoveR = false;
	float preX, preY;

	private void GamePadInput()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		//X
		if (x == 1 && x != preX)
		{
			MoveRight();
			preX = x;
		}
		else if (x == -1 && x != preX)
		{
			MoveLeft();
			preX = x;
		}
		else if (x == 0 && x != preX)
		{
			StopMove();
			preX = 0;
		}

		//Y
		if (y == 1 && y != preY)
		{
			MoveUp();
			preY = y;
		}
		else if (y == -1 && y != preY)
		{
			MoveDown();
			preY = y;
		}
		else if (y == 0)
		{
			if (preY == 1)
				StopMove();
			else if (preY == -1)
				StopLadder();
			preY = 0;
		}
		//Firing

		if (Input.GetButtonDown("B") || Input.GetKeyDown(DefaultValueKeyboard.Instance.Shooting))
		{
			AttackButtonDown();
		}
	}

	public void AttackButtonDown()
	{
		if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
			return;
		if (GameManager.Instance.Player.isSliding)
			return;
		if (GameManager.Instance.Player.PlayerState == PlayerState.Water && !GameManager.Instance.Player.allowWaterZoneShoot)
			return;
		if ((GameManager.Instance.Player.isHaningRope) && !GameManager.Instance.Player.allowHangingRopeShoot)
			return;
		if ((GameManager.Instance.Player.isGrabRopePoint) && !GameManager.Instance.Player.allowRopeType2Shoot)
			return;
		if (GameManager.Instance.Player.isClimbOnTop && !GameManager.Instance.Player.allowPipeShoot)
			return;
		if (GameManager.Instance.Player.wallSliding)
			return;

		RangeAttackDown(false);
	}

	//PC INPUT
	private void HandleInput()
	{
		if (Input.GetKeyDown( DefaultValueKeyboard.Instance.Left))
			MoveLeft();
		else if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Right))
			MoveRight();
		else if (Input.GetKeyUp(DefaultValueKeyboard.Instance.Left))
			StopMove(-1);
		else if (Input.GetKeyUp(DefaultValueKeyboard.Instance.Right))
			StopMove(1);

		if (Input.GetKeyDown( DefaultValueKeyboard.Instance.Down))
			MoveDown();
		else if (Input.GetKeyUp(DefaultValueKeyboard.Instance.Down))
			StopMove();

		if (Input.GetKeyDown( DefaultValueKeyboard.Instance.Up))
			MoveUp();
		else if (Input.GetKeyUp( DefaultValueKeyboard.Instance.Up))
		{
			StopLadder();
		}

		if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Jump))
		{
			Jump();
		}

		if (Input.GetKeyUp(DefaultValueKeyboard.Instance.Jump))
		{
			JumpOff();
		}

		if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Melee))
		{
			MeleeAttack();
		}

		if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Pause))
			MenuManager.Instance.Pause();

		if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Shooting))
		{
			AttackButtonDown();
		}

		if (Input.GetKeyDown(DefaultValueKeyboard.Instance.Cannon))
		{
			FireCannon();
		}
	}

	#region IListener implementation

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
		Debug.Log("StopMove");
		StopMove();

		isHoldingRight = false;
		isHoldingLeft = false;
		isMoveRight = false;
		isMoveLeft = false;
	}


	public void IOnStopMovingOn()
	{
	}

	public void IOnStopMovingOff()
	{
	}
	#endregion

	void OnDisable()
	{
		StopMove();
	}

	public void FireCannon()
	{
		fireCannonEvent();
	}

	public void ExitPartner()
	{
		partnerEvent();
	}
}