using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class _LevelManager : MonoBehaviour {
	public static _LevelManager Instance{ get; private set;}
    public GameObject testLevelMap;

	CameraFollow Camera;

    void Awake()
    {
        Instance = this;

        if (FindObjectOfType<LevelMapType>())
        {
            Debug.LogError("Notice: There are a Level on this scene!");
            return;
        }
            if (testLevelMap)
                Instantiate(testLevelMap, Vector2.zero, Quaternion.identity); 
    }

	void Start () {
		Camera = FindObjectOfType<CameraFollow> ();
	}
}
