using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Item 0 - Upgrade Ball Damage")]
    public TMP_Text item0_LevelText;
    public TMP_Text item0_CostText;
    public Button btnUpgradeBall;
    public GameObject Locked;

    [Header("Item 1 - Upgrade Player Health")]
    public TMP_Text item1_LevelText;
    public TMP_Text item1_CostText;
    public Button btnUpgradeHP;
    public GameObject LockedH;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        int coin = PlayerData.Instance.Coin;

        int ballLv = PlayerPrefs.GetInt("BallDamageLevel", 1);
        int ballCost = PlayerPrefs.GetInt("BallUpgradePrice", 2000);

        item0_LevelText.text = "Lv" + (ballLv + 1);
        item0_CostText.text = ballCost.ToString();

        bool temp = (coin >= ballCost);
        btnUpgradeBall.interactable = temp;
        Locked.SetActive(!temp);


        int hpLv = PlayerPrefs.GetInt("PlayerHealthLevel", 1);
        int hpCost = PlayerPrefs.GetInt("HPUpgradePrice", 2000);

        item1_LevelText.text = "Lv" + (hpLv);
        item1_CostText.text = hpCost.ToString();

        bool tempHP = (coin >= hpCost);
        btnUpgradeHP.interactable = tempHP;
        LockedH.SetActive(!tempHP);
    }

    public void OnClick_UpgradeBall()
    {
        bool success = PlayerData.Instance.UpgradeBallDamage();

        if (success)
        {
            PlayerData.Instance.SaveUpgradeData();
            RefreshShopUI();
        }
    }


    public void OnClick_UpgradeHP()
    {
        bool success = PlayerData.Instance.UpgradePlayerHP();

        if (success)
        {
            PlayerData.Instance.SaveUpgradeData();
            RefreshShopUI();
        }
    }
}
