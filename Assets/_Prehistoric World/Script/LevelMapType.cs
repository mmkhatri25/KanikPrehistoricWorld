using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LEVELTYPE { Normal, BossFight }
public enum CONTROLLER { PLATFORM, RUNNER }
public class LevelMapType : MonoBehaviour
{
    public static LevelMapType Instance;
    public LEVELTYPE levelType = LEVELTYPE.Normal;
    private void Awake()
    {
        Instance = this;
    }
}
