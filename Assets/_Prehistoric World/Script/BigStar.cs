using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigStar : MonoBehaviour, ITriggerPlayer
{
    [Range(1, 3)]
    public int ID = 1;
    public Sprite uiIcon;
    public AudioClip sound;
    public GameObject collectFX;
    bool isWork = false;

    private void Awake()
    {
        GlobalValue.ResetBigStars();
    }

    void Collect()
    {
        isWork = true;

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

        Instantiate(collectFX, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void OnTrigger()
    {
        if (isWork)
            return;

        SoundManager.PlaySfx(sound);
        Collect();
    }
}