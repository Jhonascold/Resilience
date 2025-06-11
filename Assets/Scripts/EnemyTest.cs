using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        if (target != null)
            agent.SetDestination(target.position);
    }

    // Opcional: cada frame actualiza destino
    void Update() {
        if (target != null)
            agent.SetDestination(target.position);
    }
}