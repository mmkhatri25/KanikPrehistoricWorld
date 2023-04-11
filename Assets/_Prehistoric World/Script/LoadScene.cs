using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public float delay = 1;
    public string loadSceneName = "MainMenu";

    void Start()
    {
        StartCoroutine(LoadSceneCo());
    }

    IEnumerator LoadSceneCo()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(loadSceneName);
    }
}
