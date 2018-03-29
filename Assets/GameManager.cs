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
	public Text instructionTxt;
	public Text titleText;
	public GameObject panel;
	public float posThreshold;
	public float rotThreshold;
	private float speed;
	private bool movement;
	private int piecesAmt;
	private int collectedCnt;
	private int correctPiecesCnt;
	private Vector3 defaultPos;
	private List<string> snappedList;
	private string[] instructionTitles;
	private string[] instructions;
	private int notKinematicMax;
	private int instrFirst;
	private int instrCnt;
	public bool collectMode;
	private bool solveJigsawMode;

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
		instructionTitles = new string[]{"Gameplay", "Locomotion", "Speed", "Collect Interaction", "Collect All", "Congratulations!", "Locomotion", "Grabbing Piece", "Changing Rotation and Position", "Correct Placement", "Snapping Mechanics", "Jigsaw Manipulation"};
		instructions = new string[]{
			"Explore the labyrinth and collect jigsaw pieces.\nAll pieces must be collected before starting to solve the jigsaw.\nA counter with the number of remaining pieces will be shown in the upper left corner.",
			"Press and hold left trigger button to move in the direction that your head is facing. Release the button to look around without moving.",
			"Press left grip button to increase the movement speed and right grip button to decrease it.",
			"Touch a jigsaw piece with a right controller and press the right trigger button to collect it to your inventory.",
			"If you want to skip the labyrinth part and just solve the jigsaw, press a touch pad to collect all pieces immediately. \nHave fun!",
			"All pieces collected. They are now released around you.",
			"Locomotion is now disabled and you can move around in a natural way.",
			"To grab a jigsaw piece, touch it with a controller and press the trigger button. Release the trigger button to release the object. Both controllers can be used.",
			"Once a piece is grabbed, its position and rotation changes in accordance to the controller holding it.\nTip: Rotating can be easier using two hands in turns!",
			"A piece is in a correct position when it makes a quick movement (\"snap\") and has white particles around it after being released.",
			"The first piece that is grabbed becomes a reference. Other pieces are then snapped if released within small positional and rotational thresholds from their correct placements, relative to the first piece.",
			"At the beginning, moving a snapped piece moves all other snapped pieces. This allows putting the jigsaw in a comfortable position. After 10 pieces are snapped, manipulation is disabled. \nGood luck!"
								};
		instrFirst = 6;
		instrCnt = 0;
		collectMode = false;
		solveJigsawMode = false;
		UpdateInstructions ();
		//UpdateText ();
	}
	
	// Update is called once per frame
	void Update() {
		if (collectMode) {
			if (movement)
				cameraRig.transform.position += new Vector3 (player.transform.forward.x * Time.deltaTime * speed, 0, player.transform.forward.z * Time.deltaTime * speed);
		}
		else if (solveJigsawMode) {
			if (correctPiecesCnt == piecesAmt) {
				endTxt.gameObject.SetActive(true);
				solveJigsawMode = false;
			}
		}
	}

	#region input event handlers
	public void OnTouchPadPressed() {
		if (collectMode)
			OnAllCollected(); //Collect all items for testing
	}

	public void OnTriggerBtnPressed(string controller, GameObject collidingObject = null) {

		if (!collectMode && !solveJigsawMode) {
			
			UpdateInstructions ();

			if (instrCnt == instrFirst) {
				collectMode = true;
				panel.SetActive (false);
				scoreTxt.gameObject.SetActive(true);
				UpdateText ();
			}

			else if (instrCnt == instructions.Length) {
				solveJigsawMode = true;
				panel.SetActive (false);
				SetupSolveJigsawMode ();

			}

		} else if (collectMode && !solveJigsawMode) {
			if (controller.Contains ("left"))
				Move (true);
			else if (controller.Contains ("right") && collidingObject)
				PutInInventory (collidingObject);
		}
	}

	public void OnTriggerBtnReleased(string controller) {
		if (collectMode) {
			if (controller.Contains ("left"))
				Move (false);
		}
	}

	public void OnGripPressed(string controller) {
		if (collectMode) {
			if (controller.Contains ("left"))
				ChangeSpeed (0.5f);
			else if (controller.Contains ("right"))
				ChangeSpeed (-0.5f);
		}
	}

	#endregion

	#region Collect mode
	private void Move(bool m) {
		movement = m; // constant movement on trigger pressed
		//movement = ! movement; //constant movement on/off
	}
		
	private void PutInInventory(GameObject piece)
	{
		if (piece.transform.IsChildOf (pieces.transform)) {
			piece.SetActive (false);
			collectedCnt += 1;
			UpdateText ();
			if (collectedCnt == piecesAmt) {
				SetupSolveJigsawMode ();
			}
		}
	}

	private void ChangeSpeed(float modifier) {
		speed += modifier;
		speed = Mathf.Max (speed, 1.0f); //speed should not be too low
	}

	public void OnAllCollected() {
		collectMode = false;
		collectedCnt = piecesAmt;
		scoreTxt.gameObject.SetActive(false);
		panel.SetActive (true);
	}

	private void SetupSolveJigsawMode()
	{
		Debug.Log ("Setting up Jigsaw");
		scoreTxt.gameObject.SetActive(false);
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
		leftController.AddComponent<ControllerGrabObject> ();
		rightController.AddComponent<ControllerGrabObject> ();

	}

	#endregion

	#region Solve jigsaw mode
	public void ChangeKinematic(GameObject piece) {
		Debug.Log ("Change Kinematic");
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
	#endregion


	#region update UI
	private void UpdateInstructions()
	{
		instructionTxt.text = instructions [instrCnt];
		titleText.text = instructionTitles [instrCnt];
		instrCnt += 1;
	}

	private void UpdateText() {
		scoreTxt.text = "Remaining: " + (piecesAmt - collectedCnt).ToString ();
	}
	#endregion
}
