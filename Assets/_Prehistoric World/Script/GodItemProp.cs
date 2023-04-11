using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GodmodeType
{
    Kill, Damage

}
public class GodItemProp : MonoBehaviour
{
    public bool useImmediately = true;
    public int numberAddIfNoUseImmediately = 3;
    public GodmodeType type;
    public float damage = 50;
    public float timeUse = 10;

    public AudioClip sound;

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() == null)
            yield break;

        if (useImmediately)
        {
            if (GameManager.Instance.Player.isUsingActions())
            {
                if (GameManager.Instance.Player.GodMode || FindObjectOfType<Shield>())        //if is flying then allow use this item
                    yield break;
            }

            GameManager.Instance.Player.InitGodmode(type, timeUse, damage);

            BlackScreenUI.instance.Show(0.3f, Color.white);
            yield return new WaitForSeconds(0.1f);
            BlackScreenUI.instance.Hide(0.3f);
        }
        else
            GlobalValue.storeGod += numberAddIfNoUseImmediately;

        SoundManager.PlaySfx(sound);
        gameObject.SetActive(false);
    }
}
