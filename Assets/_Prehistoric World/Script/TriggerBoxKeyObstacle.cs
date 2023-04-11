using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxKeyObstacle : MonoBehaviour {
    public GameObject boxKey;
   
   
    public AudioClip sound;
    float rotatePercent = 0;

    public GameObject target;

    List<MonoBehaviour> listMono;
    public bool disableTargetOnStart = true;
    bool isWorked = false;

    // Use this for initialization
    void Start()
    {
        listMono = new List<MonoBehaviour>();
        MonoBehaviour[] monos = target.GetComponents<MonoBehaviour>();
        foreach (var mono in monos)
        {
            listMono.Add(mono);
            mono.enabled = false;
        }
        target.SetActive(!disableTargetOnStart);
    }

    private void LateUpdate()
    {
        if (isWorked)
            return;

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (GameManager.Instance.Player.controller.collisions.above && GameManager.Instance.Player.controller.collisions.ClosestHit && GameManager.Instance.Player.controller.collisions.ClosestHit.collider.gameObject == gameObject)
        {
            StartCoroutine(ActiveCo());
        }
    }

    IEnumerator ActiveCo()
    {
        if (GameManager.Instance.Player.controller.collisions.above && GameManager.Instance.Player.controller.collisions.ClosestHit.collider.gameObject == gameObject)
        {
            isWorked = true;
            SoundManager.PlaySfx(sound);

            target.SetActive(true);
            foreach (var mono in listMono)
            {
                mono.enabled = true;
            }

            yield return new WaitForSeconds(0.1f);
            boxKey.SetActive(false);
        }
    }

    //IEnumerator OnTriggerEnter2D(Collider2D other)
    //{
    //    if (isWorked)
    //        yield break;

    //    if (other.GetComponent<Player>() && GameManager.Instance.Player.controller.collisions.above)
    //    {
    //        isWorked = true;
    //        SoundManager.PlaySfx(sound);

    //        target.SetActive(true);
    //        foreach (var mono in listMono)
    //        {
    //            mono.enabled = true;
    //        }

    //        yield return new WaitForSeconds(0.1f);
    //        boxKey.SetActive(false);
    //    }
    //}

    void OnDrawGizmos()
    {
        if (target)
            Gizmos.DrawLine(transform.position, target.transform.position);
    }
}
