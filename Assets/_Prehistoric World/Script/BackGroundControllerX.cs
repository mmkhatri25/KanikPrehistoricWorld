using UnityEngine;
using System.Collections;

public class BackGroundControllerX : MonoBehaviour {
	
	public enum Follow{FixedUpdate,Update}
	public Follow timeBase;
	public Renderer Background;
	public float speedBG;
	public Renderer Midground;
	public float speedMG;
	public Renderer Forceground;
	public float speedFG;

	Camera target;
	float startPosX;

	// Use this for initialization
	void Start () {
		target = Camera.main;
		startPosX = target.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeBase != Follow.Update)
			return;
		
		var x = target.transform.position.x - startPosX;

		if (Background != null) {
			var offset = (x * speedBG) % 1;
			Background.material.mainTextureOffset = new Vector2 (offset, Background.material.mainTextureOffset.y);
		}
		if (Midground != null) {
			var offset = (x * speedMG) % 1;
			Midground.material.mainTextureOffset = new Vector2 (offset, Midground.material.mainTextureOffset.y);
		}
		if (Forceground != null) {
			var offset = (x * speedFG) % 1;
			Forceground.material.mainTextureOffset = new Vector2 (offset, Forceground.material.mainTextureOffset.y);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (timeBase != Follow.FixedUpdate)
			return;

		var x = target.transform.position.x - startPosX;

		if (Background != null) {
			var offset = (x * speedBG) % 1;
			Background.material.mainTextureOffset = new Vector2 (offset, Background.material.mainTextureOffset.y);
		}
		if (Midground != null) {
			var offset = (x * speedMG) % 1;
			Midground.material.mainTextureOffset = new Vector2 (offset, Midground.material.mainTextureOffset.y);
		}
		if (Forceground != null) {
			var offset = (x * speedFG) % 1;
			Forceground.material.mainTextureOffset = new Vector2 (offset, Forceground.material.mainTextureOffset.y);
		}
	}
}
