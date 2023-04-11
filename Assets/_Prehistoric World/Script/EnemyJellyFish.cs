﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Controller2D))]
public class EnemyJellyFish : Enemy
{
    [Header("ENEMY JELLY FISH")]
    public Vector2 jumpForce = new Vector2(6, 8);
    public bool moveRightFirst = true;
    public AudioClip jumpSound;
    Vector2 direction;
    
    public float waitMin = 1;
    public float waitMax = 2;

    public override void Start()
    {
        base.Start();
        direction = moveRightFirst ? Vector2.right : Vector2.left;

        StartCoroutine(JumpCo());
        anim.SetBool("isGrounded", true);
    }
    public override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        velocity.y += -gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisions.above || controller.collisions.below)
        {
            //velocity.x = 0;
            velocity.y = 0;
            if (controller.collisions.below)
                velocity.x = 0;
        }
    }

    public override void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        base.Die();
        StopAllCoroutines();
        //Destroy(gameObject, 2);
    }

    public IEnumerator JumpCo()
    {
        while (true)
        {
            anim.SetTrigger("jump");

            yield return new WaitForSeconds(Random.Range(waitMin, waitMax));
        }
    }

    //called by Anim
    public void AnimJump()
    {
        if (isDead)
            return;

        if (controller.collisions.left || controller.collisions.right)
        {
            direction *= -1;
        }

        velocity = jumpForce;
        velocity.x *= direction.x;

        SoundManager.PlaySfx(jumpSound);
    }
}