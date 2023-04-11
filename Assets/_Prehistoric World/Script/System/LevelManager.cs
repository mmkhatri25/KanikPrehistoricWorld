using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LevelManager : MonoBehaviour, IListener
{
    public static LevelManager Instance { get; private set; }
    public GameObject testLevelMap;
    void Awake()
    {
        Instance = this;

        if (FindObjectOfType<LevelMapType>())
        {
            Debug.LogError("Notice: There are a Level on this scene!");
            return;
        }

        if (GlobalValue.levelPlaying != -1)
        {
            //Instantiate(LevelMaps[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
            var _go = Resources.Load("Level/LevelMap/Level Map " + GlobalValue.levelPlaying) as GameObject;
            Instantiate(_go, Vector2.zero, Quaternion.identity);
        }
        else
        {
            if (testLevelMap)
                Instantiate(testLevelMap, Vector2.zero, Quaternion.identity);
        }
    }

    void Start()
    {
        StartCoroutine(BeginGameAfterCo(0.1f));
    }

    IEnumerator BeginGameAfterCo(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void StartGame()
    {

    }

    public void KillPlayer()
    {
        GameManager.Instance.Player.Kill();
        GameManager.Instance.GameOver();
    }

    public void GotoCheckPoint()
    {
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
