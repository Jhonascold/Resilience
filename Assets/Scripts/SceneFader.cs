using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    // --- 1) Singleton ---
    public static SceneFader Instance { get; private set; }

    [Header("Configuración de fundido")]
    public Image fadeImage;       // enlace a la Image negra de UI
    public float fadeDuration = 1f;

    void Awake()
    {
        // --- 1.1) Inicialización del singleton ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- 2) Comenzar con fundido de entrada desde negro ---
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(Fade(1f, 0f));
    }

    /// <summary>
    /// Llama a esta función para iniciar la transición a otra escena.
    /// </summary>
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(PerformTransition(sceneName));
    }

    IEnumerator PerformTransition(string sceneName)
    {
        // 1) Fundir a negro
        yield return Fade(0f, 1f);
        // 2) Cargar escena en segundo plano
        yield return SceneManager.LoadSceneAsync(sceneName);
        // 3) Fundir a transparente
        yield return Fade(1f, 0f);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, to);
    }
}
