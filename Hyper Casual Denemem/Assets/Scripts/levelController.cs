using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText,gameOverMenuMOneyText,finishMenuMoneyText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;
    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;
    public DailyReward dailyReward;
    int currentLevel;
    int score;
    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().name != "level" + currentLevel)        
        {
            SceneManager.LoadScene("level" + currentLevel);
        }
        else
        {
            PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();
            GameObject.FindObjectOfType<MarketController>().InitializeMarketController();   
            dailyReward.InitializeDailyReward();
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
            UpdateMoneyText();
            GiveMoneyToPLayer(3000);
        }
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (gameActive)
        {
            PlayerController Player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance/maxDistance);
        }
        
    }
    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running",true);
        gameActive = true;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }
    public void GameOver()
    {
        UpdateMoneyText();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }
    public void FinishGame()
    {
        GiveMoneyToPLayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }
    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }
    public void UpdateMoneyText()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMOneyText.text = money.ToString();
        finishMenuMoneyText.text = money.ToString();
    }
    public void GiveMoneyToPLayer(int increment)
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyText();
    }
}
