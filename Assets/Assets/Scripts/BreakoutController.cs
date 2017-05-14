using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreakoutController : MonoBehaviour {

	public int g_PlayerTeam = 1;

	/* FORCES */
	public float g_GroundedForce = 10.0f;
	public float g_TurnForce = 6000f;
	public float g_TurnDragForce = 20f;
	public float g_MotorForce = 160f;
	public float g_JumpForce = 2500f;
	public float g_AirTurnRotate = 1500f;
	public float g_AirTurnFlip = 750f;
	public float g_carHeight = 15f;

	/* AXIS NAMES */
	private string g_MovementAxisName;
	private string g_TurnAxisName;

	/* INPUT VALUES */
	private float g_MovementInputValue;
	private float g_TurnInputValue;

	/* OTHERS */
	private GameObject SkidLeft;
	private GameObject SkidRight;
	private Rigidbody g_RigidBody;
	private bool fullyGrounded;
	private int wheelsGrounded;
	private bool wheelsWall;
	private bool shiftedLeft;
	private bool shiftedRight;

	// It is used when generated
	private void Awake () {
	
		g_RigidBody = GetComponent<Rigidbody> ();
	
	}

	// It is used just after Awake
	private void OnEnable () {

		g_MovementInputValue = 0f;
		g_TurnInputValue = 0f;
		shiftedLeft = false;
		shiftedRight = false;
		SkidLeft = GameObject.Find ("SkidMarkLeft");
		SkidRight = GameObject.Find ("SkidMarkRight");

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
		wheelsGrounded = 0;
		wheelsWall = false;
		foreach (Vector3 Wheel in WheelPositions) {

			RaycastHit grounded;
			Physics.Raycast (Wheel, -transform.up, out grounded);

			if (grounded.distance < g_carHeight) {

				g_RigidBody.AddForceAtPosition (((g_carHeight - grounded.distance) / g_carHeight) * g_GroundedForce * grounded.normal, Wheel);
				if (grounded.normal.y > -0.9f && grounded.normal.y < 0.9f) {
					Debug.Log ("Wall force normal:" + grounded.normal);
					wheelsWall = true;
				}
				++wheelsGrounded;
			} else {
				fullyGrounded = false;
			}

			/* Suspension applied to wheels */
			SuspensionWheel (Wheel, grounded.distance);

			/* Auxiliars for debugging treating
			if (grounded.distance != 0 && grounded.distance < g_carHeight) Debug.Log("Wheel Force (" + wheelCount + ") -> " + ((g_carHeight - grounded.distance) / g_carHeight) * g_GroundedForce * grounded.normal);*/
			/* Auxiliars for debugging treating */
			if (grounded.distance != 0 && grounded.distance < g_carHeight) //Debug.Log("Wheel Force (" + wheelCount + ") -> " + ((g_carHeight - grounded.distance) / g_carHeight) * g_GroundedForce * grounded.normal);

			Debug.DrawRay(Wheel, -transform.up * g_carHeight, (grounded.distance < g_carHeight)?Color.red:Color.black); /* Red lines */
			++wheelCount;
		}

		foreach (Vector3 Wheel in WheelPositions) {

			if (wheelsWall) {
				RaycastHit grounded;
				Physics.Raycast (Wheel, -transform.up, out grounded);
				float aux_relation;
				if (grounded.distance < g_carHeight)
					aux_relation = ((g_carHeight - grounded.distance) / g_carHeight);
				else
					aux_relation = 0.5f;

				if (aux_relation > 0.5f)
					aux_relation = 0.5f;
				
				g_RigidBody.AddForceAtPosition (aux_relation * g_GroundedForce * -grounded.normal, Wheel);
			}

		}

		/*if (!fullyGrounded) Debug.Log ("Falling Force: " + (g_RigidBody.velocity.y * g_RigidBody.velocity.y * g_RigidBody.mass));*/
		if (!fullyGrounded) //Debug.Log ("Falling Force: " + (g_RigidBody.velocity.y * g_RigidBody.velocity.y * g_RigidBody.mass));

	
		/* Traction */
		g_RigidBody.AddForce(-1f * Vector3.Dot(g_RigidBody.velocity, transform.right) * transform.right);

		/* After applying it we must treat the inputs */
		g_MovementInputValue = Input.GetAxis (g_MovementAxisName);
		g_TurnInputValue = Input.GetAxis (g_TurnAxisName);

		Jump ();
		Move ();
		Turn ();

		
	}

	private void FixedUpdate() {
		checkShifted ();
		Jump ();
		Move ();
		Turn ();
	}

	private void checkShifted() {

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			shiftedLeft = true;
		} 
		else if (Input.GetKeyUp(KeyCode.LeftShift)) {
			shiftedLeft = false;
		}

		if (Input.GetKeyDown(KeyCode.RightShift)) {
			shiftedRight = true;
		} 
		else if (Input.GetKeyUp(KeyCode.RightShift)) {
			shiftedRight = false;
		}
			
	}
	
	private void Jump() {

		if (Input.GetKeyDown (KeyCode.Space) && fullyGrounded) 
			g_RigidBody.AddForceAtPosition(g_JumpForce * transform.up, transform.position - 8.8f * transform.up);
		
	} 

	private void Move() {

		if (fullyGrounded)
			g_RigidBody.AddForceAtPosition (g_MotorForce * g_MovementInputValue * transform.forward, transform.position - 8.8f * transform.up);
		else if (g_MovementInputValue != 0 && wheelsGrounded == 0) {
			g_RigidBody.AddTorque (g_AirTurnFlip * g_MovementInputValue * transform.right);
		}

		/* Adding gravity force without altering others */
		if (wheelsGrounded < 4 && !wheelsWall) {
			g_RigidBody.AddForce(175 * new Vector3 (0, -1, 0));
		}
	}


	private void Turn() {
		
		if (fullyGrounded) {
			if (shiftedLeft || shiftedRight) {
				g_RigidBody.AddForceAtPosition (g_TurnDragForce * g_TurnInputValue * -transform.right, transform.position - 8.8f * transform.forward);
				if (g_TurnInputValue != 0) { SkidLeft.SetActive (true); SkidRight.SetActive (true); }
			} else {
				g_RigidBody.AddForceAtPosition (g_TurnForce * g_TurnInputValue * -transform.right, transform.position - 8.8f * transform.forward);
				SkidLeft.SetActive (false); SkidRight.SetActive (false);
			}
		} else if (g_TurnInputValue != 0 && wheelsGrounded == 0) {
			g_RigidBody.AddTorque (g_AirTurnRotate * g_TurnInputValue * transform.forward);
		}

		if (g_TurnInputValue == 0 || wheelsGrounded == 0) {
			SkidLeft.SetActive (false); SkidRight.SetActive (false);
		}

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

				/*Debug.Log ("Rotate Wheels Radians: " + radiansTravelled + "rad");*/

				//Debug.Log ("Rotate Wheels Radians: " + radiansTravelled + "rad");

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


		/*Debug.Log ("Suspension compression: " + compression);*/

		//Debug.Log ("Suspension compression: " + compression);


		WheelsRear.transform.Translate (new Vector3(0,-1f,0) * compression, Space.Self); 
		WheelsFront.transform.Translate (new Vector3(0,-1f,0) * compression, Space.Self);

	}

}
