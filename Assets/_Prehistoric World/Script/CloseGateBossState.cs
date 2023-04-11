using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGateBossState : MonoBehaviour {
    public bool isClosed = true;

    public void UpdateState(bool state)
    {
        isClosed = state;
    }
}
