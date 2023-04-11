using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour, IListener
{
	public static CameraFollow Instance;
	[Tooltip("Litmited the camera moving within this box collider")]
	public Collider2D Bounds;

	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 focusAreaSize;

	[Header("Zoom in Zoom out")]
	public bool allowZoomInZoomOut = false;
	[Tooltip("How long player don't move to active zoom action")]
	public float timeDelay = 3f;
	public float speed = 10f;
	[Range(50, 100)]
	public float minPercent = 80;
	float maxSize, minSize;
	float timeCounting = 0;

	[HideInInspector]
	public Vector2 _min, _max;
	public bool isFollowing { get; set; }

	Player target;
	FocusArea focusArea;
	Camera camera;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;
	bool lookAheadStopped;

	[Tooltip("Zoom Speed")]
	public float zoomSpeed = 1;
	private bool isZooming = false;
	float originalSize, ZoomSize;

	public float CameraHalfWidth
	{
		get { return (Camera.main.orthographicSize * ((float)Screen.width / Screen.height)); }
	}

	void Start()
	{
		Instance = this;
		target = FindObjectOfType<Player>();
		focusArea = new FocusArea(target.controller.boxcollider.bounds, focusAreaSize);

		if (Bounds == null)
		{
			Debug.LogError("Add the Bounds object (BoxCollider2D) to limit the camera", gameObject);
			return;
		}

		_min = Bounds.bounds.min;
		_max = Bounds.bounds.max;
		isFollowing = true;

		camera = GetComponent<Camera>();
		maxSize = camera.orthographicSize;
		minSize = maxSize * (minPercent / 100f);

		originalSize = camera.orthographicSize;

		if (!followY)
			originalFollowY = transform.position.y;
	}

	void Update()
	{
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		timeCounting += Time.deltaTime;


		if (Input.anyKey || GameManager.Instance.Player.input != Vector2.zero)
		{
			timeCounting = 0;
		}
	}

	[HideInInspector]
	public bool followX = true;
	public bool followY = true;
	[HideInInspector] public float originalFollowY;
	void LateUpdate()
	{

		if (!isFollowing)
			return;

		if (target == null)
			target = GameManager.Instance.Player;

		focusArea.Update(target.controller.boxcollider.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		if (focusArea.velocity.x != 0)
		{
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if (Mathf.Sign(target.controller.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.controller.playerInput.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else
			{
				if (!lookAheadStopped)
				{
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
				}
			}
		}
		if (isZooming)
		{
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, ZoomSize, zoomSpeed * Time.deltaTime);
		}
		else
		{
			if (timeCounting >= timeDelay && allowZoomInZoomOut)
			{
				camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, minSize, speed * Time.deltaTime);
			}
			else
				camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, maxSize, speed * Time.deltaTime);
		}

		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;

		var CameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);
		focusPosition.x = Mathf.Clamp(focusPosition.x, _min.x + CameraHalfWidth, _max.x - CameraHalfWidth);
		focusPosition.y = Mathf.Clamp(focusPosition.y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

		if (!followX)
			focusPosition.x = transform.position.x;
		if (!followY)
		{
			focusPosition.y = Mathf.SmoothDamp(transform.position.y, originalFollowY, ref smoothVelocityY, verticalSmoothTime);
			focusPosition.y = Mathf.Clamp(focusPosition.y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);
		}

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, .5f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);

        if (Bounds != null)
        {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(Bounds.bounds.center, Bounds.bounds.size);
        }
	}

	struct FocusArea
	{
		public Vector2 centre;
		public Vector2 velocity;
		float left, right;
		float top, bottom;


		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}

	public void ZoomIn(float value)
	{
		isZooming = true;
		ZoomSize = value;
	}

	public void ZoomOut()
	{
		isZooming = false;
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

	}

	public void IOnStopMovingOn()
	{

	}

	public void IOnStopMovingOff()
	{

	}
}