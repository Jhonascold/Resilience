using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Tooltip("Arrastra aquí tus hit-boxes (RightHand_Hitbox, LeftHand_Hitbox…)")]
    public GameObject[] hitboxes;

    private void Awake()
    {
        // Asegura que empiecen todos desactivados
        foreach (var hb in hitboxes)
            hb.SetActive(false);
    }

    /// <summary>Activa todos los hit-boxes.</summary>
    public void EnableHitboxes()
    {
        foreach (var hb in hitboxes)
            hb.SetActive(true);
    }

    /// <summary>Desactiva todos los hit-boxes.</summary>
    public void DisableHitboxes()
    {
        foreach (var hb in hitboxes)
            hb.SetActive(false);
    }
}
