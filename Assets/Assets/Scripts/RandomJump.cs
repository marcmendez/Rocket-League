/* COPIED -> CHECK */

using UnityEngine;
using System.Collections;

public class RandomJump : MonoBehaviour {
	
	public float forceMagnitude = 1500000000.0f;
	
	System.Random rnd;

	// Use this for initialization
	void Start () {
		rnd = new System.Random();
		Example ();
	}

	void Example() {
		Physics.gravity = new Vector3(0, -100.0F, 0);
	}

	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space))
			gameObject.GetComponent<Rigidbody>().AddForceAtPosition(forceMagnitude * Vector3.up, transform.position + ((float)rnd.NextDouble() - 0.5f) * transform.right + 
															((float)rnd.NextDouble() - 0.5f) * transform.forward);
	}
}
