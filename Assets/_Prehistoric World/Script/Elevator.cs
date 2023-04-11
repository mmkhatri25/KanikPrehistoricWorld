using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IListener {
	public float speed = 1;
	public Transform movingObj;
	public Transform[] paths;
	int nextPoint =0 ;
	public bool isMoving { get; set; }
    public AudioClip clickSound;
	public AudioClip movingSound;
	AudioSource movingSoundSrc;

	void Start () {
		if (paths.Length >= 2)
			nextPoint = 0;
		else {
			enabled = false;
			Debug.LogError ("Elevator need atleast 2 points");
		}

		movingSoundSrc = gameObject.AddComponent<AudioSource> ();
		movingSoundSrc.clip = movingSound;
		movingSoundSrc.Play ();
		movingSoundSrc.loop = true;
		movingSoundSrc.volume = 0;
	}

	public void Play(){
		isMoving = true;
		SoundManager.PlaySfx (clickSound);
	}

	public void Stop(){
		isMoving = false;
		movingSoundSrc.volume = 0;
	}

	public void Up(){
		nextPoint++;
		if (nextPoint >= paths.Length) {
			nextPoint--;
			return;
		} else {
			Play ();
		}
	}

	public void Down(){
		nextPoint--;
		if (nextPoint < 0) {
			nextPoint = 0;
			return;
		} else {
			Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			movingObj.position = Vector3.MoveTowards (movingObj.position, paths [nextPoint].position, speed * Time.deltaTime);
			movingSoundSrc.volume = GlobalValue.isSound ? 1 : 0;
			if (movingObj.position == paths [nextPoint].position)
				Stop ();
		}
	}

	void OnDrawGizmos() {
		if (paths != null && this.enabled) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i =0; i < paths.Length; i ++) {
				Vector3 globalWaypointPos = paths[i].position;
				Gizmos.DrawSphere(paths[i].position,size);
				Gizmos.color = Color.yellow;
				if (i + 1 < paths.Length)
					Gizmos.DrawLine (paths [i].position, paths [i + 1].position);
			}
		}
	}

 //   public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
 //   {
	//	nextPoint = 0;
	//	isMoving = false;
	//	movingSoundSrc.volume = 0;
	//	movingObj.position = paths[0].position;
	//}

    public void IPlay()
    {
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IGameOver()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
		nextPoint = 0;
		isMoving = false;
		movingSoundSrc.volume = 0;
		movingObj.position = paths[0].position;
	}

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }
}
