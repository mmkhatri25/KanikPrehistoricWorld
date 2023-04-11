using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSearchLightEnemy : MonoBehaviour, ICanTakeDamage, IListener {
    public enum Type { AlwayOpen, OpenAndClose, CloseWhenMiddle}
    public Type eyeType;

    public float rotateSpeed = 30;
    public float changeDirectionDelay = 1;
    
    public float maxAngle = 30;

    [Header("Open and Close")]
    public float closeEyeDelay = 2;
    public enum ROTATEORDER { LEFT, RIGHT}
    public ROTATEORDER[] rotateOrder;
    public Animator eyeAnim;
    public GameObject searchLightObj;
    bool isRotateRight = true;

    public AudioSource audio3DScr;
    public AudioClip soundLight;

    [Header("Like Enemy")]
    public GameObject HurtEffect;
    public GameObject DestroyEffect;        //spawn object when dead
    [Range(0, 100)]
    public float health = 50;
    float currentHealth;
    public AudioClip hurtSound;
    public AudioClip deadSound;

    Vector3 originalPos;

    // Use this for initialization
    void Start () {
        if (eyeType == Type.AlwayOpen)
            StartCoroutine(RotateAlwayOpenCo());
        else if (eyeType == Type.OpenAndClose)
            StartCoroutine(RotateOpenAndCloseCo());

        audio3DScr.clip = soundLight;
        audio3DScr.volume = 0;
        audio3DScr.Play();

        currentHealth = health;
        originalPos = transform.position;
    }

    void Update()
    {
        if (eyeType == Type.CloseWhenMiddle)
        {
            eyeAnim.SetBool("close", transform.position == originalPos);
            searchLightObj.SetActive(transform.position != originalPos);
            audio3DScr.volume = GlobalValue.isSound ? (transform.position != originalPos ? 1 : 0) : 0;
        }
    }

    IEnumerator RotateAlwayOpenCo()
    {
        float angle = 0;
        while (true)
        {
            while (isStop) { yield return null; }
            audio3DScr.volume = GlobalValue.isSound ? 1 : 0;

            angle += rotateSpeed * Time.deltaTime * (isRotateRight ? 1 : -1);
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

            transform.localRotation = Quaternion.Euler(0, 0, angle);
            if (angle == maxAngle || angle == -maxAngle)
            {
                isRotateRight = !isRotateRight;
                audio3DScr.volume = 0;
                yield return new WaitForSeconds(changeDirectionDelay);
            }

            yield return null;
        }
    }

    int currentOrder = 0;
    IEnumerator RotateOpenAndCloseCo()
    {
        eyeAnim.SetBool("close", true);
        searchLightObj.SetActive(false);
        yield return new WaitForSeconds(changeDirectionDelay);

        float angle = 0;
        currentOrder = 0;
        isRotateRight = rotateOrder[currentOrder] == ROTATEORDER.RIGHT;

        while (true)
        {
            while (isRotateRight)
            {
                eyeAnim.SetBool("close", false);
                searchLightObj.SetActive(true);
                audio3DScr.volume = GlobalValue.isSound ? 1 : 0;

                angle += rotateSpeed * Time.deltaTime;
                angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

                while (isStop) { yield return null; }
                transform.localRotation = Quaternion.Euler(0, 0, angle);
                if (angle == maxAngle)
                {
                    yield return new WaitForSeconds(changeDirectionDelay);

                    bool reversing = true;
                    while (reversing)
                    {
                        angle -= rotateSpeed * Time.deltaTime;
                        angle = Mathf.Clamp(angle, 0, maxAngle);

                        while (isStop) { yield return null; }
                        transform.localRotation = Quaternion.Euler(0, 0, angle);

                        if (angle == 0)
                        {
                            NextOrder();
                            reversing = false;
                            yield return new WaitForSeconds(closeEyeDelay);
                        }

                        yield return null;
                    }
                }

                yield return null;
            }

            while (!isRotateRight)
            {
                eyeAnim.SetBool("close", false);
                searchLightObj.SetActive(true);
                audio3DScr.volume = GlobalValue.isSound ? 1 : 0;

                angle -= rotateSpeed * Time.deltaTime;
                angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

                while (isStop) { yield return null; }
                transform.localRotation = Quaternion.Euler(0, 0, angle);
                if (angle == -maxAngle)
                {
                    yield return new WaitForSeconds(changeDirectionDelay);

                    bool reversing = true;
                    while (reversing)
                    {
                        angle += rotateSpeed * Time.deltaTime;
                        angle = Mathf.Clamp(angle, -maxAngle, 0);

                        while (isStop) { yield return null; }
                        transform.localRotation = Quaternion.Euler(0, 0, angle);

                        if (angle == 0)
                        {
                            NextOrder();
                            reversing = false;
                            yield return new WaitForSeconds(closeEyeDelay);
                        }

                        yield return null;
                    }
                }

                yield return null;
            }
        }
    }

    void NextOrder()
    {
        currentOrder++;
        if (currentOrder >= rotateOrder.Length)
            currentOrder = 0;

        isRotateRight = rotateOrder[currentOrder] == ROTATEORDER.RIGHT;
        eyeAnim.SetBool("close", true);
        searchLightObj.SetActive(false);
        audio3DScr.volume = 0;
    }

    bool isDead = false;
    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        //Debug.LogError(damage);
        if (currentHealth <= 0)
        {
            isDead = true;
            Dead();
        }
        else {
            SoundManager.PlaySfx(hurtSound);
            if (HurtEffect != null)
                Instantiate(HurtEffect, hitPoint, transform.rotation);
        }
    }

    protected void Dead()
    {
        SoundManager.PlaySfx(deadSound);
        
        GetComponent<BoxCollider2D>().enabled = false;

        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    bool isStop = false;
    public void IPlay()
    {
        //		throw new System.NotImplementedException ();
    }

    public void ISuccess()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IUnPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IGameOver()
    {
        isStop = true;
        //		throw new System.NotImplementedException ();
    }

    public void IOnRespawn()
    {
        isStop = false;
        //		throw new System.NotImplementedException ();
    }

    public void IOnStopMovingOn()
    {
        Debug.Log("IOnStopMovingOn");
        //		anim.enabled = false;
        isStop = true;
        //		GetComponent<Rigidbody2D> ().isKinematic = true;
    }

    public void IOnStopMovingOff()
    {
        //		anim.enabled = true;
        isStop = false;
        //		GetComponent<Rigidbody2D> ().isKinematic = false;
    }


}
