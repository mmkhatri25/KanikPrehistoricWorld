using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultValueKeyboard : MonoBehaviour {
	public static DefaultValueKeyboard Instance;

	[Header("Keyboard")]
	public KeyCode Left = KeyCode.LeftArrow;
	public KeyCode Right = KeyCode.RightArrow;
	public KeyCode Up = KeyCode.UpArrow;
	public KeyCode Down = KeyCode.DownArrow;
	public KeyCode Jump = KeyCode.Space;
	public KeyCode Shooting = KeyCode.K;
    public KeyCode Melee = KeyCode.L;
	public KeyCode Pause = KeyCode.Escape;
	public KeyCode Cannon = KeyCode.C;
	void Awake(){
		Instance = this;
	}
}
