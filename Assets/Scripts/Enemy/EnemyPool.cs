// =======================
// EnemyPool.cs
// Se asigna a un GameObject vacío llamado "EnemyPoolObj"
// =====================================================
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    [Header("Pool Settings")]
    public GameObject[] enemyPrefabs;    // Prefabs de enemigos configurados vía Inspector
    public int initialSize = 20;         // Tamaño inicial del pool. Si se quiere modificar los enemigos activos de esos 20, se hace desde el wavemanager.

    private readonly List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Pre-instantiate
        foreach (var prefab in enemyPrefabs)
        {
            for (int i = 0; i < initialSize; i++)
            {
                var e = Instantiate(prefab, transform);
                e.SetActive(false);
                pool.Add(e);
            }
        }
    }

    /// <summary>Spawnea un enemigo del tipo index o expande el pool.</summary>
    public GameObject SpawnEnemy(int prefabIndex)
    {
        // Busca inactivo del mismo tipo
        foreach (var e in pool)
            if (!e.activeInHierarchy && e.name.StartsWith(enemyPrefabs[prefabIndex].name))
                return Activate(e);

        // Si no hay, crea uno nuevo
        var ne = Instantiate(enemyPrefabs[prefabIndex], transform);
        pool.Add(ne);
        return Activate(ne);
    }

    private GameObject Activate(GameObject e)
    {
        e.SetActive(true);
        return e;
    }

    public void ReturnEnemy(GameObject e)
    {
        e.SetActive(false);
    }
}
