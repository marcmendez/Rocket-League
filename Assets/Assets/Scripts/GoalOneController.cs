using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalOneController : MonoBehaviour {

    void OnTriggerEnter (Collider collider)
    {
       
        if (collider.gameObject.tag == "Ball")
        {
            Debug.Log("Goal team 1");
            GameController.instance.team1Scored();
        }
           

    }
}
