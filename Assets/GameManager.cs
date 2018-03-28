using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject pieces;
	public GameObject holders;
	public GameObject player;
	public GameObject cameraRig;
	public GameObject leftController;
	public GameObject rightController;
	public Text scoreTxt;
	public Text endTxt;
	private float speed;
	private bool movement;
	private int piecesAmt;
	private int collectedCnt;
	private int correctPiecesCnt;
	private Vector3 defaultPos;
	public float posThreshold;
	public float rotThreshold;
	private List<string> snappedList;
	private int notKinematicMax;
	// Use this for initialization
	void Start () {
		movement = false;
		speed = 1.0f;
		collectedCnt = 0;
		correctPiecesCnt = 0;
		piecesAmt = pieces.transform.childCount;
		snappedList = new List<string> ();
		posThreshold = 0.25f;
		rotThreshold = 10f;
		notKinematicMax = 10;
		defaultPos = cameraRig.transform.position;
		Physics.defaultSolverIterations = 100;
		Physics.defaultSolverVelocityIterations = 50;
		scoreTxt.text = "Remaining: " + piecesAmt.ToString ();
		//UpdateText ();
	}
	
	// Update is called once per frame
	void Update() {
		if (movement)
			cameraRig.transform.position += new Vector3(player.transform.forward.x * Time.deltaTime * speed, 0, player.transform.forward.z * Time.deltaTime * speed);

		if (correctPiecesCnt == piecesAmt)
			endTxt.enabled = true;
	}
	public void Move(bool m) {
		movement = m; // constant movement on trigger pressed
		//movement = ! movement; //constant movement on/off
	}

	public void PutInInventory(GameObject piece)
	{
		if (piece.transform.IsChildOf (pieces.transform)) {
			piece.SetActive (false);
			collectedCnt += 1;
			UpdateText ();
			if (collectedCnt == piecesAmt) {
				OnAllPiecesGathered ();
			}
		}
	}

	public void ChangeSpeed(float modifier) {
		speed += modifier;
		speed = Mathf.Max (speed, 1.0f); //speed should not be too low
	}

	public void Gather() {
		collectedCnt = piecesAmt;
		OnAllPiecesGathered ();
	}

	private void OnAllPiecesGathered()
	{
		Debug.Log ("All pieces collected!");
		scoreTxt.enabled = false;
		cameraRig.transform.position = defaultPos;
		for (int i = 0; i < piecesAmt; i++) {
			Transform childTf = pieces.transform.GetChild(i);
			childTf.position = new Vector3(player.transform.position.x + Random.Range(-1.0f, 1.0f), 
				player.transform.position.y * 0.75f, player.transform.position.z + Random.Range(-1.0f, 1.0f));
			childTf.gameObject.SetActive (true);
			Destroy (childTf.gameObject.GetComponent<Rotator> ()); //.enabled = false;
		}
		Destroy (leftController.GetComponent<ControllerInput> ());
		Destroy (rightController.GetComponent<ControllerInput> ());

		//Enable new interaction script
		leftController.GetComponent<ControllerGrabObject> ().enabled = true;
		rightController.GetComponent<ControllerGrabObject> ().enabled = true;

	}

	public void ChangeKinematic(GameObject piece) {
		bool isKinematic = true;
		if (piece != null && correctPiecesCnt <= notKinematicMax) {
			if (snappedList.Contains (piece.name)) {
				isKinematic = false;
			}
		}

		holders.GetComponent<Rigidbody> ().isKinematic = isKinematic;

	}
	public void TryToSnap(GameObject piece) {
		//if the piece was not already snapped
		if (!snappedList.Contains (piece.name)) {
			Transform holderTf = holders.transform.Find (piece.name); //Find transform of a holder with the same name

			if (correctPiecesCnt == 0) { //If first piece
				//Match rotation
				holders.transform.rotation = piece.transform.rotation;

				//Move the holder by the difference
				Vector3 offset = piece.transform.position - holderTf.position;
				holders.transform.position = holders.transform.position + offset;

				//create a fixed joint between the piece and the big holder
				Snap(piece, holderTf);

			} else {
				//Calculate difference between xyz and orientations of its holder
				//float xRotDiff = Mathf.DeltaAngle(piece.transform.eulerAngles.x, holderTf.eulerAngles.x);//piece.transform.eulerAngles.x - holderTf.eulerAngles.x;
				//float yRotDiff = Mathf.DeltaAngle(piece.transform.eulerAngles.y, holderTf.eulerAngles.y);//piece.transform.eulerAngles.y - holderTf.eulerAngles.y;
				//float zRotDiff = Mathf.DeltaAngle(piece.transform.eulerAngles.z, holderTf.eulerAngles.z);//piece.transform.eulerAngles.z - holderTf.eulerAngles.z;
				float angleDiff = Quaternion.Angle(piece.transform.rotation, holderTf.rotation);
				float xDiff = piece.transform.position.x - holderTf.position.x;
				float yDiff = piece.transform.position.y - holderTf.position.y;
				float zDiff = piece.transform.position.z - holderTf.position.z;

				//if piece is within a small threshold from its holder
				//if (Mathf.Abs (xDiff) < posThreshold && Mathf.Abs (yDiff) < posThreshold && Mathf.Abs (zDiff) < posThreshold
				//    && Mathf.Abs (xRotDiff) < rotThreshold && Mathf.Abs (yRotDiff) < rotThreshold && Mathf.Abs (zRotDiff) < rotThreshold) {
				  if (Mathf.Abs (xDiff) < posThreshold && Mathf.Abs (yDiff) < posThreshold && Mathf.Abs (zDiff) < posThreshold
					&& Mathf.Abs(angleDiff) < rotThreshold) {
					//assign the coordinates and orientation of the holder
					piece.transform.position = holderTf.transform.position;
					piece.transform.rotation = holderTf.transform.rotation;
					Snap (piece, holderTf);
				} else {
					//Debug.Log ("x" + xDiff.ToString () + ", y " + yDiff.ToString () + ", z " + zDiff.ToString () + ", xRot " + xRotDiff.ToString () + ", yRot " + yRotDiff.ToString () + ", zRot " + zRotDiff);
					Debug.Log ("x" + xDiff.ToString () + ", y " + yDiff.ToString () + ", z " + zDiff.ToString () + ", Rot " + angleDiff.ToString());
					Debug.Log (piece.transform.position.ToString() + ", " + holderTf.position.ToString());
					Debug.Log (piece.transform.eulerAngles.ToString() + ", " + holderTf.eulerAngles.ToString());
				}
				
			}
		}
	}

	private void Snap (GameObject piece, Transform holderTf) {
		//create a fixed joint between that piece and the big holder
		FixedJoint joint = piece.AddComponent<FixedJoint> ();
		joint.connectedBody = holders.GetComponent<Rigidbody> ();

		var em = holderTf.gameObject.GetComponent<ParticleSystem> ().emission;
		em.enabled = true;
		snappedList.Add (piece.name);
		correctPiecesCnt += 1;

		if (correctPiecesCnt == notKinematicMax) {
			MakeSnappedKinematic ();
		} else if (correctPiecesCnt > notKinematicMax)
			piece.GetComponent<Rigidbody> ().isKinematic = true;
	}

	private void MakeSnappedKinematic() {
		for (int i = 0; i < notKinematicMax; i++)
			pieces.transform.Find (snappedList [i]).gameObject.GetComponent<Rigidbody> ().isKinematic = true;
	}

	private void UpdateText() {
		scoreTxt.text = "Remaining: " + (piecesAmt - collectedCnt).ToString ();
	}
}
