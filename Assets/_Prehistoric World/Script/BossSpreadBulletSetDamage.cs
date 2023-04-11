using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpreadBulletSetDamage : MonoBehaviour {
    public int damage = 30;
	// Use this for initialization
	void Start () {
        GiveDamageToPlayer[] childBullet = gameObject.GetComponentsInChildren<GiveDamageToPlayer>(true);
        foreach(var child in childBullet)
        {
            child.DamageToPlayer = damage;
        }

    }
}
