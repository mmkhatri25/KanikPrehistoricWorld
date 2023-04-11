using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletButtonAvailableUI : MonoBehaviour {
	public enum BulletType
		{Normal, Power, Track, Spread, Laser, Dart}

	public BulletType bulletType;

//	public bool isAvailable = true;

	public bool isAvailable(){
		bool available = false;
		switch (bulletType) {
		case BulletType.Normal:
			available = GlobalValue.normalBullet > 0;
			break;
		default:
			break;
		}

		return available;
	}
}
