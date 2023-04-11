using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCatcher : MonoBehaviour, IListener
{
	public float radius = 7f;
	public LayerMask playerLayer;

	public AudioClip hangingRopeSound;

	public Transform dot;
	public Transform end;
	public Transform beginPos;
	public Transform targetPoint;
	public float workingAngle = 45;
	public float speed = 10;
	public float movingHangingSpeed = 10;

	public bool isInZone { get; set; }
	public bool isWorking { get; set; }

	private float offset = 90f;
	private bool goRight;
	private LineRenderer lineRend;

	private Vector2 destination;
	Vector3 dotOripos;

	float targetPercent = 0;
	Vector2 offsetBeginWithDot;
	[ReadOnly] public bool isFinishTheDestination = false;

	// Use this for initialization
	void Start()
	{
		lineRend = GetComponent<LineRenderer>();

		destination = dot.position + lineRend.GetPosition(1);

		dotOripos = dot.localPosition;

		offsetBeginWithDot = (Vector2)(dot.position - beginPos.position);

		lineRend.SetPosition(0, transform.position);
		lineRend.SetPosition(1, targetPoint.position);
	}

	// Update is called once per frame
	void Update()
	{
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		RaycastHit2D hit = Physics2D.CircleCast(dot.position, radius, Vector2.zero, 0, playerLayer);

		isInZone = hit;
		if (hit)
		{
			if (RopeUI.instance)
				RopeUI.instance.CurrentRope = this;
		}
		else if (RopeUI.instance)
		{
			if (RopeUI.instance.CurrentRope && RopeUI.instance.CurrentRope == this)
				RopeUI.instance.CurrentRope = null;
		}

		if (isWorking)
		{

			targetPercent += movingHangingSpeed * Time.deltaTime;
			dot.position = Vector2.MoveTowards(beginPos.position, targetPoint.position, targetPercent) + offsetBeginWithDot;

			if (Vector2.Distance(dot.position, targetPoint.position + (Vector3)offsetBeginWithDot) < 0.02f)
			{
				GameManager.Instance.Player.ExitTheRope();
				RopeUI.instance.ExitRope();
				isFinishTheDestination = true;

			}
		}
	}

	public void CatchTheRope()
	{
		if (isFinishTheDestination)
			return;

		if (!isWorking)
		{
			GameManager.Instance.Player.isHaningRope = true;

			GameManager.Instance.Player.transform.SetParent(end, true);
			GameManager.Instance.Player.transform.localPosition = Vector3.zero;

			isWorking = true;

			SoundManager.PlaySfx(hangingRopeSound);

			goRight = GameManager.Instance.Player.transform.position.x < dot.position.x;
			lineRend.enabled = true;
		}
	}

	public void Stop()
	{
		isWorking = false;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(dot.position, radius);

		if (!Application.isPlaying)
		{
			if (lineRend == null)
				lineRend = GetComponent<LineRenderer>();

			lineRend.enabled = true;
			lineRend.SetPosition(0, transform.position);
			lineRend.SetPosition(1, targetPoint.position);
		}
	}

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
		isWorking = false;
	}

	public void IOnRespawn()
	{
		isFinishTheDestination = false;
		dot.localPosition = dotOripos;
		targetPercent = 0;
		//		throw new System.NotImplementedException ();

	}

	public void IOnStopMovingOn()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOff()
	{
		//		throw new System.NotImplementedException ();
	}

	#endregion
}
