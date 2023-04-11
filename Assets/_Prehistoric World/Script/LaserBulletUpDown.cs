using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletUpDown : MonoBehaviour, ICanTakeDamage, IListener
{
    Player player;
    public float speedMin = 2f;
    public float speedMax = 3f;
    float speed;
    public Vector2 direction;
    public float timeToLive = 5;
    public GameObject ExplosionFx;

    bool allowPlaying = false;
    bool moveToTarget = false;
    float timeCouint = 0;
    Transform target;
    Vector2 directAttack;

    public float moveDownAngle = -20;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        speed = Random.Range(speedMin, speedMax);
        GameManager.Instance.listeners.Add(this);
    }

    public void Init(float YOffset = 0.5f, float XOffset = 0)
    {
        direction = (Vector2)(Quaternion.Euler(0, 0, (Mathf.Abs(moveDownAngle + (directAttack.x > 0 ? 0 : 180)) * (directAttack.y > 0 ? 1 : -1))) * Vector2.right);
        allowPlaying = true;
    }

    public void MoveToFirstTarget(Transform pos, float delayAttack, Vector2 dirAttack)
    {
        directAttack = dirAttack;
        StartCoroutine(MoveToFirstTargetCo(pos, delayAttack));
    }

    IEnumerator MoveToFirstTargetCo(Transform pos, float delayAttack)
    {
        target = pos;
        moveToTarget = true;
        yield return new WaitForSeconds(delayAttack);

        allowPlaying = true;
        moveToTarget = false;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStop)
            return;


        Debug.Log("moveToTarget" + moveToTarget);

        if (allowPlaying)
            transform.Translate(speed * direction * Time.deltaTime);
        else if (moveToTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * 2 * Time.deltaTime);
        }

        timeCouint += Time.deltaTime;
        if (timeCouint > timeToLive)
            Destroy(gameObject);
    }

    #region ICanTakeDamage implementation

    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        Destroy(gameObject);
    }

    #endregion
    #region IListener implementation

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
        //		throw new System.NotImplementedException ();
    }

    public void IOnRespawn()
    {
        //		throw new System.NotImplementedException ();
    }

    bool isStop = false;
    public void IOnStopMovingOn()
    {
        Debug.Log("IOnStopMovingOn");
        //		anim.enabled = false;
        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = false;
        isStop = true;
    }

    public void IOnStopMovingOff()
    {
        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = true;
        isStop = false;
    }
    #endregion
}
