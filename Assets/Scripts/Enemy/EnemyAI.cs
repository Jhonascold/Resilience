using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    private Transform player;  // Referencia al jugador al que perseguir. Le he quitado el [SerializeField] porque no es necesario asignarlo desde el editor.

    [Header("Radios")]
    [SerializeField, Tooltip("Distancia para iniciar Scream→RunFast→JumpAttack")]
    private float radiusX = 20.0f;
    [SerializeField, Tooltip("Ataques ligeros/fuertes")]
    private float radiusY = 2.2f;
    [SerializeField, Tooltip("Distancia para iniciar JumpAttack")]
    private float radiusZ = 10.0f; 

    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runFastSpeed = 8f;
    [SerializeField] private float runNormalSpeed = 3f;

    [Header("Duraciones de animación")]
    [SerializeField, Tooltip("Cooldown mínimo entre Light/Strong Attack")]
    private float attackCooldown = 1.1f;

    private NavMeshAgent agent;
    private Animator animator;
    private Health healthComponent;

    // Flags de ciclo
    private bool isInsideX = false;
    private bool hasEnteredXThisCycle = false;
    private bool hasJumpAttackedThisCycle = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        healthComponent = GetComponent<Health>();

        var go = GameObject.FindGameObjectWithTag("Player");
        if(go != null) player = go.transform;
        else Debug.LogError($"[EnemyAI] No se ha encontrado el jugador. Asegúrate de que el objeto tiene el tag 'Player'.");

        agent.speed = walkSpeed;
    }

    private void Update()
    {

        Vector3 selfPos = transform.position;
        Vector3 playerPos = player.position;
        selfPos.y = 0f; // Ignoramos la altura
        playerPos.y = 0f; // Ignoramos la altura
        float dist = Vector3.Distance(selfPos, playerPos);

        bool inRangeY = dist <= radiusY;    // Para comprobar si el jugador está dentro del rango Y y en caso de que no lo esté, se ponga a correr.
        animator.SetBool("IsInRangeY", inRangeY);

        /*Debug.Log($"[EnemyAI] Update(): isStopped={agent.isStopped}, " +
              $"hasPath={agent.hasPath}, pathPending={agent.pathPending}");*/

        // 1) Entrada / salida radio X
        if (!isInsideX && dist <= radiusX)
        {
            //Debug.Log($"[EnemyAI] Player entered radiusX ({dist:F2} <= {radiusX})");
            OnEnterX();
        }
        else if (isInsideX && dist > radiusX)
        {
            //Debug.Log($"[EnemyAI] Player exited radiusX ({dist:F2} > {radiusX})");
            OnExitX();
        }
        // 2) Si estamos dentro de X, comprobamos salto y ataques
        if (isInsideX && hasEnteredXThisCycle)
        {
            // JumpAttack único
            if (!hasJumpAttackedThisCycle && dist <= radiusZ)
            {
                //Debug.Log($"[EnemyAI] Triggering JumpAttack ({dist:F2} <= {radiusY})");
                OnJumpAttack();
            }
            // Ataques ligeros/fuertes aleatorios una vez haya saltado
            if (hasJumpAttackedThisCycle && dist <= radiusY && Time.time >= nextAttackTime)
            {
                //Debug.Log($"[EnemyAI] Ready for Light/Strong Attack (cooldown passed)");
                TriggerRandomLightOrStrong();
            }

            if (hasJumpAttackedThisCycle && dist > radiusY)
            {
                agent.speed = runNormalSpeed;
            }
        }

        // 3) Mover siempre hacia el jugador (si no está detenido)
        if (!agent.isStopped)
            agent.SetDestination(player.position);
    }

    private void OnEnterX()
    {
        isInsideX = true;
        hasEnteredXThisCycle = true;
        hasJumpAttackedThisCycle = false;
        animator.SetTrigger("EnterX");
    }

    private void OnJumpAttack()
    {
        hasJumpAttackedThisCycle = true;
        animator.SetTrigger("JumpAttack");

    }

    private void TriggerRandomLightOrStrong()
    {
        // Elegimos ligero o fuerte al 50%
        if (Random.value < 0.5f)
            animator.SetTrigger("LightAttack");
        else
            animator.SetTrigger("StrongAttack");

        nextAttackTime = Time.time + attackCooldown;
    }


    /// <summary>
    /// Parar por completo al NavMeshAgent. Llamado desde ScreamBehaviour.OnStateEnter
    /// </summary>
    public void PauseAgent()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.speed     = 0f;
    }

    /// <summary>
    /// Reanudar tras Scream. Llamado desde ScreamBehaviour.OnStateExit
    /// </summary>
    public void ResumeAgentAfterScream()
    {
        agent.speed     = runFastSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    /// <summary>
    /// Reanudar tras JumpAttack. Llamado desde JumpAttackBehaviour.OnStateExit
    /// </summary>
    public void ResumeAgentAfterJump()
    {
        agent.speed     = walkSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        nextAttackTime = Time.time + attackCooldown;
    }

    /// <summary>
    /// Reanudar tras LightAttack o StrongAttack. Llamado desde AttackBehaviour.OnStateExit
    /// </summary>
    public void ResumeAgentAfterAttack()
    {
        agent.speed = 3.0f;//walkSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        nextAttackTime = Time.time + attackCooldown;
    }

    public void SnapFacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f; // Ignoramos la altura
        if (dir.sqrMagnitude < 0.001f) return; // Si el jugador está muy cerca, no rotamos
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetSpeeds(float walk, float runFast, float runNorm)
    {
        walkSpeed = walk;
        runFastSpeed = runFast;
        runNormalSpeed = runNorm;
    }

    public float GetWalkSpeed() => walkSpeed;


    private void OnExitX()
    {
        isInsideX = false;
        hasEnteredXThisCycle = false;
        hasJumpAttackedThisCycle = false;
        animator.SetTrigger("ExitX");

        // Reset de movimiento
        agent.isStopped = false;
        agent.speed = walkSpeed;
    }

}
