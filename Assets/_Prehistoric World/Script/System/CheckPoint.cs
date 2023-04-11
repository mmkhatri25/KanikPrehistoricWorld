using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour, ITriggerPlayer {

    public void OnTrigger()
    {
        GameManager.Instance.currentCheckpoint = transform;
        GameManager.Instance.checkpointDir = GameManager.Instance.Player.transform.localScale.x > 0 ? 1 : -1;

        SoundManager.PlaySfx(SoundManager.Instance.soundCheckpoint);
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
