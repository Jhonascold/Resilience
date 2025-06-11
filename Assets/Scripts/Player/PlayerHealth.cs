// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// ESTE SCRIPT DEBE ESTAR ASIGNADO A UN JUGADOR

[RequireComponent(typeof(Health))]
public class PlayerHealth : MonoBehaviour
{
    [Header("UI (opcional)")]
    [SerializeField] private HealthBar healthSlider;

    private Health healthComponent;
    private Animator animator;
    private PlayerMovement movement;
    private PlayerWeapon weapon;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeapon>();
    }

    private void OnEnable()
    {
        healthComponent.OnHealthChanged += UpdateHealthUI;
        healthComponent.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        healthComponent.OnHealthChanged -= UpdateHealthUI;
        healthComponent.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        if (healthSlider != null)
        {
            //healthSlider.maxValue = healthComponent.MaxHP;
            //healthSlider.value = healthComponent.CurrentHP;
            healthSlider.SetHealth(1f);

        }
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        //if (healthSlider != null)
            //healthSlider.value = currentHP;
    }

    private void HandleDeath()
    {
        // Animación de muerte
        animator.SetTrigger("Dead");

        // Desactivar controles del jugador
        if (movement != null) movement.enabled = false;
        if (weapon != null) weapon.enabled = false;

        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        StartCoroutine(LoadNextSceneAfterDelay(3f));

        
    }
    
    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOver"); 
    }
}

