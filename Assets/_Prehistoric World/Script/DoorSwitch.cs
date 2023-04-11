using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour, ICanTakeDamage
{
    public PlatformController Door;
    public AudioClip doorSound;
    public AudioClip switchSound;
    public bool useCameraShake = false;
    bool isDetectPlayer = false;
    Animator anim;
    bool isOpen = false;
    bool isWoring = false;

    void Start()
    {
        Door.enabled = false;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isOpen)
            return;

        isDetectPlayer = true;
        Work();
    }

    void Work()
    {
        if (!isDetectPlayer || isWoring)
            return;

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        StartCoroutine(WorkCo());
    }

    IEnumerator WorkCo()
    {
        isWoring = true;

        SoundManager.PlaySfx(doorSound);
        SoundManager.PlaySfx(switchSound);
        isOpen = true;
        GameManager.Instance.isHasKey = false;

        if (anim)
            anim.SetTrigger("open");

        if (Door.GetComponent<Animator>())
            Door.GetComponent<Animator>().SetTrigger("open");

        yield return new WaitForSeconds(0.5f);

        Door.enabled = true;

        if (useCameraShake)
            CameraPlay.EarthQuakeShake(999, 30, 2);

        while (Door.enabled)
        {
            yield return null;
        }

        if (useCameraShake)
        {
            var camShake = FindObjectOfType<CameraPlay_Shake>();
            if (camShake)
                camShake.ForceDestroy();
        }
        enabled = false;
    }
}
