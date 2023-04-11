using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawnTrigger : MonoBehaviour {
    public AutoSpawn autoSpawnOwner;
    bool isWorked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isWorked)
            return;

        if(collision.gameObject == GameManager.Instance.Player.gameObject)
        {
            autoSpawnOwner.Play();
            isWorked = true;
        }
    }
}
