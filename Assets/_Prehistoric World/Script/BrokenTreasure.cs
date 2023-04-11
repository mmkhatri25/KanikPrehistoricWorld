using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenTreasure : MonoBehaviour, ICanTakeDamage
{
    public enum Type { _2D, _3D }
    public Type type;
    public enum BlockTyle { Destroyable, Rocky }
    public BlockTyle blockTyle;
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

                if (blockTyle == BlockTyle.Rocky)
                    item.transform.position += Vector3.up * 1.5f;
            }
        }
        GetComponent<BoxCollider2D>().enabled = false;

        if (anim)
            anim.enabled = true;

        SoundManager.PlaySfx(sound);


        if (blockTyle == BlockTyle.Destroyable)
        {
            if (destroyFX)
                Instantiate(destroyFX, transform.position, Quaternion.identity);

            //if (type == Type._2D && mainImage)
            //    mainImage.enabled = false;

            gameObject.SetActive(false);
        }
        else if (blockTyle == BlockTyle.Rocky)
        {
            if (anim)
                anim.enabled = false;
            if (type == Type._2D && mainImage)
            {
                mainImage.sprite = imageBlockStatic;
                GetComponent<BoxCollider2D>().enabled = true;
                if (GetComponent<CanBeJumpOn>())
                    Destroy(GetComponent<CanBeJumpOn>());
                Destroy(this);
            }
            if (type == Type._3D && StoneBlock3DObj)
            {
                Instantiate(StoneBlock3DObj, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            
        }

    }

    #endregion
}
