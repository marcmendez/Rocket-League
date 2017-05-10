using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    public static GameController instance;
    public Text ScoreTeam1Text;
    public Text ScoreTeam2Text;
    public Text Timer;

    private int scoreTeam1 = 0;
    private int scoreTeam2 = 0;
    public bool goalScored = false;
    private float timeLeft;
    // Use this for initialization
    void Awake()
    {
        timeLeft = 300;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
            timeLeft -= Time.deltaTime;
        if (timeLeft > 0)
        {
            string minutes = ((int)timeLeft / 60).ToString();
            string seconds = (timeLeft % 60).ToString("f0");
            Timer.text = minutes + ":" + seconds;
            //finish game
        }
        else {
            //gameover
        }
       
        if (goalScored == true) { } //Aqui algo per tornar a fer spawn
    }

    public void team1Scored()
    {
        Debug.Log("holis");
        scoreTeam1++;
        ScoreTeam1Text.text = scoreTeam1.ToString();
        goalScored = true;
    }

    public void team2Scored()
    {
        scoreTeam2++;
        ScoreTeam2Text.text = scoreTeam2.ToString();
        goalScored = true;
    }
}
