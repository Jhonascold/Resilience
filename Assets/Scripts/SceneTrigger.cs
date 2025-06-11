using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena a cargar (Build Settings)")]
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos que el que entra lleva un CharacterController
        if (other.GetComponent<CharacterController>() != null)
        {
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }
}