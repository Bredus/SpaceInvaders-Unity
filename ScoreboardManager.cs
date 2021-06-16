using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    public GameObject ScoreboardDisplay;

    public Text namesText;
    public Text scoresText;

    string[] names = new string[10];
    int[] scores = new int[10];

    string path = "Assets/Resources/Scoreboard.txt";

    public int Highscore {
        get
        {
            LoadScoreboardFile();
            SortScoreboard();
            return scores[0];
        }
    }

    public void LoadScoreboardFile()
    {
        string line = "";

        if (File.Exists(path))
        {
            StreamReader file = new StreamReader(path);
            for (int i = 0; i < 10; i++)
            {
                if ((line = file.ReadLine()) == null)
                    break;

                names[i] = line;

                if ((line = file.ReadLine()) == null)
                    break;

                scores[i] = int.Parse(line);
                if (!int.TryParse(line, out scores[i]))
                {
                    SetDefaultData();
                    break;
                }
            }
            file.Close();
            Debug.Log("SCOREBOARD LOADED");
        }
        else
        {
            Debug.Log("HIGHSCORE TEXT FILE NOT FOUND.");
            SetDefaultData();
        }
    }

    public void SetDefaultData()
    {
        for (int i = 0; i < 10; i++)
        {
            names[i] = "";
            scores[i] = 0;
        }
        Debug.Log("SCOREBOARD SET TO DEFAULT VALUES");
    }

    public void DisplayScoreboard()
    {
        string namesDisplay = "";
        string scoresDisplay = "";

        for (int i = 0; i < names.Length; i++)
        {
            namesDisplay += names[i] + "\n";
            if (names[i] == "") //No Entry
                scoresDisplay += "" + "\n";
            else
                scoresDisplay += scores[i].ToString() + "\n";
        }

        namesText.text = namesDisplay;
        scoresText.text = scoresDisplay;
        ScoreboardDisplay.SetActive(true);
    }

    public void SortScoreboard()
    {
        bool ordered = false;
        string tempName = "";
        int tempScore = 0;

        while (!ordered)
        {
            ordered = true;
            for (int i = 0; i < names.Length - 1; i++)
            {
                if (scores[i] < scores[i + 1])
                {
                    tempScore = scores[i];
                    tempName = names[i];

                    scores[i] = scores[i + 1];
                    names[i] = names[i + 1];

                    scores[i+1] = tempScore;
                    names[i+1] = tempName;

                    ordered = false;
                }
            }
        }
        Debug.Log("SCOREBOARD SORTED");
    }

    public void UpdateScoreboard(string name, int score)
    {
        bool isTopTen = false;
        int rank = 11; // Max entries on scoreboard is 10 - so is out of scope by default

        SortScoreboard();

        for (int i = 0; i < scores.Length && !isTopTen; i++)
        {
            if (score > scores[i])
            {
                isTopTen = true;
                rank = i;
            }
        }

        if (isTopTen)
        {
            for (int i = scores.Length -1; i > rank && i > 0; i--)
            {
                names[i] = names[i - 1];
                scores[i] = scores[i - 1];
            }
            names[rank] = name;
            scores[rank] = score;
            SaveScoreboardFile();
        }
        Debug.Log("SCOREBOARD UPDATED");
    }

    public void SaveScoreboardFile()
    {
        SortScoreboard();
        
        if (File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path);

            for (int i = 0; i < names.Length; i++)
            {
                writer.WriteLine(names[i]);
                writer.WriteLine(scores[i].ToString());
            }
            writer.Close();
            Debug.Log("SCOREBOARD SAVED");
        }
    }
}
