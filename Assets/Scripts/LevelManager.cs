using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public List<Transform> spawnPoints; 
    public float timeBetweenWaves = 2f;

    void Start()
    {
        LevelConfig config = LevelDatabase.Instance.GetCurrentLevelConfig();
        StartCoroutine(RunLevel(config));
    }

    IEnumerator RunLevel(LevelConfig level)
    {
        for (int waveIndex = 0; waveIndex < level.waves.Length; waveIndex++)
        {
            WaveInfo wave = level.waves[waveIndex];

            Debug.Log("Wave " + (waveIndex + 1) + " bắt đầu!");

            yield return StartCoroutine(SpawnWave(wave));

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("Hoàn tất Level!");
    }

    IEnumerator SpawnWave(WaveInfo wave)
    {
        for (int i = 0; i < wave.enemies.Length; i++)
        {
            for (int j = 0; j < wave.spawnCounts[i]; j++)
            {
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                Instantiate(wave.enemies[i], point.position, wave.enemies[i].transform.rotation);

                yield return new WaitForSeconds(wave.spawnDelay);
            }
        }
    }
}
