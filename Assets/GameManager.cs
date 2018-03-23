using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private int collectedCount;
	public GameObject piecesHolder;
	public GameObject player;
	public GameObject cameraRig;
	public GameObject leftController;
	public GameObject rightController;
	//public Text scoreTxt;
	private int piecesCount;
	private float speed;
	private bool movement;
	// Use this for initialization
	void Start () {
		movement = false;
		speed = 1.0f;
		collectedCount = 0;
		piecesCount = piecesHolder.transform.childCount;
		//UpdateText ();
	}
	
	// Update is called once per frame
	void Update() {
		if (movement)
			cameraRig.transform.position += new Vector3(player.transform.forward.x * Time.deltaTime * speed, 0, player.transform.forward.z * Time.deltaTime * speed);
	}
	public void Move(bool m) {
		movement = m; // constant movement on trigger pressed
		//movement = ! movement; //constant movement on/off
	}

	public void PutInInventory(GameObject piece)
	{
		if (piece.transform.IsChildOf (piecesHolder.transform)) {
			piece.SetActive (false);
			collectedCount += 1;
			Debug.Log (collectedCount);
			if (collectedCount == piecesCount) {
				OnAllPiecesGathered ();
			}
		}
	}

	public void ChangeSpeed(float modifier) {
		speed += modifier;
		speed = Mathf.Max (speed, 1.0f); //speed should not be too low
	}

	public void Gather() {
		collectedCount = piecesCount;
		OnAllPiecesGathered ();
	}

	private void OnAllPiecesGathered()
	{
		Debug.Log ("All pieces collected!");
		for (int i = 0; i < piecesCount; i++) {
			Transform childTransform = piecesHolder.transform.GetChild(i);
			childTransform.position = new Vector3(player.transform.position.x + Random.Range(0.0f, 3.0f), 
				player.transform.position.y * 0.75f, player.transform.position.z + Random.Range(0.0f, 3.0f));
			childTransform.gameObject.SetActive (true);
			childTransform.gameObject.GetComponent<Rotator> ().enabled = false;
		}
		Destroy (leftController.GetComponent<ControllerInput> ());
		Destroy (rightController.GetComponent<ControllerInput> ());

		//Enable new interaction script
		leftController.GetComponent<ControllerGrabObject> ().enabled = true;
		rightController.GetComponent<ControllerGrabObject> ().enabled = true;

	}

	public void TryToSnap(GameObject piece) {
		//For all other children, check if any of the neighbours is within a small threshold of distance and small orientation difference
		//If true, snap them together
		// Copy orientation + place in a correct distance, parallel to floor
		//add to the same parent (in correct coordinates, extract from name)
	}
	//private void UpdateText() {
	//	scoreTxt.text = "Remaining: " + (piecesCount - collectedCount).ToString ();
	//}
}
