using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración")]
    public WaveData[]   waves;               // Oleadas configuradas
    public SpawnPoint[] spawnPoints;         // Puntos de respawn
    public int          maxActiveEnemies = 30;

    [Header("Filtrado de SpawnPoints")]
    [Tooltip("Solo usar spawn points a esta distancia (m) o menos del jugador")]
    [SerializeField] private float maxSpawnDistance = 75f;

    [Header("Cuotas locales")]
    [Tooltip("Radio (m) para contar cuántos enemigos hay en cada zona")]
    [SerializeField] private float localSpawnRadius = 50f;

    [Header("Despawning de lejanos")]
    [Tooltip("A esta distancia (m) los enemigos muy lejos se reciclan")]
    [SerializeField] private float despawnDistanceMultiplier = 2f;

    private Transform        player;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private float despawnDistance;

    private void Awake()
    {
        var go = GameObject.FindWithTag("Player");
        if (go != null) player = go.transform;
        else Debug.LogError("WaveManager: no se encontró ningún GameObject con tag 'Player'");
    }

    private void Start()
    {
        despawnDistance = maxSpawnDistance * despawnDistanceMultiplier;

        StartCoroutine(SpawnLoop());
        StartCoroutine(CullingLoop());
    }

    /// <summary>
    /// Bucle infinito de spawns: mientras no superemos el límite global y local, intentamos crear uno.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            // 1) Control global: ¿hay hueco total?
            if (activeEnemies.Count < maxActiveEnemies)
            {
                // 2) Control local: ¿hay slots cerca del jugador?
                int nearby = CountEnemiesInRange(maxSpawnDistance);
                if (nearby < maxActiveEnemies)
                    TrySpawnOne();
            }
            yield return wait;
        }
    }

    /// <summary>
    /// Cada cierto tiempo recicla al pool los enemigos muy lejos para liberar slots.
    /// </summary>
    private IEnumerator CullingLoop()
    {
        var wait = new WaitForSeconds(2f);
        float sqrDespawn = despawnDistance * despawnDistance;
        while (true)
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                var e = activeEnemies[i];
                if ((e.transform.position - player.position).sqrMagnitude > sqrDespawn)
                {
                    // Recicla sin animación
                    EnemyPool.Instance.ReturnEnemy(e);
                    activeEnemies.RemoveAt(i);
                }
            }
            yield return wait;
        }
    }

    /// <summary>
    /// Cuenta cuántos enemigos vivos están dentro de 'range' del jugador.
    /// </summary>
    private int CountEnemiesInRange(float range)
    {
        float sqr = range * range;
        int count = 0;
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if ((activeEnemies[i].transform.position - player.position).sqrMagnitude <= sqr)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Lanza un intento de spawn de un enemigo aleatorio dentro de la ola actual.
    /// </summary>
    private void TrySpawnOne()
    {
        var wave = waves[0]; // O tu lógica para elegir ola
        int typeIndex = Random.Range(0, wave.enemyTypes.Length);
        SpawnEnemy(wave, typeIndex);
    }

    /// <summary>
    /// Intenta spawnear un enemigo de typeIndex en un punto válido.
    /// </summary>
    private bool SpawnEnemy(WaveData wave, int typeIndex)
    {
        // 1) Filtrar spawnPoints en rango global
        float sqrMax = maxSpawnDistance * maxSpawnDistance;
        var valid = spawnPoints
            .Where(sp => (sp.transform.position - player.position).sqrMagnitude <= sqrMax)
            .ToList();
        if (valid.Count == 0) return false;

        // 2) Calcular cuota local
        int localCap = Mathf.CeilToInt((float)maxActiveEnemies / valid.Count);
        float sqrLocal = localSpawnRadius * localSpawnRadius;

        // 3) Filtrar puntos con espacio local
        var canSpawn = valid
            .Where(sp =>
                activeEnemies.Count(e =>
                    (e.transform.position - sp.transform.position).sqrMagnitude <= sqrLocal
                ) < localCap
            ).ToList();
        if (canSpawn.Count == 0) return false;

        // 4) Elegir spawn point aleatorio
        var chosen = canSpawn[Random.Range(0, canSpawn.Count)];

        // 5) Sacar del pool y reposicionar via Warp
        var enemyGO = EnemyPool.Instance.SpawnEnemy(typeIndex);
        enemyGO.transform.SetParent(null, true);
        if (enemyGO.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.Warp(chosen.transform.position);
            agent.transform.rotation = chosen.transform.rotation;
            agent.isStopped = false;
        }
        else
        {
            enemyGO.transform.position = chosen.transform.position;
            enemyGO.transform.rotation = chosen.transform.rotation;
        }

        // 6) Inyectar stats desde EnemyData
        var data   = wave.enemyTypes[typeIndex];
        var health = enemyGO.GetComponent<Health>(); health.SetMaxHP(data.health);
        var ai     = enemyGO.GetComponent<EnemyAI>(); ai.SetSpeeds(data.walkSpeed, data.runFastSpeed, data.runNormalSpeed);
        var melee  = enemyGO.GetComponent<EnemyMelee>();
        foreach (var hb in melee.hitboxes)
            if (hb.TryGetComponent<MeleeDamageOnTrigger>(out var md))
                md.damage = data.damage;

        // 7) Suscribir muerte para limpiar lista
        void OnDeath()
        {
            health.OnDeath -= OnDeath;
            activeEnemies.Remove(enemyGO);
        }
        health.OnDeath += OnDeath;
        activeEnemies.Add(enemyGO);

        return true;
    }
}
