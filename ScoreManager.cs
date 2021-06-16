using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    string[,] highscoreEntrys = new string[10, 10];

    public static int scoreValue = 0;
    public static int highscoreValue = 0;
    public static int waveValue = 0;

    public GameObject scoreObject;
    public GameObject highscoreObject;
    public GameObject waveObject;

    ScoreboardManager scoreboardManager;

    Text ScoreText;
    Text HighscoreText;
    Text WaveText;

    public int ScoreValue { get { return scoreValue; } set { scoreValue = value; } }
    public int HighscoreValue { get { return highscoreValue; } set { highscoreValue = value; } }
    public int WaveValue { get { return waveValue; } set { waveValue = value; } }

    // Start is called before the first frame update
    void Start()
    {
        scoreboardManager = FindObjectOfType<ScoreboardManager>();
        highscoreValue = scoreboardManager.Highscore;

        ScoreText = scoreObject.GetComponent<Text>();
        HighscoreText = highscoreObject.GetComponent<Text>();
        WaveText = waveObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = "Score: " + scoreValue;
        HighscoreText.text = "Highscore " + highscoreValue;
        WaveText.text = "Wave " + waveValue;
    }

    public void AddToScore(int points)
    {
        scoreValue += points;
        if (scoreValue > highscoreValue)
        {
            highscoreValue = scoreValue;
        }
    }
}
