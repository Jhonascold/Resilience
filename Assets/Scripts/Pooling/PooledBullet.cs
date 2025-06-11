using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PooledBullet : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo que la bala permanece activa si no choca con nada.")]
    public float lifeTime = 5f;
    [Tooltip("Daño que inflige al impactar.")]
    public float damage   = 10f;

    private Rigidbody rb;
    private Coroutine lifeCoroutine;

    public GameObject impactEffectPrefab;

    public AudioClip impactSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // En lugar de Update, arrancamos un temporizador
        lifeCoroutine = StartCoroutine(ReturnAfterTime());
    }

    void OnDisable()
    {
        // Cancelar la corrutina si desactivan la bala antes
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);
    }

    IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        BulletPool.Instance.ReturnBullet(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1) Intentar hacer daño
        var health = collision.collider.GetComponent<Health>();
        
        if (health != null)
        {
            Vector3 impactPoint = collision.GetContact(0).point;

            Quaternion effectRotation = Quaternion.LookRotation(collision.GetContact(0).normal);

            GameObject effect = Instantiate(impactEffectPrefab, impactPoint, effectRotation);

            Destroy(effect, 2f); 

            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, impactPoint);
            }

            health.TakeDamage(damage);
        }
            

        // 2) Devolver la bala al pool
        BulletPool.Instance.ReturnBullet(gameObject);
    }

    /// <summary>
    /// Llamar desde quien la dispare para darle velocidad.
    /// </summary>
    public void Shoot(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
    }
}
