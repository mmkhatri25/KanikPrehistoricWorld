using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public AudioClip sound;
	public bool Shaking; 
	public float shakeDecay = 0.02f;
	public float shakeIntensity = 0.3f;    
	public float wide = 0.2f;
	float ShakeDecay = 0;
	float ShakeIntensity = 0;
	private Vector3 OriginalPos;
	private Quaternion OriginalRot;
	public GameObject Target;
	bool isWorking = false;
	public float timeShake = 1;
	public float timeRate = 1;
	public bool auto=false;
	void Awake(){
		if (Target == null)
			Target = gameObject;

		Shaking = false;   
	}

	public virtual void Start()
	{
		isWorking = true;
		if (auto)
			InvokeRepeating ("DoShake", timeShake, timeRate);
	}

	void OnDisable(){
		StopShake ();
	}

	public void StopShake(){
		isWorking = false;
		CancelInvoke ();
//		Debug.LogError ("StopShake");
	}

	public void DoShakeManually(){
//		if (isWorking)
//			return;

//		Debug.LogError ("DoShakeManually");
//		Debug.LogError("DoShakeManually");
		isWorking = true;
		InvokeRepeating ("DoShake", timeShake, timeRate);
	}

	public void DoShakeCustom(float _timeShake, float _timeRate){
		isWorking = true;
		InvokeRepeating ("DoShake", _timeShake, _timeRate);
	}


	// Update is called once per frame
	void Update () 
	{
		if(ShakeIntensity > 0)
		{
			Target.transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
			Target.transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity)*wide,
				OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity)*wide,
				OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity)*wide,
				OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity)*wide);

			ShakeIntensity -= ShakeDecay;
		}
		else if (Shaking)
		{
			Shaking = false;  
		}
	}


//	void OnGUI() {
//
//		if (GUI.Button(new Rect(10, 200, 50, 30), "Shake"))
//			DoShake();
//		Debug.Log("Shake");
//
//	}        

	public void DoShake()
	{
		isWorking = true;
		SoundManager.PlaySfx (sound);
		OriginalPos = Target.transform.position;
		OriginalRot = Target.transform.rotation;

		ShakeIntensity = shakeIntensity;
		ShakeDecay = shakeDecay;
		Shaking = true;
	}    


}