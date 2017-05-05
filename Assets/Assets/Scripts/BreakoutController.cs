using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreakoutController : MonoBehaviour {

	public int g_PlayerTeam = 1;

	/* FORCES */
	public float g_GroundedForce = 10.0f;
	public float g_TurnForce = 6000f;
	public float g_MotorForce = 160f;
	public float g_carHeight = 15f;

	/* AXIS NAMES */
	private string g_MovementAxisName;
	private string g_TurnAxisName;

	/* INPUT VALUES */
	private float g_MovementInputValue;
	private float g_TurnInputValue;

	/* OTHERS */
	private Rigidbody g_RigidBody;
	private bool fullyGrounded;

	// It is used when generated
	private void Awake () {
	
		g_RigidBody = GetComponent<Rigidbody> ();
	
	}

	// It is used just after Awake
	private void OnEnable () {

		g_MovementInputValue = 0f;
		g_TurnInputValue = 0f;

	}

	// Use this for initialization
	void Start () {

		g_RigidBody.centerOfMass = new Vector3 (0f, -8.8f, 0.5f); 
		g_MovementAxisName = "Vertical";
		g_TurnAxisName = "Horizontal";
		
	}
	
	// Update is called once per frame
	void Update () {

		/* We must save the points where we can find wheels in order to apply a Raycast from there to check suspensions */

		List<Vector3> WheelPositions = new List<Vector3> ();

		/* FRONT WHEELS: */  WheelPositions.Add (transform.TransformPoint(new Vector3(-30f, -7.5f, -37f))); 
							 WheelPositions.Add (transform.TransformPoint(new Vector3(30f, -7.5f, -37f)));

		/* BACK WHEELS: */   WheelPositions.Add (transform.TransformPoint(new Vector3(-33f, -7.5f, 52f)));
							 WheelPositions.Add (transform.TransformPoint(new Vector3(33f, -7.5f, 52f)));

		/* For each wheel we calculate if it is grounded and if it is we apply the normal force on it. */
		fullyGrounded = true;

		/* Auxiliar to notify in conosole, which wheel aplies the force */
		int wheelCount = 0;
		float fallingForce = g_RigidBody.velocity.y * g_RigidBody.velocity.y * g_RigidBody.mass;
	
		foreach (Vector3 Wheel in WheelPositions) {

			RaycastHit grounded;
			Physics.Raycast (Wheel, -transform.up, out grounded);

			if (grounded.distance < g_carHeight) {

				g_RigidBody.AddForceAtPosition (((g_carHeight - grounded.distance) / g_carHeight) * g_GroundedForce * grounded.normal, Wheel);
				
			} else {
				fullyGrounded = false;
			}

			/* Suspension applied to wheels */
			SuspensionWheel (Wheel, grounded.distance);

			/* Auxiliars for debugging treating */
			if (grounded.distance != 0 && grounded.distance < g_carHeight) Debug.Log("Wheel Force (" + wheelCount + ") -> " + ((g_carHeight - grounded.distance) / g_carHeight) * g_GroundedForce * grounded.normal);
			Debug.DrawRay(Wheel, -transform.up * g_carHeight, (grounded.distance < g_carHeight)?Color.red:Color.black); /* Red lines */
			++wheelCount;
		}
			
		if (!fullyGrounded) Debug.Log ("Falling Force: " + (g_RigidBody.velocity.y * g_RigidBody.velocity.y * g_RigidBody.mass));
	
		/* Traction */
		g_RigidBody.AddForce(-5f * Vector3.Dot(g_RigidBody.velocity, transform.right) * transform.right);

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

			float radiansTravelled = g_RigidBody.velocity.z * Mathf.Rad2Deg * Time.deltaTime / 10;

			foreach (string WheelName in Wheels) {
				Debug.Log ("Rotate Wheels Radians: " + radiansTravelled + "rad");
				GameObject Wheel = GameObject.Find (WheelName);
				Wheel.transform.Rotate(new Vector3(0f, 1f, 0f), radiansTravelled, Space.Self);
			}
		}
	}

	private void SuspensionWheel (Vector3 position, float distance) {
		
		GameObject WheelsFront = GameObject.Find ("WheelsFront");
		WheelsFront.transform.position = transform.position;

		GameObject WheelsRear = GameObject.Find ("WheelsRear");
		WheelsRear.transform.position = transform.position;

		float compression;

		RotateWheels();

		if (distance > g_carHeight)
			compression = 1;
		else
			compression = distance / g_carHeight;

		Debug.Log ("Suspension compression: " + compression);

		WheelsRear.transform.Translate(-transform.up * compression); 
		WheelsFront.transform.Translate (-transform.up * compression);

	}

}
