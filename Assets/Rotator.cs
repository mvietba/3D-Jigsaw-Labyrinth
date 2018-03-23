using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
	private int speed;
	// Use this for initialization
	void Start () {
		speed = Random.Range (40, 60);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, 0, Time.deltaTime * speed));
	}
}
