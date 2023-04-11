using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutEffect : MonoBehaviour {
	public static FadeInOutEffect Instance;
	public SpriteRenderer image;
	 float timeShow = 1;
	 float timeHold = 1;
	 float timeHide = 1;
	Color color;
	IEnumerator fadeWorkCo;
	// Use this for initialization
	void Awake () {
		Instance = this;
		image.gameObject.SetActive (false);
		fadeWorkCo = WorkCo ();
	}

	public void Work(Color _color, float _timeShow = 1, float _timeHold = 1, float _timeHide = 1, bool overPlayer = true){
		if (overPlayer) {
			//			Debug.LogWarning (sprite.sortingLayerID +"/" +SortingLayer.GetLayerValueFromName ("Front"));
			image.sortingLayerName = "Front";
			image.sortingOrder = -10;
		} else {
			image.sortingOrder = -10;
		}

//		Debug.LogError ("WORK");
		timeShow = _timeShow;
		timeHold = _timeHold;
		timeHide = _timeHide;
		color = _color;
		StopCoroutine (fadeWorkCo);
		fadeWorkCo = WorkCo ();
		StartCoroutine(fadeWorkCo);
	}

	IEnumerator WorkCo(){
		image.gameObject.SetActive (true);
		//show
		color.a = 0;
		image.color = color;
		if (timeShow > 0) {
			float counter = 0;
			while(counter < timeShow){
				counter += Time.deltaTime;
				color.a = counter / timeShow;
				color.a = Mathf.Clamp01 (color.a);
				image.color = color;
				yield return 0;
			}
			color.a = 1;
			image.color = color;
		}

		//hold
		yield return new WaitForSeconds(timeHold);

		//hide

		color.a = 1;
		image.color = color;
		if (timeHide > 0) {
			float counter = 0;
			while(counter < timeShow){
				counter += Time.deltaTime;
				color.a = 1 - counter / timeShow;
				color.a = Mathf.Clamp01 (color.a);
				image.color = color;
				yield return 0;
			}
			color.a = 0;
			image.color = color;
		}

		image.gameObject.SetActive (false);
	}
}
