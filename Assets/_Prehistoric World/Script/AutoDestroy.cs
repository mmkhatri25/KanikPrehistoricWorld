using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
	public float time = 1;
	public bool onlyDisactivity = true;

	// Use this for initialization
	IEnumerator Start()
	{
		yield return new WaitForSeconds(time);
		if (onlyDisactivity)
			gameObject.SetActive(false);
		else
			Destroy(gameObject, time);
	}
}