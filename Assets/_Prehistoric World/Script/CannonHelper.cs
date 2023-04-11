using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonHelper : MonoBehaviour, ITriggerPlayer
{
    public float setCameraSize = 5;
    public Transform cannonBody;
    public Transform firePoint;
    public float rotateSpeed = 20;
    public float limitAngle = 60;
    public float fireForce = 10;
    public AudioClip soundFire;
    public GameObject fireFX;
   
    //public bool playEarthQuakeOnHitDogge = true;
    public float _eqTime = 0.1f;
    public float _eqSpeed = 60;
    public float _eqSize = 1;

    bool rotatingToRight = true;
    float currentAngle = 0;
    Animator anim;

    float lastTimeShoot = -999;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void ControllerInput_fireCannonEvent()
    {
        ControllerInput.fireCannonEvent -= ControllerInput_fireCannonEvent;
        Fire();
    }

    IEnumerator RotatingCannonCo()
    {
        currentAngle = cannonBody.rotation.eulerAngles.z;
        currentAngle = (currentAngle > 180) ? currentAngle - 360 : currentAngle;
        while (true)
        {
            if (rotatingToRight)
            {
                currentAngle += rotateSpeed * Time.deltaTime;
                if (currentAngle > limitAngle)
                {
                    currentAngle = limitAngle;
                    rotatingToRight = false;
                }
            }
            else
            {
                currentAngle -= rotateSpeed * Time.deltaTime;
                if (currentAngle < -limitAngle)
                {
                    currentAngle = -limitAngle;
                    rotatingToRight = true;
                }
            }

            cannonBody.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }
    }

    public void Fire()
    {
        lastTimeShoot = Time.time;
        StopAllCoroutines();
        anim.SetTrigger("fire");
        GameManager.Instance.Player.GetInCannon(false);
        ControllerInput.Instance.StopMove();
        GameManager.Instance.Player.transform.position = firePoint.position;
        GameManager.Instance.Player.gameObject.SetActive(true);
        GameManager.Instance.Player.SetForce(cannonBody.up * fireForce);
        SoundManager.PlaySfx(soundFire);
        if (fireFX)
            Instantiate(fireFX, firePoint.position, Quaternion.identity);

            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);

        CameraFollow.Instance.ZoomOut();
        //CinemachineManager.Instance.SetScreenOffset(0.5f, 0.5f);
    }

    public void OnTrigger()
    {
        if (Time.time < (lastTimeShoot + 0.5f))
            return;

        if (GameManager.Instance.Player.playerWithPartner)
            return;

        GameManager.Instance.Player.GetInCannon(true);
        ControllerInput.fireCannonEvent += ControllerInput_fireCannonEvent;
        StartCoroutine(RotatingCannonCo());

        CameraFollow.Instance.ZoomIn(setCameraSize);
    }
}
