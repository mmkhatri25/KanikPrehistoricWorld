using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour {
	public float letterPause = 0.01f;
	public AudioClip typeSound1;
	public AudioClip typeSound2;
	public Image headImage;
	Text text;
	string message;
	string textComp = "";

	public void Init(string _text, Sprite image){
		textComp = _text;
		if (headImage) {
			headImage.transform.gameObject.SetActive (image != null);
			headImage.sprite = image;
		}

		
	}

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		message = textComp;
		text.text = "";
		StartCoroutine(TypeText ());
	}

	IEnumerator TypeText () {
		foreach (char letter in message.ToCharArray()) {
			text.text += letter;
			if (typeSound1 && typeSound2)
//				SoundManager.instance.RandomizeSfx(typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}

		Debug.Log ("Done");
	}
}
