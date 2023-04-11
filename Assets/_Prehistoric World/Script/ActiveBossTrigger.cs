﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBossTrigger : MonoBehaviour
{
    public AudioClip soundDetectPlayer;
    public bool pauseMusic = true;

    [Header("BOSS ON START BEHAVIOR")]
    public bool hideBossOnStart = false;
    public GameObject showBossEffect;
    public AudioClip bossVisibleSound;

    [Header("Show Boss Option")]
    public BossManager bossObject;

    [Header("CHANGE GAME MUSIC")]
    public bool changeMusic = true;
    public AudioClip music;
    public float musicVolume = 0.8f;

    [Header("SET CAMERA MIN MAX")]
    public float limitLeftPos = -3;
    public float limitRightPos = 7;
    public bool setCameraLimitMin = true;
    public bool setCameraLimitMax = true;

    bool isTalk = false;

    void Start()
    {
        if (hideBossOnStart)
        {
            bossObject.gameObject.SetActive(false);
        }
    }

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (isTalk)
            yield break;

        if (other.GetComponent<Player>())
        {
            //set as check point
            GameManager.Instance.currentCheckpoint = transform;
            GameManager.Instance.checkpointDir = GameManager.Instance.Player.transform.localScale.x > 0 ? 1 : -1;

            SoundManager.PlaySfx(soundDetectPlayer, 0.8f);
            if (setCameraLimitMin)
                CameraFollow.Instance._min.x = transform.position.x - limitLeftPos;
            if (setCameraLimitMax)
                CameraFollow.Instance._max.x = transform.position.x + limitRightPos;

            GameManager.Instance.Player.velocity.x = 0;

            if (pauseMusic)
                SoundManager.Instance.PauseMusic(true);

                GameManager.Instance.Player.StopMove();
                MenuManager.Instance.TurnController(false);
                MenuManager.Instance.TurnGUI(false);
                GameManager.Instance.State = GameManager.GameState.Waiting;

            if (setCameraLimitMin)
            {
                Vector3 targetPos = CameraFollow.Instance.transform.position;
                targetPos.z = CameraFollow.Instance.transform.position.z;

                CameraFollow.Instance.isFollowing = false;
                Vector3 mainCameraStartPoint = CameraFollow.Instance.transform.position;

                float percent = 0;
                var targetBack = new Vector3(CameraFollow.Instance._min.x + CameraFollow.Instance.CameraHalfWidth, mainCameraStartPoint.y, mainCameraStartPoint.z);
                while (percent < 1)
                {
                    percent += Time.deltaTime * 1;
                    percent = Mathf.Clamp01(percent);
                    CameraFollow.Instance.transform.position = Vector3.Lerp(targetPos, targetBack, percent);
                    yield return null;
                }
                CameraFollow.Instance.isFollowing = true;
            }

            if (hideBossOnStart)
            {
                bossObject.gameObject.SetActive(true);
                Instantiate(showBossEffect, bossObject.gameObject.transform.position, Quaternion.identity);
                SoundManager.PlaySfx(bossVisibleSound);


                yield return new WaitForSeconds(2);
            }

            isTalk = true;
            FinishDialog();
        }
    }

    //called from DialogManager
    public void FinishDialog()
    {
        StartCoroutine(FinishDialogueCo());
    }


    IEnumerator FinishDialogueCo()
    {
            yield return new WaitForSeconds(1);
            bossObject.Play();

        SoundManager.Instance.PauseMusic(false);
        if (changeMusic)
            SoundManager.PlayMusic(music, musicVolume);

        ControllerInput.Instance.StopMove();
        MenuManager.Instance.TurnController(true);
        MenuManager.Instance.TurnGUI(true);

        GameManager.Instance.State = GameManager.GameState.Playing;
    }

    private void OnDrawGizmos()
    {
        if (bossObject)
            Gizmos.DrawLine(transform.position, bossObject.transform.position);

        if (setCameraLimitMin)
        {
            Gizmos.DrawLine(transform.position + Vector3.left * limitLeftPos, transform.position);
            Gizmos.DrawSphere(transform.position + Vector3.left * limitLeftPos, 0.2f);
        }

        if (setCameraLimitMax)
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * limitRightPos);
            Gizmos.DrawSphere(transform.position + Vector3.right * limitRightPos, 0.2f);
        }
    }
}
