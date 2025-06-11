using System.Collections;
using System.Collections.Generic;

// ESTE SCRIPT VA ASIGNADO A UN OBJETO CON UN COMPONENTE CharacterController, EN MI CASO EL PLAYER

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed  = 5f;

    [Header("Rotaciones")]
    [SerializeField] private float locomotionRotationSpeed = 10f;
    [SerializeField] private float aimRotationSpeed       = 15f;
    private static readonly Quaternion aimBodyOffset      = Quaternion.Euler(0f, 20f, 0f);

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 1) Input de movimiento
        Vector2 inputDir   = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 worldDir   = new Vector3(inputDir.x, 0f, inputDir.y).normalized;
        bool    isAiming   = Input.GetMouseButton(1);
        bool    isRunning  = Input.GetKey(KeyCode.LeftShift);

        // 2) Animador: locomoción y modo apuntado
        animator.SetBool("isAiming", isAiming);
        HandleMovement(worldDir, isRunning);

        // 3) Rotación cuerpo
        if (isAiming)
            HandleAimingRotation();
        else
            HandleLocomotionRotation(worldDir);
    }

    #region Movilidad

    private void HandleMovement(Vector3 dir, bool running)
    {
        float speed = running ? runSpeed : walkSpeed;
        cc.SimpleMove(dir * speed);

        // Blend-Tree Speed
        float animSpeed = dir.magnitude * (running ? 1f : 0.5f);
        animator.SetFloat("Speed", animSpeed);
    }

    #endregion

    #region Rotaciones

    private void HandleLocomotionRotation(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;

        float angle   = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        Quaternion target = Quaternion.Euler(0f, snapped, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            locomotionRotationSpeed * Time.deltaTime
        );
    }

    private void HandleAimingRotation()
    {
        // Rotación del cuerpo hacia donde apunta el ratón
        Ray   ray      = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane    = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit         = ray.GetPoint(enter);
            Vector3 lookDir     = hit - transform.position;
            lookDir.y = 0f;
            Quaternion target = Quaternion.LookRotation(lookDir) * aimBodyOffset;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                aimRotationSpeed * Time.deltaTime
            );
        }
    }

    #endregion
}
