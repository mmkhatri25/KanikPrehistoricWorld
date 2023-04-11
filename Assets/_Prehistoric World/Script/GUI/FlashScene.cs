using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlashScene : MonoBehaviour {

	public string sceneLoad = "scene name";
	public float delay = 2;

	// Use this for initialization
	void Start () {
		StartCoroutine (LoadSceneCo ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator LoadSceneCo(){
		yield return new WaitForSeconds (delay);
		SceneManager.LoadSceneAsync (sceneLoad);
	}
}
