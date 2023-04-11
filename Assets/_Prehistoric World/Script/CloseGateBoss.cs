using UnityEngine;
using System.Collections;

public class CloseGateBoss : MonoBehaviour {
    public CloseGateBossState ownerGate;
    public enum OPENDIR { UpToDown, DownToUp}
    public OPENDIR openType;
	bool moving = false;
	public Transform TheGate;
	
	public AudioClip sound;

    public Transform upPos, downPos;
    public float movingSpeed = 5;
    public bool useCameraShake = true;

    //[Header("Active Reset Camera, place ResetCamera here")]
    //public ResetBoundCamera ResetCamera;

    float movePercent = 0;

	
    // Use this for initialization
    void Start()
    {
        //if (openType == OPENDIR.DownToUp)
        //    TheGate.position = downPos.position;
        //else
        //    TheGate.position = upPos.position;
    }
	
	// Update is called once per frame
	void Update () {
		if (moving) {
            movePercent += movingSpeed * Time.deltaTime;

            if(openType == OPENDIR.DownToUp)
                TheGate.position = Vector2.MoveTowards(downPos.position, upPos.position, movePercent);
            else
                TheGate.position = Vector2.MoveTowards(upPos.position, downPos.position, movePercent);

            //TheGate.position = Vector2.MoveTowards (TheGate.transform.position, transform.position, 0.1f);
			if (Vector2.Distance (TheGate.transform.position, openType == OPENDIR.DownToUp?upPos.position:downPos.position) == 0) {
				GetComponent<BoxCollider2D> ().enabled = false;
				if (useCameraShake) {
					var camShake = FindObjectOfType<CameraPlay_Shake> ();
					if(camShake)
						camShake.ForceDestroy ();
//					FindObjectOfType<CameraShake> ().StopShake ();
				}
                Destroy(this);
			}
		}
	}
    bool isWorked = false;
	void OnTriggerEnter2D(Collider2D other){
        if (ownerGate.isClosed && openType == OPENDIR.UpToDown)
            return;

        if (!ownerGate.isClosed && openType == OPENDIR.DownToUp)
            return;

        if (isWorked)
            return;

		if (other.GetComponent<Player> () != null) {
            if (openType == OPENDIR.DownToUp)
                TheGate.position = downPos.position;
            else
                TheGate.position = upPos.position;

            moving = true;
            isWorked = true;
            ownerGate.UpdateState(openType == OPENDIR.UpToDown);


            SoundManager.PlaySfx (sound);
			if (useCameraShake) {
				CameraPlay.EarthQuakeShake(2,30, 2);
//				FindObjectOfType<CameraShake> ().DoShakeCustom (0, 0.2f);
			}
			
			//if (ResetCamera != null)
			//	ResetCamera.Work ();
		}
	}
}
