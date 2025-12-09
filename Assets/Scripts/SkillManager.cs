using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public bool multiShotActive = false;
    public float multiShotCooldown = 5f;
    private float cooldownTimer;
    public Button skillBtn;

    void Awake() 
    {
        Instance = this;
    }
    private void Start()
    {
        skillBtn.onClick.AddListener(ActivateMultiShot);
    }

    public void ActivateMultiShot()
    {
        if (cooldownTimer > 0) return;

        multiShotActive = true;
        cooldownTimer = multiShotCooldown;

        Invoke("DisableMultiShot", 1f);
    }

    void DisableMultiShot()
    {
        multiShotActive = false;
    }

    void Update()
    {
        if (cooldownTimer > 0) 
        {
            cooldownTimer -= Time.deltaTime;
            skillBtn.interactable = false;
        }
        else skillBtn.interactable = true;
    }
}
