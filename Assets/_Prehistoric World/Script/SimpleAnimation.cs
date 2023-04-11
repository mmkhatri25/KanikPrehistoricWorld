using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleAnimation : MonoBehaviour
{
    public SpriteRenderer ownerSpriteRenderer;
    public Image ownerImage;
    public float ratePerSprite = 0.1f;
    public Sprite[] spriters;
    int currentPos = 0;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ChangeSprite", Random.Range(0f, 0.1f), ratePerSprite);
    }

    void ChangeSprite()
    {
        if (ownerSpriteRenderer)
            ownerSpriteRenderer.sprite = spriters[currentPos];

        if (ownerImage)
            ownerImage.sprite = spriters[currentPos];

        currentPos++;
        if (currentPos >= spriters.Length)
            currentPos = 0;
    }
}
