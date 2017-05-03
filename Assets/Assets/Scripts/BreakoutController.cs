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

		g_RigidBody.centerOfMass = new Vector3 (0f, -7f, 0f); 
		g_MovementAxisName = "Vertical";
		g_TurnAxisName = "Horizontal";
		
	}
	
	// Update is called once per frame
	void Update () {

		/* We must save the points where we can find wheels in order to apply a Raycast from there to check suspensions */

		List<Vector3> WheelPositions = new List<Vector3> ();

		/* FRONT WHEELS: */  WheelPositions.Add (transform.TransformPoint(new Vector3(-52f, -17.5f, -30f))); 
							 WheelPositions.Add (transform.TransformPoint(new Vector3(-52f, -17.5f, 30f)));

		/* BACK WHEELS: */   WheelPositions.Add (transform.TransformPoint(new Vector3(37f, -17.5f, -33f)));
							 WheelPositions.Add (transform.TransformPoint(new Vector3(37f, -17.5f, 33f)));

		/* For each wheel we calculate if it is grounded and if it is we apply the normal force on it. */
		fullyGrounded = true;

		foreach (Vector3 Wheel in WheelPositions) {

			RaycastHit grounded;
			Physics.Raycast (Wheel, -transform.up, out grounded);

			if (grounded.distance < g_carHeight) 
				g_RigidBody.AddForceAtPosition (((g_carHeight - grounded.distance)/g_carHeight) * g_GroundedForce * grounded.normal, Wheel);
			else
				fullyGrounded = false;
					
			/* Suspension applied to wheels */
			SuspensionWheel (Wheel, grounded.distance);

		}
	
		/* Traction */
		g_RigidBody.AddForce(-5f * Vector3.Dot(g_RigidBody.velocity, transform.right) * transform.right);

		/* After applying it we must treat the inputs */
		g_MovementInputValue = Input.GetAxis (g_MovementAxisName);
		g_TurnInputValue = Input.GetAxis (g_TurnAxisName);

		
	}

	private void FixedUpdate() {
		Move ();
		Turn ();
		RotateWheels();
	}

	private void Move() {

		if (fullyGrounded)
		g_RigidBody.AddForceAtPosition(g_MotorForce * g_MovementInputValue * transform.forward, transform.position - 9f * transform.up);

	}


	private void Turn() {

		g_RigidBody.AddTorque(g_TurnForce * g_TurnInputValue * transform.up);

	}

	private void RotateWheels () {

		if (g_RigidBody.velocity.z != 0) {

			List<string> Wheels = new List<string> ();
			Wheels.Add ("AlchemistFrontLeft");
			Wheels.Add ("AlchemistFrontRight");
			Wheels.Add ("AlchemistRearLeft");
			Wheels.Add ("AlchemistRearRight");

			float radiansTravelled = g_RigidBody.velocity.z * Mathf.Rad2Deg * Time.deltaTime / 150 ;

			foreach (string WheelName in Wheels) {

				GameObject Wheel = GameObject.Find (WheelName);
				Wheel.transform.Rotate(new Vector3(0f, 1f, 0f), radiansTravelled, Space.Self);
			}
		}
	}

	private void SuspensionWheel (Vector3 position, float distance) {
		
		GameObject Wheels = GameObject.Find ("Wheels");
		Wheels.transform.position = transform.position;
		float compression;

		RotateWheels();

		if (distance > g_carHeight)
			compression = 1;
		else
			compression = distance / g_carHeight;

		Debug.Log (compression);

		Wheels.transform.Translate(-transform.up * compression * 3); 


	}

}
