using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ButtonActivated : MonoBehaviour
{
	 bool AutoActiveFirstTime = true;
	bool AlwaysShowPrompt = true;
	bool ShowPromptWhenColliding = true;

	public GameObject chatIcon;
	public GameObject chatGuideObj;
	[ReadOnly] public bool isPlayerInZone = false;
	public AudioClip soundBegin;
	public virtual bool CanShowPrompt()
	{
		if (chatIcon == null)
		{
			return true;
		}
		return false;
	}

	protected virtual void OnEnable()
	{
		if (AlwaysShowPrompt)
		{
			ShowPrompt();
		}

        ControllerInput.pressUpButton += ControllerInput_pressUpButton;
	}

    private void OnDisable()
    {
		ControllerInput.pressUpButton -= ControllerInput_pressUpButton;
	}

	private void ControllerInput_pressUpButton()
	{
        if (isPlayerInZone)
        {
			ShowDialogue();
		}
	}

	void ShowDialogue()
    {
		TriggerButtonAction();
		SoundManager.PlaySfx(soundBegin);
		if (CanShowPrompt() && ShowPromptWhenColliding)
		{
			ShowPrompt();
		}
	}

	 public virtual void Update()
    {
		if (Physics2D.CircleCast(transform.position, 1.5f, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Player")))
		{
			isPlayerInZone = true;
			if (AutoActiveFirstTime)
            {
				AutoActiveFirstTime = false;
				ShowDialogue();
			}
		}
		else
			isPlayerInZone = false;

		chatGuideObj.SetActive(isPlayerInZone && chatIcon.activeInHierarchy);

	}

    public virtual void TriggerButtonAction()
	{

	}

	public virtual void ShowPrompt()
	{
		// we add a blinking A prompt to the top of the zone
		chatIcon.SetActive(true);
	}

	public virtual IEnumerator HidePrompt()
	{
		yield return new WaitForSeconds(0.3f);
		chatIcon.SetActive(false);
	}
}