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
		manager.OnTouchPadPressed ();
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		manager.OnTriggerBtnPressed (this.name, collidingObject);
		if (this.name.Contains ("right") && collidingObject)
				collidingObject = null;
	}

	private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
	{
		manager.OnTriggerBtnReleased (this.name);
	}

	private void HandleGripClicked(object sender, ClickedEventArgs e)
	{
		manager.OnGripPressed (this.name);
	}		

	public void OnTriggerEnter(Collider col)
	{
		if (manager.collectMode) {
			if (!col.GetComponent<Rigidbody> ())
				return;
			collidingObject = col.gameObject;
		}
	}

	void OnDestroy() {
		_controller.TriggerClicked -= HandleTriggerClicked;
		_controller.TriggerUnclicked -= HandleTriggerUnclicked;
		_controller.PadClicked -= HandlePadClicked;
		_controller.Gripped -= HandleGripClicked;
	}
}
