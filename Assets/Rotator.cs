using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
	public bool isPicked = false;
	private int speed;
	// Use this for initialization
	void Start () {
		speed = Random.Range (40, 60);
	}
	
	// Update is called once per frame
	void Update () {

		if (!isPicked)
			transform.Rotate (new Vector3 (0, 0, Time.deltaTime * speed));
		
	}
}
