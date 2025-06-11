using System;
using UnityEngine;

// ESTE SCRIPT VA ASOCIADO A UN ENEMY PREFAB Y A UN PLAYER PREFAB.

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Tooltip("Maximum health points for this entity.")]
    private float maxHP = 100f;

    [SerializeField] private HealthBar healthSlider;

    private float currentHP;

    /// <summary>
    /// Invoked whenever the health value changes.
    /// Parameters: (currentHP, maxHP)
    /// </summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>
    /// Invoked when health reaches zero.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// The current health points.
    /// </summary>
    public float CurrentHP => currentHP;

    /// <summary>
    /// The maximum health points.
    /// </summary>
    public float MaxHP => maxHP;

    private void Awake()
    {
        // Initialize current health.
        currentHP = maxHP;
        // Notify any listeners of the initial health.
        OnHealthChanged?.Invoke(currentHP, maxHP);

        healthSlider?.SetHealth(1f); // Set initial health to 100% in the UI
    }

    /// <summary>
    /// Reduces health by the specified amount. Triggers OnDeath if health falls to zero or below.
    /// </summary>
    /// <param name="amount">Amount of damage to take (must be non-negative).</param>
    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        Debug.Log($"[Health] {gameObject.name} recibe {amount} de daño. HP antes: {currentHP}/{maxHP}");

        currentHP = Mathf.Max(currentHP - amount, 0f);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        Debug.Log($"[Health] {gameObject.name} ahora tiene {currentHP}/{maxHP} de vida");

        if(healthSlider != null)
        {
            float healthPercent = Mathf.Clamp01(currentHP / maxHP); 
            healthSlider.SetHealth(healthPercent);
        }

        if (currentHP <= 0f)
            Die();
    }

    /// <summary>
    /// Restores health by the specified amount, up to maxHP.
    /// </summary>
    /// <param name="amount">Amount to heal (must be non-negative).</param>
    public void Heal(float amount)
    {
        if (amount <= 0f || currentHP <= 0f) return;

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void SetMaxHP(float hp)
    {
        maxHP = hp;
        currentHP = hp;
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if(healthSlider != null)
        {
            healthSlider.SetHealth(1f);
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}

