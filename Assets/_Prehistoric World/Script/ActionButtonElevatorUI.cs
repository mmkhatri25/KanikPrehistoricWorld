using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonElevatorUI : MonoBehaviour {
	public static ActionButtonElevatorUI Instance;
	//	public delegate void ClickAction();
	//	public static event ClickAction OnClicked;
	public GameObject buttonUp, buttonDown;
    [HideInInspector]
	public ElevatorTrigger currentElevator;

    private void Awake()
    {
		Instance = this;

	}

    public void SetCurrentElevator(ElevatorTrigger _trigger){
		currentElevator = _trigger;
	}

	public void RemoveCurrentElevator(){
		currentElevator = null;
	}

	void Update(){
		buttonUp.SetActive (currentElevator != null && currentElevator.playerInArea && !currentElevator.elevator.isMoving);
		buttonDown.SetActive (currentElevator != null && currentElevator.playerInArea && !currentElevator.elevator.isMoving);
	}

	public void Up(){
		if (currentElevator != null)
			currentElevator.Up ();
	}

	public void Down(){
		if (currentElevator != null)
			currentElevator.Down ();
	}
}
