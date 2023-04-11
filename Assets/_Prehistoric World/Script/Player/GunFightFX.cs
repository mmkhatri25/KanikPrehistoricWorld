using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFightFX : MonoBehaviour {
	public Animator animFX;

    public void Fire()
    {
       animFX.SetTrigger("fire");
    }
}
