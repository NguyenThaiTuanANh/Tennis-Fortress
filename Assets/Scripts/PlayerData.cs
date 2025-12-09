using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [Header("Player Data")]
    public int Level;
    public int Coin;

    [Header("Base Stats")]
    public int ballDamageBase = 40;
    public int playerMaxHPBase = 100;

    [HideInInspector]
    public int ballDamageLevel;
    public int playerHealthLevel;

    [HideInInspector]
    public int ballUpgradePrice;
    public int hpUpgradePrice;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
        LoadUpgradeData();
        ApplyFinalValuesToPrefs();
    }

    void Start()
    {
        
    }


    public void SaveData()
    {
        Debug.Log("Save Level: " + Level);
        PlayerPrefs.SetInt("PlayerLevel", Level);
        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        Level = PlayerPrefs.GetInt("PlayerLevel", 1);
        Coin = PlayerPrefs.GetInt("Coin", 0);
    }


    public void SaveUpgradeData()
    {
        PlayerPrefs.SetInt("BallDamageLevel", ballDamageLevel);
        PlayerPrefs.SetInt("PlayerHealthLevel", playerHealthLevel);

        PlayerPrefs.SetInt("BallUpgradePrice", ballUpgradePrice);
        PlayerPrefs.SetInt("HPUpgradePrice", hpUpgradePrice);

        PlayerPrefs.Save();

        ApplyFinalValuesToPrefs();
    }

    public void LoadUpgradeData()
    {
        ballDamageLevel = PlayerPrefs.GetInt("BallDamageLevel", 1);
        playerHealthLevel = PlayerPrefs.GetInt("PlayerHealthLevel", 1);

        ballUpgradePrice = PlayerPrefs.GetInt("BallUpgradePrice", 2000);
        hpUpgradePrice = PlayerPrefs.GetInt("HPUpgradePrice", 2000);
    }


    public void ApplyFinalValuesToPrefs()
    {
        int finalBallDamage = GetBallDamage();
        int finalMaxHP = GetPlayerMaxHP();

        PlayerPrefs.SetInt("BallDamage", finalBallDamage);
        PlayerPrefs.SetInt("PlayerMaxHP", finalMaxHP);

        PlayerPrefs.Save();
    }


    public bool UpgradeBallDamage()
    {
        if (Coin < ballUpgradePrice)
            return false;

        Coin -= ballUpgradePrice;
        ballDamageLevel++;

        ballUpgradePrice = Mathf.RoundToInt(ballUpgradePrice * 1.5f);

        SaveData();
        SaveUpgradeData();
        return true;
    }

    public int GetBallDamage()
    {
        return ballDamageBase + ((ballDamageLevel - 1) * 10);
    }


    public bool UpgradePlayerHP()
    {
        if (Coin < hpUpgradePrice)
            return false;

        Coin -= hpUpgradePrice;
        playerHealthLevel++;

        hpUpgradePrice = Mathf.RoundToInt(hpUpgradePrice * 1.5f);

        SaveData();
        SaveUpgradeData();
        return true;
    }

    public int GetPlayerMaxHP()
    {
        return playerMaxHPBase + ((playerHealthLevel - 1) * 15);
    }
}
