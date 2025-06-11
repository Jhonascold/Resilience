using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeleeDamageOnTrigger : MonoBehaviour
{
    [Tooltip("Cuánto daño hace este golpe")]
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // Sólo golpeamos al jugador
        if (!other.CompareTag("Player")) return;

        var h = other.GetComponent<Health>();
        if (h != null)
            h.TakeDamage(damage);
    }
}
