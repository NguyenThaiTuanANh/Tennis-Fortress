using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Countdown")]
    public int countdownTime = 8;

    [Header("References")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public LevelManager levelManager;

    [Header("Game State")]
    public bool isGameStarted = false;
    public bool isWin = false;
    public bool isLose = false;
    public bool isPaused = false;

    public int playerHP = 100;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        StartCoroutine(StartCountdown());
        playerHP = PlayerPrefs.GetInt("PlayerMaxHP", 100);
    }

    IEnumerator StartCountdown()
    {
        UIManager.Instance.ShowCountdown(true);

        int timer = countdownTime;
        while (timer > 0)
        {
            UIManager.Instance.UpdateCountdown(timer);
            yield return new WaitForSeconds(1f);
            timer--;
        }

        UIManager.Instance.ShowCountdown(false);

        StartGameplay();
    }

    void StartGameplay()
    {
        isGameStarted = true;

        Instantiate(ballPrefab, ballSpawnPoint.position, ballPrefab.transform.rotation);

        levelManager.enabled = true;
    }

    void Update()
    {
        if (!isGameStarted || isWin || isLose) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            HandleWin();
        }

        if (playerHP <= 0)
        {
            HandleLose();
        }
    }

    public void DamagePlayer(int dmg)
    {
        UIManager.Instance.DamagePlayer(dmg);
        playerHP -= dmg;
        if (playerHP <= 0)
        {
            HandleLose();
        }
    }

    void HandleWin()
    {
        if (isWin) return;

        isWin = true;
        PlayerController.Instance.isWin = true;

        int level = LevelDatabase.Instance.currentLevel;
        int levelPlayerPass = PlayerPrefs.GetInt("LevelUnlocked", 1);
        if (levelPlayerPass >= level) {
            Debug.Log("LevelUnlocked 1:" + (level + 1));
            PlayerPrefs.SetInt("LevelUnlocked", level + 1);
        }

        int coins = PlayerPrefs.GetInt("coins", 0);
        coins = coins + 1000;
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();

        UIManager.Instance.ShowWinUI();
    }

    void HandleLose()
    {
        if (isLose) return;

        isLose = true;
        PlayerController.Instance.isLose = true;

        UIManager.Instance.ShowLoseUI();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void PauseGame()
    {
        if (isPaused || isWin || isLose) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (PlayerController.Instance)
            PlayerController.Instance.animator.enabled = false;

    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (PlayerController.Instance)
            PlayerController.Instance.animator.enabled = true;

    }


    public void ButtonRetry()
    {
        Time.timeScale = 1f;
        Retry();
    }

    public void ButtonBackMenu()
    {
        Time.timeScale = 1f;
        BackToMenu();
    }
}
