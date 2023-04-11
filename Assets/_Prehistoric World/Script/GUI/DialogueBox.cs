using UnityEngine;
using UnityEngine.UI;
using System.Collections;

	public class DialogueBox : MonoBehaviour 
	{
		public Image TextPanel;
		public Text DialogueText;

	    protected Color _backgroundColor;
	    protected Color _textColor;
		
		public virtual void ChangeText(string newText)
		{
			DialogueText.text = newText;
		}
		
		public virtual void ChangeColor(Color backgroundColor, Color textColor)
		{		
			_backgroundColor=backgroundColor;
			_textColor=textColor;
		
			Color newBackgroundColor=new Color(_backgroundColor.r,_backgroundColor.g,_backgroundColor.b,0);
			Color newTextColor=new Color(_textColor.r,_textColor.g,_textColor.b,0);
			
			TextPanel.color=newBackgroundColor;
			DialogueText.color=newTextColor;
		}
		
		public virtual void FadeIn(float duration)
		{	
			if (TextPanel != null) 
			{
				StartCoroutine (MMFade.FadeImage (TextPanel, duration, _backgroundColor));
			}
			if (DialogueText != null) 
			{
				StartCoroutine (MMFade.FadeText (DialogueText, duration, _textColor));
			}
		}
		
		public virtual void FadeOut(float duration)
		{				
			Color newBackgroundColor=new Color(_backgroundColor.r,_backgroundColor.g,_backgroundColor.b,0);
			Color newTextColor=new Color(_textColor.r,_textColor.g,_textColor.b,0);
		
			StartCoroutine(MMFade.FadeImage(TextPanel, duration,newBackgroundColor));
			StartCoroutine(MMFade.FadeText(DialogueText,duration,newTextColor));	
		}
	}
