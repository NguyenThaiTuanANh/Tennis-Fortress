using System.Collections.Generic;
using UnityEngine;

public class HitParticlePool : MonoBehaviour
{
    public static HitParticlePool Instance;

    [Header("Particle Prefab")]
    public ParticleSystem particlePrefab;

    [Header("Pool Settings")]
    public int poolSize = 10;

    private List<ParticleSystem> pool;

    private void Awake()
    {
        Instance = this;

        pool = new List<ParticleSystem>();
        for (int i = 0; i < poolSize; i++)
        {
            var ps = Instantiate(particlePrefab, transform);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.gameObject.SetActive(false);
            pool.Add(ps);
        }
    }

    public ParticleSystem GetParticle()
    {
        foreach (var ps in pool)
        {
            if (!ps.gameObject.activeSelf)
            {
                ps.gameObject.SetActive(true);
                return ps;
            }
        }

        // Nếu hết slot → tự expand
        var newPs = Instantiate(particlePrefab, transform);
        pool.Add(newPs);
        return newPs;
    }
}
