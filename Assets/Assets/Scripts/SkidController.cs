using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidController : MonoBehaviour {

	public GameObject skidMark;

	private bool shiftedLeft;
	private bool shiftedRight;
	private float g_TurnInputValue;
	private string g_TurnAxisName;

	void Start() {
		//skidMark.gameObject.active = false;
		shiftedLeft = false;
		shiftedRight = false;
		g_TurnInputValue = 0f;
		g_TurnAxisName = "Horizontal";
	}

	void Update () {

		checkInput();

		if ((shiftedLeft || shiftedRight) && g_TurnInputValue != 0) 
			skidMark.gameObject.active = true;
		else
			skidMark.gameObject.active = true;


	}

	private void checkInput() {

		if (Input.GetKeyDown(KeyCode.LeftShift))
			shiftedLeft = true;
		else if (Input.GetKeyUp(KeyCode.LeftShift))
			shiftedLeft = false;

		if (Input.GetKeyDown(KeyCode.RightShift)) 
			shiftedRight = true;
		else if (Input.GetKeyUp(KeyCode.RightShift)) 
			shiftedRight = false;

		g_TurnInputValue = Input.GetAxis (g_TurnAxisName);

	}
}