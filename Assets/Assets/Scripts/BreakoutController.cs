using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreakoutController : MonoBehaviour {

	public int g_PlayerTeam = 1;

	/* FORCES */
	public float g_GroundedForce = 10.0f;
	public float g_TurnForce = 6000f;
	public float g_MotorForce = 160f;

	/* AXIS NAMES */
	private string g_MovementAxisName;
	private string g_TurnAxisName;

	/* INPUT VALUES */
	private float g_MovementInputValue;
	private float g_TurnInputValue;

	/* OTHERS */
	private Rigidbody g_RigidBody;
	private float g_carHeight;
	private bool fullyGrounded;

	// It is used when generated
	private void Awake () {
	
		g_RigidBody = GetComponent<Rigidbody> ();
	
	}

	// It is used just after Awake
	private void OnEnable () {

		g_carHeight = 7f;
		g_MovementInputValue = 0f;
		g_TurnInputValue = 0f;

	}

	// Use this for initialization
	void Start () {

		g_MovementAxisName = "Vertical";
		g_TurnAxisName = "Horizontal";
		
	}
	
	// Update is called once per frame
	void Update () {

		/* We must save the points where we can find wheels in order to apply a Raycast from there to check suspensions */

		List<Vector3> WheelPositions = new List<Vector3> ();

		/* FRONT WHEELS: */  WheelPositions.Add (transform.TransformPoint(new Vector3(40f, -15f, 75f))); 
							 WheelPositions.Add (transform.TransformPoint(new Vector3(-40f, -15f, 75f)));

		/* BACK WHEELS: */   WheelPositions.Add (transform.TransformPoint(new Vector3(40f, -15f, -75f)));
							 WheelPositions.Add (transform.TransformPoint(new Vector3(-40f, -15f, -75f)));

		/* For each wheel we calculate if it is grounded and if it is we apply the normal force on it. */
		fullyGrounded = true;

		foreach (Vector3 Wheel in WheelPositions) {

			RaycastHit grounded;
			Physics.Raycast (Wheel, -transform.up, out grounded);
			if (grounded.distance < g_carHeight)
				GetComponent<Rigidbody> ().AddForceAtPosition ((g_carHeight - grounded.distance) * g_GroundedForce * grounded.normal, Wheel);
			else
				fullyGrounded = false;

		}

		/* Traction */
		g_RigidBody.AddForce(-0.1f * Vector3.Dot(GetComponent<Rigidbody>().velocity, transform.right) * transform.right);

		/* After applying it we must treat the inputs */
		g_MovementInputValue = Input.GetAxis (g_MovementAxisName);
		g_TurnInputValue = Input.GetAxis (g_TurnAxisName);

		
	}

	private void FixedUpdate() {
		Move ();
		Turn ();
	}

	private void Move() {

		if (fullyGrounded)
		g_RigidBody.AddForceAtPosition(g_MotorForce * g_MovementInputValue * transform.forward, transform.position - 0.5f * transform.up);

	}


	private void Turn() {

		g_RigidBody.AddTorque(g_TurnForce * Input.GetAxis("Horizontal") * transform.up);

	}

}
