﻿/*
 * Copyright (c) 2016 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;

    private GameObject laserPrefab; // The laser prefab
    private GameObject laser; // A reference to the spawned laser
    private Transform laserTransform; // The transform component of the laser for ease of use
	private GameManager manager;
	private Vector3 hitPoint;
	private GameObject pieces;
    private GameObject hitObject; // Object where the raycast hits

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    //new
    void Start()
    {
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		pieces = manager.pieces;
		laser = (GameObject)Instantiate(Resources.Load("Laser"));
        laserTransform = laser.transform;
    }

    void Update()
    {
		if (!enabled)
			return;
        // Is the touchpad held down?
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // Send out a raycast from the controller
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
				hitPoint = hit.point;
				hitObject = hit.transform.gameObject;
				if (hitObject.transform.IsChildOf (pieces.transform) && !manager.snappedList.Contains (hitObject.name))
					laser.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
				else
					laser.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
					 
	
                ShowLaser(hit);

            }
        }
        else // Touchpad not held down, hide laser
        {
            laser.SetActive(false);
        }

        // Touchpad released this frame & valid teleport position found
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
			if (hitObject != null) {
				if (hitObject.transform.IsChildOf (pieces.transform))
					manager.OnTouchPadPressed (gameObject, hitObject);
				hitObject = null;
			}
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); //Show the laser
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }
}
