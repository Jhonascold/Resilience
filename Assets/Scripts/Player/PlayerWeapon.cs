// PlayerWeapon.cs
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerWeapon : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject gunObject;

    [SerializeField] private AudioClip muzzleSfx;

    [Header("Disparo")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate    = 0.2f;

    private Animator    animator;
    private Renderer[]  gunRenderers;
    private float       nextFireTime;
    private Vector3     currentAimPoint;
    private bool        wasAimingLastFrame;

    [Header("Efectos")]
    [SerializeField] private GameObject muzzleFlashPrefab;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Cacheamos renderers del arma
        if (gunObject != null)
            gunRenderers = gunObject.GetComponentsInChildren<Renderer>(true);
        else
            Debug.LogWarning("PlayerWeapon: asigna gunObject en el Inspector.");

        if (muzzlePoint == null)
            Debug.LogWarning("PlayerWeapon: asigna muzzlePoint en el Inspector.");
    }

    private void Update()
    {
        HandleAiming();
        if (animator.GetBool("isAiming"))
            HandleShooting();
    }

    private void HandleAiming()
    {
        bool isAiming = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAiming);

        // Mostrar/ocultar arma sólo si cambió el estado
        if (isAiming != wasAimingLastFrame)
            SetGunVisibility(isAiming);

        // Si estamos apuntando, actualizamos el punto de mira y blend tree
        if (isAiming)
            UpdateAimAndBlend();

        wasAimingLastFrame = isAiming;
    }

    private void UpdateAimAndBlend()
    {
        // Calculamos punto de mira en plano horizontal al nivel del cañón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane aimPlane = new Plane(Vector3.up, muzzlePoint.position);

        if (aimPlane.Raycast(ray, out float enter))
        {
            currentAimPoint = ray.GetPoint(enter);

            // Blend tree de apuntado (MoveX/MoveY) basado en input local
            Vector3 worldDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 localDir = transform.InverseTransformDirection(worldDir);
            animator.SetFloat("MoveX", localDir.x);
            animator.SetFloat("MoveY", localDir.z);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireBullet();
        }
    }

    private void FireBullet()
    {
        if (muzzleSfx != null)
        {
            AudioSource.PlayClipAtPoint(muzzleSfx, muzzlePoint.position, 1f);        
        }

        // Obtenemos la bala del pool
        //GameObject bullet = BulletPool.Instance.SpawnBullet();
        if (BulletPool.Instance == null)
        {
            //Debug.LogError("PlayerWeapon: BulletPool no está inicializado.");
            return;
        }

        GameObject bullet = BulletPool.Instance.SpawnBullet();
        Quaternion correction = Quaternion.Euler(-90f, 180f, 0f);
        if (bullet == null)
        {
            //Debug.LogWarning("PlayerWeapon: no hay balas disponibles en el pool.");
            return;
        }

        // Posicionamos y rotamos la bala
        bullet.transform.position = muzzlePoint.position;
        Vector3 dir = (currentAimPoint - muzzlePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(dir) * correction;

        // Disparamos usando el método público de PooledBullet
        if (bullet.TryGetComponent<PooledBullet>(out var pooled))
            pooled.Shoot(dir, bulletSpeed);

        /*// Instanciar el fogueo
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
            Destroy(flash, 0.2f); // Destruye el fogueo tras 0.2 segundos (ajusta si es necesario)
        }*/
    }

    private void SetGunVisibility(bool visible)
    {
        if (gunRenderers == null) return;
        foreach (var r in gunRenderers)
            r.enabled = visible;
    }
}

