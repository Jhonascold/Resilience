using UnityEngine;

/// <summary>
/// Distributes damage to any Health component it collides with.
/// Implements IDamageDealer to expose its damage value.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageOnCollision : MonoBehaviour, IDamageDealer
{
    [Header("Damage Settings")]
    [Tooltip("Amount of damage dealt on collision.")]
    [SerializeField] private float damage = 10f;

    /// <summary>
    /// Amount of damage this object inflicts.
    /// </summary>
    public float Damage => damage;

    public GameObject impactEffectPrefab;

    private void OnCollisionEnter(Collision other)
    {
        // If the other object has a Health component, apply damage
        var health = other.collider.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

    }
}
