using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Countdown")]
    public GameObject countdownUI;
    public TMP_Text countdownText;

    [Header("Coin")]
    public TMP_Text CoinMainText;

    [Header("Win UI")]
    public GameObject winPanel;
    public TMP_Text winCoinText;
    public TMP_Text levelText;

    [Header("Lose UI")]
    public GameObject losePanel;

    [Header("HP UI")]
    public Image hpFill;
    public TMP_Text hpText;
    public TMP_Text hpLevelText;
    private int currentHP;
    private int maxHP;
    [Header("Current Level")]
    public TMP_Text currentLevel;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitHPUI();
        CoinMainText.text = PlayerPrefs.GetInt("Coin", 0).ToString();
        currentLevel.text = LevelDatabase.Instance.currentLevel.ToString();
        Debug.Log("AnhNTT level start " + LevelDatabase.Instance.currentLevel);
    }

    public void ShowCountdown(bool state)
    {
        countdownUI.SetActive(state);
    }

    public void UpdateCountdown(int value)
    {
        countdownText.text = value.ToString();
    }

    public void ShowWinUI()
    {
        AudioManager.Instance.PlayWinLevel();
        int coins = PlayerPrefs.GetInt("coins", 0);
        int level = PlayerData.Instance.Level;
        Debug.Log("Complete Level: " + level);
        winPanel.SetActive(true);
        winCoinText.text = "1000";
        levelText.text = level.ToString();
        PlayerData.Instance.Coin = coins + 1000;
        PlayerPrefs.SetInt("Coin", PlayerData.Instance.Coin);
    }

    public void NextLevel()
    {
        LevelDatabase.Instance.currentLevel = PlayerData.Instance.Level;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowLoseUI()
    {
        losePanel.SetActive(true);
    }

    public void Retry()
    {
        GameManager.Instance.Retry();
    }

    public void BackMenu()
    {
        GameManager.Instance.BackToMenu();
    }

    public void InitHPUI()
    {
        maxHP = PlayerPrefs.GetInt("PlayerMaxHP", 100);
        currentHP = maxHP;

        int hpLv = PlayerPrefs.GetInt("PlayerHealthLevel", 1);

        UpdateHPBar();
        hpLevelText.text = (hpLv).ToString();
    }
    public void SetPlayerHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHPBar();
    }
    public void HealPlayer(int heal)
    {
        currentHP += heal;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpFill != null)
            hpFill.fillAmount = (float)currentHP / maxHP;

        if (hpText != null)
            hpText.text = currentHP + "/" + maxHP;
    }
    public void DamagePlayer(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
    }
}
