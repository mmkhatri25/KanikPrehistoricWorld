using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedChest : MonoBehaviour, ICanTakeDamage
{
    public bool canBeBroken = false;
    public GameObject destroyFX;
    public SpriteRenderer mainImage;

    public Transform spawnPoint;
    public GameObject[] randomItem;
    public AudioClip sound;

    bool isWorked = false;
    public Animator anim;
    public Sprite imageBlockStatic;
    public GameObject StoneBlock3DObj;
    // Use this for initialization
    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (anim && !canBeBroken)
            anim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ICanTakeDamage implementation

    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isWorked)
            return;

        isWorked = true;

        if (randomItem.Length > 0)
        {
            int pickObj = Random.Range(0, randomItem.Length);
            if (randomItem[pickObj] != null)
            {
                var item = Instantiate(randomItem[pickObj], spawnPoint.position, Quaternion.identity) as GameObject;
                if (!canBeBroken)
                    item.transform.SetParent(spawnPoint);

            }
        }
        GetComponent<BoxCollider2D>().enabled = false;

        if (anim)
            anim.enabled = true;

        SoundManager.PlaySfx(sound);

        if (canBeBroken)
        {
            if (destroyFX)
                Instantiate(destroyFX, transform.position, Quaternion.identity);

            if (mainImage)
                mainImage.enabled = false;

            gameObject.SetActive(false);

        }
    }

    #endregion
}