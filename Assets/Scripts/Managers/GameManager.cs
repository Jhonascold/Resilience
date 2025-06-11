// =======================
// GameManager.cs
// Se asigna a un GameObject vacío llamado "GameManager"
// =====================================================
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadScene(string sceneName)
    {
        SceneFader.Instance.FadeToScene(sceneName);
    }
    // Ampliable para exponer eventos de juego, UI de oleada, GameOver, etc.
}
