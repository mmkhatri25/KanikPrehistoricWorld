using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOverrideParametersChecker : MonoBehaviour {

	[ReadOnlyAttribute]
	public bool useOverrideAcc = false;
	[ReadOnlyAttribute]
	public float accGrounedOverride = 0;
	[ReadOnlyAttribute]
	public float velocityDevide = 1;
	[ReadOnlyAttribute]
	public OverrideParameterZone OPZ = null;

	void OnTriggerStay2D(Collider2D other){
		OverrideParameterZone zone = other.GetComponent<OverrideParameterZone> ();
		if (zone) {
			
			if (zone.isOverridParameter) {
				GameManager.Instance.Player.SetOverrideParameter (zone.overrideParameter, true, zone.zone);
				GameManager.Instance.Player.SetupParameter ();
			}


			if (zone.isOverrideAcceleration && zone.overrideAcc > 0 && zone.overrideAcc < 1) {
				accGrounedOverride = 1f / zone.overrideAcc;
				useOverrideAcc = true;
			}

			if (zone.canWalkOnThis && GameManager.Instance.Player.controller.collisions.hitBelowObj != zone.gameObject)
				useOverrideAcc = false;
			
			velocityDevide = 1;
			if (zone.isOverrideAcceleration && zone.overrideAcc > 1) {
				velocityDevide = 1f / zone.overrideAcc;
			}

			if (zone.isUseAddForce) {
				GameManager.Instance.Player.AddHorizontalForce (zone.forceMoveSpeed);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		OverrideParameterZone zone = other.GetComponent<OverrideParameterZone> ();
		if (zone) {
			if (zone.isOverridParameter) {
				GameManager.Instance.Player.SetOverrideParameter (zone.overrideParameter, false);
				GameManager.Instance.Player.SetupParameter ();
			}
			if (zone.isOverrideAcceleration) {
				useOverrideAcc = false;
				velocityDevide = 1;
            }

			if (zone.isUseAddForce) {
				GameManager.Instance.Player.AddHorizontalForce (0);
			}
		}
	}
}
