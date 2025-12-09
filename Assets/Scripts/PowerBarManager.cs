using UnityEngine;
using UnityEngine.UI;

public class PowerBarManager : MonoBehaviour
{
    public static PowerBarManager Instance;

    [Header("UI")]
    public Image powerFill;  // thanh full sau 10s

    [Header("Config")]
    public float fillTime = 10f;
    private float timer = 0f;

    public bool isReady = false;
    public bool isActivated = false; // đã bật để buff cho đòn kế tiếp chưa

    public int maxPowerBounce = 5;
    public int currentBounce = 0;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isReady || isActivated) return;
        if (GameManager.Instance.isGameStarted) 
        {
            timer += Time.deltaTime;
            powerFill.fillAmount = timer / fillTime;

            if (timer >= fillTime)
            {
                isReady = true;
            }
        }
        //timer += Time.deltaTime;
        //powerFill.fillAmount = timer / fillTime;

        //if (timer >= fillTime)
        //{
        //    isReady = true;
        //}
    }

    public void OnClickPower()
    {
        if (!isReady) return;

        PlayerController.Instance.powerPlayerEffect.SetActive(true);
        Debug.Log("AnhNTT click power");
        isActivated = true;
        isReady = false;

        timer = 0;
        powerFill.fillAmount = 0;
        //powerFill.color = Color.white;
        Invoke("DisableMultiShot", 5f);
    }

    void DisableMultiShot()
    {
        Debug.Log("AnhNTT END power");
        if (currentBounce == 0) 
        {
            isActivated = false;
            currentBounce = 0;
            PlayerController.Instance.powerPlayerEffect.SetActive(false);
        }
    }

    public void Consume()
    {
        isActivated = false;
        currentBounce = 0;
    }
}
