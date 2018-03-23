using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private int collectedCount;
	public GameObject piecesHolder;
	public GameObject player;
	public GameObject cameraRig;
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
			if (collectedCount == 1) {//piecesCount
				OnAllPiecesGathered ();
			}
		}
	}

	public void ChangeSpeed(float modifier) {
		speed += modifier;
		speed = Mathf.Max (speed, 1.0f); //speed should not be too low
	}
	private void OnAllPiecesGathered()
	{
		Debug.Log ("All pieces collected!");
		for (int i = 0; i < piecesCount; i++) {
			Transform childTransform = piecesHolder.transform.GetChild(i);
			childTransform.position = new Vector3(cameraRig.transform.position.x + Random.Range(0.0f, 5.0f), 
				cameraRig.transform.position.y, cameraRig.transform.position.z + Random.Range(0.0f, 5.0f));
			childTransform.gameObject.SetActive (true);
			childTransform.gameObject.GetComponent<Rotator> ().enabled = false;
			childTransform.gameObject.GetComponent<Rigidbody>().useGravity = true;
			childTransform.gameObject.GetComponent<Rigidbody>().isKinematic = false;

			//Debug.Log (childTransform.position);
			//Debug.Log (cameraRig.transform.position);
			//Debug.Log (player.transform.position);
		}
	}

	//private void UpdateText() {
	//	scoreTxt.text = "Remaining: " + (piecesCount - collectedCount).ToString ();
	//}
}
