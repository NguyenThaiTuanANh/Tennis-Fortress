using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{

    public Button[] levelButtons;
    public GameObject[] UiLock;
    public LoadingScreenController loadingScreen;
    public TMP_Text CoinMainText;
    public TMP_Text CoinText;
    public TMP_Text CoinTextShop;

    private void Awake()
    {
    }
    void Start()
    {
        UpdateCointInGame();
        int levelReached = PlayerPrefs.GetInt("LevelUnlocked", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
                UiLock[i].SetActive(true);
            }
            else UiLock[i].SetActive(false);

        }
    }

    public void Select()
    {
        loadingScreen.LoadScene("GamePlay");
    }


    public void SelectLevel(int level)
    {
        PlayerData.Instance.Level = level;
        Select();
    }

    public void UpdateCointInGame() 
    {
        CoinMainText.text = PlayerPrefs.GetInt("Coin", 0).ToString();
        CoinText.text = PlayerPrefs.GetInt("Coin", 0).ToString();
        CoinTextShop.text = PlayerPrefs.GetInt("Coin", 0).ToString();
    }
}
