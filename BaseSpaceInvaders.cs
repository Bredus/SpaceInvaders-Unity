using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// --------------------------------------------------------------------------------------------------------
// 
// Base class for Space Invaders game.
//
// Please derive from this class to implement your game.
// 
// --------------------------------------------------------------------------------------------------------

public class BaseSpaceInvaders : MonoSingleton<BaseSpaceInvaders>, ISpaceInvaders
{

    //GAME DATA
    ScoreManager scoreManager;
    ScoreboardManager scoreboardManager;
    EnemyHordeManager hordeManager;
    public GameObject resultsScreen;
    public GameObject resultsObject;
    public GameObject ScoreboardDisplay;

    public bool gameRunning = false;
    public bool gameOver = false;
    string playerName = "temp";

    //GAME OBJECTS
    public GameObject player;
    public GameObject shieldManager;

    //DISPLAY/TEXT OBJECTS
    public GameObject nameInput;
    public GameObject InputObject;
    public GameObject statementTextObject;
    public GameObject statementTextObject2;
    public GameObject NameDisplayObject;

    //OTHER OBJECTS
    public GameObject boundaryObject;


    public float GetBorders(GameObject thisObject)
    {
        // OBJECT.BORDER = ACTUALBORDER - OBJECT.BOXCOLLIDER.WIDTH
        return boundaryObject.GetComponent<BoxCollider2D>().size.x / 2 - thisObject.GetComponent<BoxCollider>().size.x / 2;
    }

    protected override void Awake()
	{
		base.Awake();
	}

	void Start() 
	{
        scoreManager = FindObjectOfType<ScoreManager>();
        scoreboardManager = FindObjectOfType<ScoreboardManager>();
        hordeManager = FindObjectOfType<EnemyHordeManager>();

        if (!gameRunning)
            PreGame();
        else
            StartNewGame(); //FOR TESTING
    }

    void Update() 
	{
        
        if (!gameRunning && !gameOver)
        {
            if (InputObject.activeSelf)//PRE-GAME
            {
                //USER NAME SUBMIT
                if (Input.GetKeyDown(KeyCode.Return) && nameInput.GetComponent<Text>().text != "")
                {
                    playerName = nameInput.GetComponent<Text>().text;
                    InputObject.SetActive(false);
                    NameDisplayObject.SetActive(true);
                    NameDisplayObject.GetComponent<Text>().text = "Player: " + playerName;
                    statementTextObject.SetActive(true);
                }
            }
            //PRESS ENTER - START/RESTART GAME
            else if (Input.GetKeyDown(KeyCode.Return) && (statementTextObject.activeSelf || statementTextObject2.activeSelf))
            {
                StartNewGame();
            }
        }

        //QUIT GAME
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q PRESSED");
            Application.Quit();
        }

        //FORCED GAMEOVER - FOR TESTING
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("0 PRESSED");
            StartCoroutine(GameOverStart());
        }
    }

    public void HandleHit( GameObject object1 , GameObject object2 )
	{
        if (gameRunning)
        {
            if (object1.tag == "PlayerBullet" && object2.tag == "Enemy")
            {
                Debug.Log(object1.name + " HIT " + object2.name);
                Destroy(object1);
                Destroy(object2);
                scoreManager.AddToScore(100);
                hordeManager.enemyCount--;
                Debug.Log(hordeManager.enemyCount + " ENEMY REMAINING");
                if (hordeManager.enemyCount == 0)
                {
                    scoreManager.AddToScore(1000);
                    statementTextObject.GetComponent<Text>().text = "NEW WAVE\nGET READY!";
                    StartCoroutine(NewRound(2f));
                }
            }

            if (object1.tag == "EnemyBullet" && object2.tag == "Player")
            {
                Debug.Log(object1.name + " HIT " + object2.name);
                Destroy(object1);
                StartCoroutine(GameOverStart());
            }

            if ((object1.tag == "EnemyBullet" || object1.tag == "PlayerBullet") && object2.tag == "Shield")
            {
                Debug.Log(object1.name + " HIT " + object2.name);
                Destroy(object1);
                object2.GetComponent<ShieldController>().Damaged();
            }

            if (object1.tag == "Enemy" && object2.tag == "Shield")
            {
                Debug.Log(object1.name + " HIT " + object2.name);
                Destroy(object2);
            }

            if (object1.tag == "Enemy" && object2.tag == "Player")
            {
                Debug.Log(object1.name + " HIT " + object2.name);
                StartCoroutine(GameOverStart());
            }
        }
    }
    void PreGame()
    {
        player.GetComponent<MeshRenderer>().enabled = false;
        player.SetActive(false);
        InputObject.SetActive(true);
    }

    void StartNewGame()
    {
        resultsScreen.SetActive(false);
        ScoreboardDisplay.SetActive(false);
        statementTextObject2.SetActive(false);

        for (int i = 0; i < shieldManager.transform.childCount; i++)
        {
            GameObject child = shieldManager.transform.GetChild(i).gameObject;
            if (child != null)
            {
                child.SetActive(true);
            }
        }

        scoreManager.ScoreValue = 0;
        scoreManager.WaveValue = 0;
        statementTextObject.GetComponent<Text>().text = "GET READY!";
        StartCoroutine(NewRound(2f));
        gameOver = false;
        player.GetComponent<MeshRenderer>().enabled = true;
        player.SetActive(true);
    }

    IEnumerator GameOverStart()
    {
        Debug.Log("GAME OVER");
        gameRunning = false;
        hordeManager.gameRunning = false;
        gameOver = true;
        player.SetActive(false);

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.SetActive(false);
        }
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("PlayerBullet"))
        {
            Destroy(bullet);
        }
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("EnemyBullet"))
        {
            Destroy(bullet);
        }

        statementTextObject.SetActive(true);
        statementTextObject.GetComponent<Text>().text = "GAME OVER";
        yield return new WaitForSeconds(2f);
        statementTextObject.SetActive(false);
        GameOver(scoreManager.ScoreValue);
    }

    public void GameOver( int score )
	{
        resultsScreen.SetActive(true);
        resultsObject.GetComponent<Result>().SetScore(playerName, score);
        scoreboardManager.UpdateScoreboard(playerName, score);
        StartCoroutine(DisplayScoreboard());
    }

    IEnumerator DisplayScoreboard()
    {
        yield return new WaitForSeconds(1f);
        scoreboardManager.DisplayScoreboard();
        yield return new WaitForSeconds(1f);
        statementTextObject2.GetComponent<Text>().text = "RETRY?\nPRESS ENTER\n(or Q to quit)";
        statementTextObject2.SetActive(true);
        gameOver = false;
    }

    IEnumerator NewRound(float waitTime)
    {
        statementTextObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        statementTextObject.SetActive(false);

        hordeManager.SpawnEnemies();
        scoreManager.WaveValue++;
        gameRunning = true;
        hordeManager.gameRunning = true;
    }


}
