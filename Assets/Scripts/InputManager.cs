using UnityEngine;
using System;
//using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public float horizontalAxis = 0;
	public float verticalAxis = 0;
	public bool jumpButtonDown;
	public bool jumpButton;

	void Update () {

		if (SystemInfo.deviceType != DeviceType.Handheld) {
			horizontalAxis = Input.GetAxisRaw ("Horizontal");
			verticalAxis = Input.GetAxisRaw ("Vertical");
			jumpButtonDown = Input.GetButtonDown ("Jump");
			jumpButton = Input.GetButton ("Jump");
		} else {
			/*
			if (Input.acceleration.x > 0.15f) {
				horizontalAxis = 1;
			} else if (Input.acceleration.x < -0.15f) {
				horizontalAxis = -1;
			} else {
				horizontalAxis = 0;
			}


			verticalAxis = Input.acceleration.y;

			if (Input.touchCount > 0) {
				if (Input.touches [0].phase == TouchPhase.Began) {
					jumpButtonDown = true;
				}else
					jumpButtonDown = false;
			}
			else
				jumpButtonDown = false;
				*/
		}
	}

	public void LeftPress()
	{
		horizontalAxis = -1;
	}

	public void LeftRelease(){
		horizontalAxis = 0;
	}

	public void RightPress()
	{
		horizontalAxis = 1;
	}

	public void RightRelease(){
		horizontalAxis = 0;
	}

	public void JumpPress()
	{
		jumpButtonDown = true;
	}

	public void JumpRelease()
	{
		jumpButtonDown = false;
	}
}
