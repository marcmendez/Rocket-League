using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.tag == "Cars") {
			
			int magnitude = 5000;
			var force = transform.position - collision.transform.position;
			force.Normalize ();
			gameObject.GetComponent<Rigidbody> ().AddForce (force * magnitude);
		}
	}
}
