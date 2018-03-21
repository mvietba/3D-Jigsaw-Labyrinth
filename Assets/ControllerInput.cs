using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour {
	private SteamVR_TrackedController _controller;
	private GameObject collidingObject; 
	private GameObject objectInHand;

	// Use this for initialization
	void Start () {
		_controller = GetComponent<SteamVR_TrackedController>();
		_controller.TriggerClicked += HandleTriggerClicked;

		_controller.PadClicked += HandlePadClicked;
		_controller.Gripped += HandleGripClicked;
	}
	
	private void HandlePadClicked(object sender, ClickedEventArgs e)
	{

	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		if (collidingObject)
		{
			//If the object is one of the cubes
			//Put object in the inventory
		}
	}

	private void HandleGripClicked(object sender, ClickedEventArgs e)
	{

		if (this.name.Contains ("left"))
			Debug.Log ("Left");
			else if (this.name.Contains("right"))
			Debug.Log ("Right");
	}

	/* Ray Wenderlich's tutorial: https://www.raywenderlich.com/149239/htc-vive-tutorial-unity */

	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	private void SetCollidingObject(Collider col)
	{
		// 1
		if (collidingObject || !col.GetComponent<Rigidbody>())
			return;
		// 2
		collidingObject = col.gameObject;
	}		
}
