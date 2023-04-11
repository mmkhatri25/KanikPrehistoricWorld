using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollItem : MonoBehaviour, IListener,ITriggerPlayer
{
    public int ID = 1;
    public AudioClip sound;
    public GameObject effect;
    public int giveCoin = 100;
    
    bool isCollected = false;

    public float moveToTargetSpeed = 5;
    Vector2 startPos;

    //called by Player
    public void OnTrigger()
    {
        if (isCollected)
            return;

        isCollected = true;

        if (GameMode.Instance != null)
        {
            switch (ID)
            {
                case 1:
                    GlobalValue.bigStar1 = true;
                    break;
                case 2:
                    GlobalValue.bigStar2 = true;
                    break;
                case 3:
                    GlobalValue.bigStar3 = true;
                    break;
            }
        }

        SoundManager.PlaySfx(sound);

        Instantiate(effect, transform.position, Quaternion.identity);

        startPos = transform.position;
        StartCoroutine(MoveToTargetCo());
    }

    IEnumerator MoveToTargetCo()
    {
        float percentMove = 0;
        while (percentMove < 1)
        {
            percentMove += Time.deltaTime * moveToTargetSpeed;
            transform.position = Vector2.Lerp(startPos, Menu_GUI.Instance.maskUIPosition(ID), percentMove);

            yield return null;
        }

        //GameManager.Instance.maskCollector++;
        //Menu_GUI.Instance.MaskCollect(GameManager.Instance.maskCollector);
        Menu_GUI.Instance.ScrollCollectAnim(ID);

        GameManager.Instance.AddCoin(giveCoin, transform);
        gameObject.SetActive(false);
    }

    public void IPlay()
    {
        CheckCollected();
    }

    void CheckCollected()
    {
        if (GameMode.Instance == null)
            return;

        bool isCollected = GlobalValue.IsScrollLevelAte(ID);

        if (isCollected)
        {
            Menu_GUI.Instance.ScrollCollectAnim(ID);
            Destroy(gameObject);
        }
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
