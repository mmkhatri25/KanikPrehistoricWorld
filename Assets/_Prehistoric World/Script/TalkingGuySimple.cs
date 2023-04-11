using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingGuySimple : ButtonActivated
{
    public string[] Dialogue;

    Color TextBackgroundColor = Color.black;
    Color TextColor = Color.white;
    float FadeDuration = 0.2f;
    float TransitionTime = 0.2f;
    [Header("Box Position")]
    Vector2 dialogueLocalPosition = new Vector2(0, 2f);
    bool ButtonHandled = true;
    bool ActivableMoreThanOnce = true;
    float InactiveTime = 3f;

    protected DialogueBox _dialogueBox;
    protected bool _activated = false;
    protected bool _playing = false;
    protected int _currentIndex;
    [HideInInspector] public bool _activable = true;
    [HideInInspector] public bool readyToTalk = true;

    public override bool CanShowPrompt()
    {
        if ((chatIcon == null) && _activable && !_playing)
        {
            return true;
        }
        return false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentIndex = 0;
    }

    public override void TriggerButtonAction()
    {
        if (!readyToTalk)
            return;

        if (_playing && !ButtonHandled)
        {
            return;
        }

        GameManager.Instance.isInDialogue = true;
        StartDialogue();
    }

    public virtual void StartDialogue()
    {
        if (chatIcon != null)
            chatIcon.SetActive(false);

        if (_activated && !ActivableMoreThanOnce)
            return;

        if (!_activable)
            return;

        GameManager.Instance.Player.allowMoving = false;

        if (!_playing)
        {
            GameObject dialogueObject = (GameObject)Instantiate(Resources.Load("GUI/DialogueBox"));
            _dialogueBox = dialogueObject.GetComponent<DialogueBox>();

            _dialogueBox.transform.position = transform.position + (Vector3)dialogueLocalPosition;

            _dialogueBox.ChangeColor(TextBackgroundColor, TextColor);

            _playing = true;
        }
        StartCoroutine(PlayNextDialogue());
        isFinishedTalking = false;
    }

    public bool isFinishedTalking { get; set; }
    [HideInInspector]
    public bool manualDisableObj = false;
    protected virtual IEnumerator PlayNextDialogue()
    {
        if (_dialogueBox == null)
        {
            yield return null;
        }

        if (_currentIndex != 0)
        {
            _dialogueBox.FadeOut(FadeDuration);
            yield return new WaitForSeconds(TransitionTime);
        }

        if (_currentIndex >= Dialogue.Length)
        {
            isFinishedTalking = true;
            _currentIndex = 0;
            Destroy(_dialogueBox.gameObject);
            _activated = true;
            GameManager.Instance.Player.allowMoving = true;

            GameManager.Instance.isInDialogue = false;
            if (!manualDisableObj)
                transform.parent.gameObject.SetActive(false);
            if (ActivableMoreThanOnce)
            {
                _activable = false;
                _playing = false;
                StartCoroutine(Reactivate());
            }
            else
            {
                if (!manualDisableObj)
                    gameObject.SetActive(false);
            }
            yield break;
        }

        if (_dialogueBox.DialogueText != null)
        {
            _dialogueBox.FadeIn(FadeDuration);
            _dialogueBox.DialogueText.text = Dialogue[_currentIndex];
        }

        _currentIndex++;
    }

    protected virtual IEnumerator Reactivate()
    {
        readyToTalk = false;
        yield return new WaitForSeconds(InactiveTime);
        _activable = true;
        _playing = false;
        _currentIndex = 0;

            ShowPrompt();
        readyToTalk = true;
    }


    //------
    public enum FinishTalk
    {
        Disappear, Stand
    }

    public FinishTalk finishTalk;

    public float moveWhenTalkDone = 10;

    bool isFacingRight() { return (transform.rotation.eulerAngles.y == 0 ? true : false); }

    void Start()
    {
        manualDisableObj = true;       //allow control dialogueZone
    }
    public override void Update()
    {
        base.Update();

        if (isFacingRight() && (transform.position.x > GameManager.Instance.Player.transform.position.x))
            ;
        else if (!isFacingRight() && (transform.position.x < GameManager.Instance.Player.transform.position.x))
            ;

        else if (isFinishedTalking)
        {
            if (finishTalk == FinishTalk.Stand)
                ;
            else if (finishTalk == FinishTalk.Disappear)
                gameObject.SetActive(false);
        }
    }
  
    void Flip()
    {
        transform.rotation = Quaternion.Euler(0, isFacingRight() ? 180 : 0, 0);
    }
}