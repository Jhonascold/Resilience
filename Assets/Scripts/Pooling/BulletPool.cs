using UnityEngine;
using System.Collections.Generic;

// ESTE SCRIPT VA ASIGNADO A UN OBJETO VACIO LLAMADO BULLETPOOLOBJ DONDE HABRA QUE ASIGNAR EL PREFAB DE LA BALA
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [Header("Pool Settings")]
    public GameObject bulletPrefab;
    public int initialSize = 20;

    public GameObject impactEffectPrefab;

    private readonly List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Pre-instantiate
        for (int i = 0; i < initialSize; i++)
        {
            var b = Instantiate(bulletPrefab, transform);
            b.SetActive(false);

            b.GetComponent<PooledBullet>().impactEffectPrefab = impactEffectPrefab;
            pool.Add(b);
        }
    }

    /// <summary>Devuelve una bala inactiva del pool (o expande si hace falta).</summary>
    public GameObject SpawnBullet()
    {
        foreach (var b in pool)
            if (!b.activeInHierarchy)
                return Activate(b);

        // Si no quedan, creamos una nueva
        var nb = Instantiate(bulletPrefab, transform);

        nb.GetComponent<PooledBullet>().impactEffectPrefab = impactEffectPrefab;
        pool.Add(nb);
        return Activate(nb);
    }

    private GameObject Activate(GameObject b)
    {
        b.SetActive(true);
        return b;
    }

    /// <summary>Vuelve a desactivar la bala y la deja lista para reutilizar.</summary>
    public void ReturnBullet(GameObject b)
    {
        b.SetActive(false);
    }
}
