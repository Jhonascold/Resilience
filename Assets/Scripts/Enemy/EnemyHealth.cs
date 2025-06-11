using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//ESTE SCRIPT VA ASOCIADO A UN ENEMY PREFAB

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Death Settings")]
    [Tooltip("Tiempo que tarda la animación de muerte en completarse antes de devolver al pool.")]
    [SerializeField] private float deathDuration = 2.3f;
    [SerializeField] private HealthBar healthSlider;

    private Health healthComponent;
    private Animator      animator;
    private NavMeshAgent  agent;
    private EnemyAI       ai;
    private Collider      col;

    private bool isDead;

    public AudioClip deathSound;
    
    private void Start()
    {
        // Inicializar la UI de salud si está asignada
        if (healthSlider != null)
        {
            healthSlider.SetHealth(1f); // Asignar salud inicial al 100%
        }
    }

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<EnemyAI>();
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        // Reset al reactivar
        isDead = false;
        if (col   != null) col.enabled      = true;
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = ai != null ? ai.GetWalkSpeed() : agent.speed;
        }

        if(ai != null) ai.enabled = true;

        healthComponent.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        healthComponent.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // 1) Parar IA y navegación
        if (ai != null) ai.enabled = false;
        if (agent != null) agent.isStopped = true;

        // 2) Desactivar collider
        if (col != null) col.enabled = false;

        // 3) Disparar animación de muerte
        animator.SetTrigger("Dead");

        ScoreManager.Instance.AddScore(250);    // Puntuacion por defecto.

        // 4) Encolar devolución al pool
        StartCoroutine(ReturnToPoolAfterDeath());     
    }

    private IEnumerator ReturnToPoolAfterDeath()
    {
        yield return new WaitForSeconds(deathDuration);
        EnemyPool.Instance.ReturnEnemy(gameObject);   
    }
}
