using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : InformationSign
{
	public Elevator elevator;
	public GameObject UpButton,DownButton;

    public override void Start()
    {
        base.Start();
	}

    // Update is called once per frame
    void Update () {
		UpButton.SetActive (playerInArea && !elevator.isMoving);
		DownButton.SetActive (playerInArea && !elevator.isMoving);

		if (playerInArea && ActionButtonElevatorUI.Instance.currentElevator != this)
			ActionButtonElevatorUI.Instance.SetCurrentElevator(this);
		else if(!playerInArea && ActionButtonElevatorUI.Instance.currentElevator == this)
			ActionButtonElevatorUI.Instance.RemoveCurrentElevator();
	}

	public void Up(){
		elevator.Up ();
	}

	public void Down(){
		elevator.Down ();
	}
}
