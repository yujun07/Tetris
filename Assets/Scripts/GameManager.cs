using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameover = false;

    public Text CurrentScoreText;
    public Text BestScoreText;
    
    public GameObject GameOverPanel;
    
    private int CurrentScore = 0;
    private int bestScore = 0;


    public int Score
    {
        get
        {
            return CurrentScore;
        }
        set
        {
            CurrentScore = value;
            CurrentScoreText.text = "���� ���� : " + CurrentScore;
            if (CurrentScore > bestScore)
            {
                bestScore = CurrentScore;

                BestScoreText.text = "�ְ� ���� : " + bestScore;

                PlayerPrefs.SetInt("Best Score", bestScore);
            }
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    void Start()
    {
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        BestScoreText.text = "�ְ� ���� : " + bestScore;
        CurrentScoreText.text = "���� ���� : " + CurrentScore;
    }
    public void GameOver()
    {
        if (isGameover)
        {
            GameOverPanel.SetActive(true);
        }
    }
    private void Update()
    {
        GameOver();
    }
}
