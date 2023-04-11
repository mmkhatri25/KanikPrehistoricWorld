using UnityEngine;
using System.Collections;

public class DetectMonsterFalling : MonoBehaviour, IListener {
	public Rigidbody2D monsterIV;
	public AudioClip soundShowUp;
    bool isStop = false;

    public void IGameOver()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        isStop = false;
    }

    public void IOnStopMovingOn()
    {
        isStop = true;
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IPlay()
    {
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }

    // Use this for initialization
    IEnumerator OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player>() && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer ("HidingZone")) {
            while (isStop) { yield return null;  }
			SoundManager.PlaySfx (soundShowUp);
			monsterIV.isKinematic = false;
			Destroy (gameObject);
		}
	}
}
