using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletupDownManager : MonoBehaviour, IListener
{

    public LaserBulletUpDown laserBullet;
    public Transform[] Points;

    public float delaySpawn = 1;
    public float delayAttack = 1;

    Vector2 directAttack = new Vector2(1, -1);
    int numberBullet = 1;

    // Use this for initialization
    IEnumerator Start()
    {
        GameManager.Instance.listeners.Add(this);
        //if (GameManager.Instance.Player.inverseGravity)
        //{
        //    transform.localScale = new Vector3(1, -1, 1);
        //}
        for (int i = 0; i < Points.Length; i++)
        {
            while (isStop)
            {
                yield return new WaitForEndOfFrame();
            }
            LaserBulletUpDown obj = (LaserBulletUpDown)Instantiate(laserBullet, transform.position, Quaternion.identity);
            obj.MoveToFirstTarget(Points[i], delayAttack, directAttack);
            yield return new WaitForSeconds(delaySpawn);
            Debug.Log("Spawn: " + i);

            if (i + 1 >= numberBullet)
                break;
        }
    }

    public void Init(Vector2 _dirAttack, int _numberBullet)
    {
        directAttack = _dirAttack;
        numberBullet = _numberBullet;
    }

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
