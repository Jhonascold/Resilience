// =======================
// EnemyData.cs
// ScriptableObject en Assets/ScriptableObjects
// =====================================================
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Waves/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;     // Prefab de enemigo
    public float health = 100f;
    public float walkSpeed = 1f;
    public float runFastSpeed = 8f;

    public float runNormalSpeed = 3.0f;
    public float damage = 10f;
    // añade más stats: ataque, etc.
}