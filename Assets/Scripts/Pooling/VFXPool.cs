using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ESTE SCRIPT VA ASOCIADO A UN GAMEOBJECT VACÍO VFXPOOLOBJ     
// se llama desde PlayerWeapon.FireBullet() o desde un StateMachineBehaviour de disparo/impacto.
public class VFXPool : MonoBehaviour
{
    public static VFXPool Instance { get; private set; }

    [System.Serializable]
    public struct VFXEntry
    {
        public string key;     // "MuzzleFlash", "HitSpark", "BuffPickup", etc.
        public GameObject prefab;  // ParticleSystem o prefab con ParticleSystem
        public int size;    // número inicial de instancias
    }

    public VFXEntry[] entries;
    private Dictionary<string, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Pre-instantiate
        foreach (var e in entries)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < e.size; i++)
            {
                var go = Instantiate(e.prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools[e.key] = q;
        }
    }

    /// <summary>Instancia (o reutiliza) un VFX y lo dispara.</summary>
    public void Spawn(string key, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(key, out var q))
        {
            Debug.LogWarning($"No existe pool de VFX “{key}”");
            return;
        }

        GameObject go;
        if (q.Count > 0) go = q.Dequeue();
        else go = Instantiate(entries[0].prefab, transform); // fallback
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        // Asume que el VFX se desactiva solo al terminar su sistema de partículas:
        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
            StartCoroutine(DisableAfterPlay(ps, q));
        else
            q.Enqueue(go);
    }

    private IEnumerator DisableAfterPlay(ParticleSystem ps, Queue<GameObject> q)
    {
        ps.Play();
        yield return new WaitWhile(() => ps.isPlaying);
        ps.gameObject.SetActive(false);
        q.Enqueue(ps.gameObject);
    }
}
