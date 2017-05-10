using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTwoController : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Ball")
        {
            Debug.Log("Goal team 2");
            GameController.instance.team2Scored();
        }


    }
}
