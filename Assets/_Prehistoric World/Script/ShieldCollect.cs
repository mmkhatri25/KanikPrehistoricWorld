using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectNo { Effect1, Effect2, Effect3 }
public class ShieldCollect : MonoBehaviour
{
    public bool useImmediately = true;
    public int numberAddIfNoUseImmediately = 3;

    public EffectNo effectType;
    public float time = 7;
    public int hits = 3;
    public GameObject Shield;
    public GameObject hitFX;

    public AudioClip soundCollect;
    public GameObject collectFX;

    bool isWork = false;

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (isWork)
            yield break;

        if (other.GetComponent<Player>())
        {
            if (useImmediately)
            {
                if (GameManager.Instance.Player.isUsingActions())
                {
                    if (GameManager.Instance.Player.GodMode || FindObjectOfType<Shield>())        //if is flying then allow use this item
                        yield break;
                }

                if (Shield)
                {
                    int effect = 1;
                    if (effectType == EffectNo.Effect1)
                        effect = 1;
                    else if (effectType == EffectNo.Effect2)
                        effect = 2;
                    else if (effectType == EffectNo.Effect3)
                        effect = 3;
                    Instantiate(Shield, transform.position, Quaternion.identity).GetComponent<Shield>().Init(time, hits, hitFX, effect);
                }
                else
                    Debug.LogError("Place the Shield in" + gameObject.name);

                BlackScreenUI.instance.Show(0.3f, Color.white);
                yield return new WaitForSeconds(0.1f);
                BlackScreenUI.instance.Hide(0.3f);
            }
            else
            {
                GlobalValue.storeShield += numberAddIfNoUseImmediately;
                //ItemActionUI.Instance.effectType = effectType;
                //				FindObjectOfType<ShieldUI> ().AddAmount (1, time, hits, useItImediately, hitFX);
            }

            isWork = true;
            SoundManager.PlaySfx(soundCollect);



            Destroy(gameObject);
        }
    }
}
