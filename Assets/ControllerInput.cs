using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour {
	private SteamVR_TrackedController _controller;
	private GameObject collidingObject; 
	private GameObject objectInHand;
	private GameManager manager;
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		_controller = GetComponent<SteamVR_TrackedController>();
		_controller.TriggerClicked += HandleTriggerClicked;
		_controller.TriggerUnclicked += HandleTriggerUnclicked;

		_controller.PadClicked += HandlePadClicked;
		_controller.Gripped += HandleGripClicked;
	}
	
	private void HandlePadClicked(object sender, ClickedEventArgs e)
	{
		//Collect all items for testing
		manager.Gather();
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		if (this.name.Contains ("left")) {
			manager.Move (true);
		} else if (this.name.Contains ("right") && collidingObject) {
				manager.PutInInventory (collidingObject);
				collidingObject = null;
		}	
	}

	//For constant movement mode
	private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
	{
		if (this.name.Contains ("left")) {
			manager.Move (false);
		}
	}

	private void HandleGripClicked(object sender, ClickedEventArgs e)
	{
		if (this.name.Contains ("left")) 
			manager.ChangeSpeed (0.5f);
		else if (this.name.Contains ("right")) 
			manager.ChangeSpeed (-0.5f);
	}		

	public void OnTriggerEnter(Collider col)
	{
		if (!col.GetComponent<Rigidbody> ()) 
			return;
		
		collidingObject = col.gameObject;
	}

	void OnDestroy() {
		_controller.TriggerClicked -= HandleTriggerClicked;
		_controller.TriggerUnclicked -= HandleTriggerUnclicked;
		_controller.PadClicked -= HandlePadClicked;
		_controller.Gripped -= HandleGripClicked;
	}
}
