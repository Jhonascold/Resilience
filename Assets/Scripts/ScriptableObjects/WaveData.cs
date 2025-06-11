// =======================
// WaveData.cs
// ScriptableObject en Assets/ScriptableObjects
// =====================================================
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    public EnemyData[] enemyTypes;
    public int[] counts;  // counts[i] corresponde a enemyTypes[i]
}