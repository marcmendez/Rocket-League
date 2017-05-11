using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float camPosOffsetXZ = 0.0f;
	public float camPosOffsetY = 0.0f;
	public float camPosYOffset = 3.71f;
	public float smoothTime = 5.0f;
	public Transform ball;
	public Transform car;

	// Use this for initialization
	void Start () {



	}
	// Update is called once per frame
	void Update() {
		
		Vector3 targetPos = new Vector3(car.position.x, car.position.y + camPosYOffset, car.position.z);

		float distanceBallCarX = Mathf.Abs(car.position.x - ball.position.x);
		float distanceBallCarY = Mathf.Abs(car.position.y - ball.position.y);
		float distanceBallCarZ = Mathf.Abs(car.position.z - ball.position.z);

		float distanceRelationXZ = distanceBallCarZ / distanceBallCarX;
		float distanceRelationXY = distanceBallCarY / distanceBallCarX;

		float OX = Mathf.Sqrt( -Mathf.Pow(camPosOffsetXZ,2) / (-1 - Mathf.Pow(distanceRelationXZ,2)));
		float OY = OX * distanceRelationXY;
		float OZ = OX * distanceRelationXZ;


		/*Debug.Log ("OX: " + OX + " / OY: " + OY + "/ OZ: " + OZ);*/

		//Debug.Log ("OX: " + OX + " / OY: " + OY + "/ OZ: " + OZ);

		if (ball.position.x < car.position.x)
			targetPos.x = targetPos.x + OX;
		else
			targetPos.x = targetPos.x - OX;

		if (ball.position.z < car.position.z)
			targetPos.z = targetPos.z + OZ;
		else
			targetPos.z = targetPos.z - OZ;
		
		if (ball.position.y < car.position.y)
			targetPos.y = targetPos.y + OY;
		else {
			targetPos.y = targetPos.y - OY;
			if (targetPos.y <= 0)
				targetPos.y = car.position.y;
			
		}

		transform.position = Vector3.Lerp(ball.position, targetPos, smoothTime);
		transform.LookAt(ball);
			
	}
}
