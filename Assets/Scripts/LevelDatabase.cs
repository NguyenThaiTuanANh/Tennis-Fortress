using UnityEngine;

public class LevelDatabase : MonoBehaviour
{
    public static LevelDatabase Instance;

    public LevelConfig[] levels;
    public int currentLevel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }
    private void Start()
    {
        currentLevel = PlayerData.Instance.Level;
    }

    public LevelConfig GetCurrentLevelConfig()
    {
        Debug.Log("AnhNTT level " + currentLevel);
        return levels[currentLevel - 1];
    }
}