using System;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private AsyncOperation preloadOperation;
    
    public TextMeshProUGUI scoreBoard;
    public TextMeshProUGUI highScoreBoard;
    public TextMeshProUGUI EndingText;
    
    public GameObject restartButton;
    public GameObject startButton;
    [FormerlySerializedAs("PlayerController")] public GameObject PlayerControllerObject;
    
    public int scoreThreshold = 3;
    public BallController ballController;
    //public PlayerController playerController;
    
    private int score = 0;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;

            if (score >= scoreThreshold)
            {
                scoreThreshold *= 3;
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                //SceneManager.LoadSceneAsync(currentSceneIndex + 1, LoadSceneMode.Additive);
                //SceneManager.UnloadSceneAsync(currentSceneIndex);
                preloadOperation.allowSceneActivation = true;
            }
            if (score > HighScore)
            {
                HighScore = score;
            }
            scoreBoard.text = "Score: " + score.ToString();
            highScoreBoard.text = "High Score: " + HighScore.ToString();
        }
    }
    
    private int highScore;

    public int HighScore
    {
        get
        {
            if (File.Exists(filePathHighScore))
            {
                string fileContents = File.ReadAllText(filePathHighScore);
                highScore = int.Parse(fileContents);
            }
            return highScore;
        }
        set
        {
            highScore = value;

            if (!File.Exists(filePathHighScore))
            {
                string dirLocation = Application.dataPath + DirPath;

                if (!Directory.Exists(dirLocation))
                {
                    Directory.CreateDirectory(dirLocation);
                }
            }

            File.WriteAllText(filePathHighScore, highScore.ToString());
        }
    }
    
    [SerializeField] List<KeyValuePair<string, int>> highScoreList = new List<KeyValuePair<string, int>>();
    public string playerName = "Player";
    
    string filePathHighScore;
    string filePathHighScoreList;
    const string DirPath = "/Data/";
    const string FilePath = DirPath + "highScore.txt";
    const string ListPath = DirPath + "highScoreList.txt";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            filePathHighScore = Application.dataPath + FilePath;
            Score = 0;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        preloadOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        preloadOperation.allowSceneActivation = false;
        
        
        filePathHighScoreList = Application.dataPath + ListPath;
        string fileContents = File.ReadAllText(filePathHighScoreList);
        //Debug.Log(fileContents);
        
        string[] lines = fileContents.Split(',');
        foreach (var line in lines)
        {
            string[] parts = line.Split(':');
            for (int j=0; j<parts.Length; j++)
            {
                if (j%2 == 0)
                {
                    string name = parts[j];
                    int score = int.Parse(parts[j + 1]);
                    highScoreList.Add(new KeyValuePair<string, int>(name, score));
                }
            }
        }
        
        restartButton.SetActive(false);
        if (BallController.Instance.gameStarted == false)
        {
            startButton.SetActive(true);
            PlayerControllerObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Update()
    {

    }

    // public void UpdateScoreDisplay(int playerScore)
    // {
    //     scoreBoard.text = playerScore.ToString();
    //
    // }

    public void EndGame()
    {
        restartButton.SetActive(true);
        PlayerControllerObject.GetComponent<PlayerController>().enabled = false;
    }
    
    public void RestartGame()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(BallController.Instance.gameObject);
        Destroy(PlayerController.Instance.gameObject);
        GameManager.Instance = null;
        BallController.Instance = null;
        PlayerController.Instance = null;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        startButton.SetActive(false);
        PlayerControllerObject.GetComponent<PlayerController>().enabled = true;
        ballController.StartBall();
    }
    
    public void UpdateHighScoreList()
    {
        //go through all the high scores
        for (int i = 0; i < highScoreList.Count; i++)
        {
            int currentHS = highScoreList[i].Value;

            if (score >= currentHS)
            {
                highScoreList.Insert(i, new KeyValuePair<string, int>(playerName, score));
                break;
            }
        }

        if (highScoreList.Count > 10)
        {
            highScoreList.RemoveAt(highScoreList.Count - 1);
        }
        
        string fileContents = "";
        foreach (var scoreData in highScoreList)
        {
            fileContents += scoreData.Key + ": " + scoreData.Value + ",";
        }
        fileContents = fileContents.TrimEnd(',');
        
        File.WriteAllText(filePathHighScoreList, fileContents);
    }
    
}
