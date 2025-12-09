using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    public GameObject[] enemies;
    public int[] spawnCounts;
    public float spawnDelay = 0.3f;
}

[System.Serializable]
public class LevelConfig
{
    public string levelName;
    public WaveInfo[] waves;
}