using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemActionUI : MonoBehaviour, IListener
{
    public static ItemActionUI Instance;
    public GameObject container;

    [Header("God mode")]
    public Button godBut;
    public Text godTxt;
    public GodmodeType type;
    public float damage = 50;
    public float timeUse = 10;

    public Image godImage;
    public float godCoolDown = 3f;
    float godCoolDownCounter = 0;

    void GodCoolDownTimer()
    {
        godCoolDownCounter -= Time.deltaTime;

        godImage.fillAmount = Mathf.Clamp01((godCoolDown - godCoolDownCounter) / godCoolDown);

        godBut.interactable = godCoolDownCounter <= 0;
    }

    [Header("Shield mode")]
    public Button shieldBut;
    public Text shieldTxt;

    public EffectNo effectType;
    public float shieldTime = 7;
    public int shieldHits = 3;
    public GameObject Shield;
    public GameObject shieldHitFX;

    public Image shieldImage;
    public float shieldCoolDown = 3f;
    float shieldCoolDownCounter = 0;

    public CanvasGroup canvasGroup;
    //public CanvasGroup canvasGroupGodBut;
    //public CanvasGroup canvasGroupShieldBut;

    void ShieldCoolDownTimer()
    {
        shieldCoolDownCounter -= Time.deltaTime;

        shieldImage.fillAmount = Mathf.Clamp01((shieldCoolDown - shieldCoolDownCounter) / shieldCoolDown);

        shieldBut.interactable = shieldCoolDownCounter <= 0;
    }

    // Update is called once per frame
    void Update()
    {
        container.SetActive(GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone"));

        godBut.gameObject.SetActive(GlobalValue.storeGod > 0);
        shieldBut.gameObject.SetActive(GlobalValue.storeShield > 0);
        godTxt.text = GlobalValue.storeGod.ToString();
        shieldTxt.text = GlobalValue.storeShield.ToString();

        if (!GameManager.Instance.Player.isUsingActions())
        {
            //if (Input.GetKeyDown( DefaultValueKeyboard.Instance.ShieldItem) && (GlobalValue.storeShield > 0))
            //    ActiveShield();

        }

        GodCoolDownTimer();
        ShieldCoolDownTimer();

        canvasGroup.interactable = !GameManager.Instance.Player.isUsingActions();
        canvasGroup.alpha = GameManager.Instance.isInDialogue ? 0 : 1;
    }

    IEnumerator Blinking()
    {
        BlackScreenUI.instance.Show(0.3f, Color.white);
        yield return new WaitForSeconds(0.1f);
        BlackScreenUI.instance.Hide(0.3f);
    }

    /// <summary>
    /// Actives the god.
    /// </summary>
    public void ActiveGod()
    {
        if (GameManager.Instance.Player.GodMode)
            return;

        if (GameManager.Instance.Player.isSlidingInTurnel)
            return;

        GlobalValue.storeGod--;
        godCoolDownCounter = godCoolDown;
        GameManager.Instance.Player.InitGodmode(type, timeUse, damage);

        StartCoroutine(Blinking());
    }

    /// <summary>
    /// Actives the shield.
    /// </summary>
    public void ActiveShield()
    {
        if (FindObjectOfType<Shield>())
            return;

        if (GameManager.Instance.Player.isSlidingInTurnel)
            return;

        GlobalValue.storeShield--;
        shieldCoolDownCounter = shieldCoolDown;

        if (Shield)
        {
            int effect = 1;
            if (effectType == EffectNo.Effect1)
                effect = 1;
            else if (effectType == EffectNo.Effect2)
                effect = 2;
            else if (effectType == EffectNo.Effect3)
                effect = 3;
            Instantiate(Shield, transform.position, Quaternion.identity).GetComponent<Shield>().Init(shieldTime, shieldHits, shieldHitFX, effect);
        }
        else
            Debug.LogError("Place the Shield in" + gameObject.name);
    }

    public void StopMovingEnemyOn()
    {
        Debug.Log("OnTriggerEnter2DIOnStopMovingOn");
        List<IListener> listeners = new List<IListener>();
        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            listeners.Add(_listener);
        }

        foreach (var item in listeners)
        {
            item.IOnStopMovingOn();
        }

        GameManager.Instance.ActiveStopTimer(true);
    }

    public void StopMovingEnemyOff()
    {
        List<IListener> listeners = new List<IListener>();
        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            listeners.Add(_listener);
        }

        foreach (var item in listeners)
        {
            item.IOnStopMovingOff();
        }

        GameManager.Instance.ActiveStopTimer(false);
    }

    public void IPlay()
    {

    }

    public void ISuccess()
    {

    }

    public void IPause()
    {

    }

    public void IUnPause()
    {

    }

    public void IGameOver()
    {
        if (GameManager.Instance.isStopTimerActivating)
        {
            StopMovingEnemyOff();
        }
    }

    public void IOnRespawn()
    {

    }

    public void IOnStopMovingOn()
    {

    }

    public void IOnStopMovingOff()
    {

    }
}
