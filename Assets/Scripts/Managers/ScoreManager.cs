using UnityEngine;
using UnityEngine.UI; // O TMPro si usas TextMeshPro
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Instancia global

    public int score = 0; // Puntuación actual
    //public TMP_Text scoreText; // Referencia al UI Text (si usas TextMeshPro, cambia el tipo a TMP_Text)

    void Awake()
    {
        // Singleton: si ya existe una instancia, destruye la nueva
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //UpdateScoreUI();
    }

    // Método para sumar puntos
    public void AddScore(int points)
    {
        score += points;
        //UpdateScoreUI();
    }

    public void ResetValues()
    {
        score = 0;
        
    }

    // Actualiza el texto de la puntuación
    /*
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }*/
}


