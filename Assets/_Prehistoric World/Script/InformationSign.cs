using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationSign : MonoBehaviour
{
    public enum DetectZone { Circle, Box}
    public DetectZone detectZone;
    public float radius = 1;
    public float detectHigh = 3;

    public AudioClip sound;
    public SpriteRenderer spriteRenderer;
    public GameObject inforContainer;
    Color oriColor;
    Color colorTransparent;
    [ReadOnly] public bool playerInArea = false;

    public GameObject mobileTut, windowsTut;



    public virtual void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
         if(mobileTut)
        mobileTut.SetActive(true);
         if(windowsTut)
        windowsTut.SetActive(false);
#else
        if (mobileTut)
        mobileTut.SetActive(false);
        if (windowsTut)
            windowsTut.SetActive(true);
#endif

        oriColor = spriteRenderer.color;
        colorTransparent = oriColor;
        colorTransparent.a = 0;
        spriteRenderer.color = colorTransparent;
        InvokeRepeating("CheckPlayerInRange", 1, 0.1f);
    }

    void CheckPlayerInRange()
    {
        if (!gameObject.activeInHierarchy)
            return;

        RaycastHit2D hit;
        if (detectZone == DetectZone.Circle)
            hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, 1 << (LayerMask.NameToLayer("Player")));
        else
            hit = Physics2D.BoxCast(transform.position, new Vector2(1, detectHigh), 0, Vector2.zero, 0, 1 << (LayerMask.NameToLayer("Player")));
        if (hit)
        {
            if (!playerInArea)
            {
                SoundManager.PlaySfx(sound);
                StartCoroutine(MMFade.FadeSpriteRenderer(spriteRenderer, 0.2f, oriColor));
                inforContainer.SetActive(true);
            }

            playerInArea = true;
        }
        else
        {
            if (playerInArea)
                StartCoroutine(MMFade.FadeSpriteRenderer(spriteRenderer, 0.2f, colorTransparent));

            inforContainer.SetActive(false);
            playerInArea = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (detectZone == DetectZone.Circle)
            Gizmos.DrawWireSphere(transform.position, radius);
        else
            Gizmos.DrawWireCube(transform.position, new Vector2(1, detectHigh));
    }
}
