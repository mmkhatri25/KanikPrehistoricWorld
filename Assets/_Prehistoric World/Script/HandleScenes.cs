using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleScenes : MonoBehaviour {
	public string sceneName = "name";

	public Camera main;
	public SoundManager sound;
	public GameObject menu;
	public GameObject UIEvent;
	// Use this for initialization
	void Awake () {
		main.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		sound.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		menu.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		UIEvent.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
	}
	
	// Update is called once per frame
	void Update () {
		main.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		sound.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		menu.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
		UIEvent.gameObject.SetActive (string.Compare (SceneManager.GetActiveScene ().name, sceneName) == 0);
	}
}
