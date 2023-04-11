using UnityEngine;
using System.Collections;

public class GameFinishFlag : MonoBehaviour, ITriggerPlayer
{
    public GameObject fireObj;

    private void Start()
    {
        if (fireObj)
            fireObj.SetActive(false);
    }

    public void OnTrigger()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (fireObj)
            fireObj.SetActive(true);
        GameManager.Instance.GameFinish();
    }
}
