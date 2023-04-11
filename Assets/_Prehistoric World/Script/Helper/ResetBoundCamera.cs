using UnityEngine;
using System.Collections;

public class ResetBoundCamera : MonoBehaviour {
    //public Transform leftPoint;
    //public Transform rightPoint;
    public bool canUseAgain = true;
    public float distanceLocalRight = 15;
    public float distanceLocalLeft = 15;

    public float slideSpeed = 10;

	bool isActive = false;
	float startMinX, startMaxX;
    public enum MOVEDIR { Left2Right, Right2Left}
    MOVEDIR moveDir;
	//public bool moveLeftToRight = true;
    CameraFollow targetCamera;

    float leftPoint, rightPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * distanceLocalRight);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * distanceLocalLeft);
    }

    void OnTriggerEnter2D(Collider2D other){
		if (isActive)
			return;

		if (other.GetComponent<Player> () == null)
			return;

        Work();
        //isActive = true;
        //transform.FindChild ("Detect Reset Camera").gameObject.SetActive (false);

        //      targetCamera = FindObjectOfType<CameraFollow>();

        //      var CameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);

        //startMinX = Camera.main.transform.position.x - CameraHalfWidth;
        //startMaxX = Camera.main.transform.position.x + CameraHalfWidth;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //if (!isActive)
        //    return;

        if (other.GetComponent<Player>() == null)
            return;

        isActive = true;
        //Work();
        //isActive = true;
        //transform.FindChild ("Detect Reset Camera").gameObject.SetActive (false);

        //      targetCamera = FindObjectOfType<CameraFollow>();

        //      var CameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);

        //startMinX = Camera.main.transform.position.x - CameraHalfWidth;
        //startMaxX = Camera.main.transform.position.x + CameraHalfWidth;
    }

    void Update(){
		if (!isActive)
			return;

        GameManager.Instance.Player.isPlaying = false;

        

		if (moveDir == MOVEDIR.Left2Right) {

			startMinX += slideSpeed * Time.deltaTime;
			startMaxX += slideSpeed * Time.deltaTime;

			startMinX = Mathf.Min (startMinX, leftPoint);
			startMaxX = Mathf.Min (startMaxX, rightPoint);
		} else {
			startMinX -= slideSpeed * Time.deltaTime;
			startMaxX -= slideSpeed * Time.deltaTime;

			startMinX = Mathf.Max (startMinX, leftPoint);
			startMaxX = Mathf.Max (startMaxX, rightPoint);
		}

        if(startMinX == leftPoint && moveDir == MOVEDIR.Left2Right)
        {
            startMaxX = rightPoint;
            isActive = false;
            GameManager.Instance.Player.isPlaying = true;
            if (!canUseAgain)
                Destroy(this);
        }else if (startMaxX == rightPoint && moveDir == MOVEDIR.Right2Left)
        {
            startMinX = leftPoint;
            isActive = false;
            GameManager.Instance.Player.isPlaying = true;
            if (!canUseAgain)
                Destroy(this);
        }

        targetCamera._min.x = startMinX;
        targetCamera._max.x = startMaxX;

    }

	public void Work(){
		//isActive = true;
        //transform.FindChild("Detect Reset Camera").gameObject.SetActive(false);
        targetCamera = FindObjectOfType<CameraFollow>();
        var CameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);

		startMinX = Camera.main.transform.position.x - CameraHalfWidth;
		startMaxX = Camera.main.transform.position.x + CameraHalfWidth;

        moveDir = transform.position.x > GameManager.Instance.Player.transform.position.x? MOVEDIR.Left2Right : MOVEDIR.Right2Left;

        if (moveDir == MOVEDIR.Left2Right)
        {
            leftPoint = transform.position.x - 2;
            rightPoint = transform.position.x + distanceLocalRight;
        }
        else
        {
            leftPoint = transform.position.x - distanceLocalLeft;
            rightPoint = transform.position.x + 2;
        }

    }
}
