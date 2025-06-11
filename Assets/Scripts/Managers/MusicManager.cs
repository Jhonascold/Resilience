using UnityEngine;
using UnityEngine.SceneManagement;   // ← necesario




[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip backgroundClip;    // bucle general
    public AudioClip winClip;           // gana (loop=false)
    public AudioClip loseClip;          // pierde (loop=false)

    AudioSource src;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            src = GetComponent<AudioSource>();
            // configuramos la música de fondo por defecto
            src.clip = backgroundClip;
            src.loop = true;
            src.Play();

            // nos suscribimos al evento de cambio de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Se llama siempre que se carga una escena nueva
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // nombre exacto de tu escena de “Game Over”
        if (scene.name == "GameOver")
        {
            src.clip = loseClip;
            src.loop = false;
            src.Play();
        }
        // nombre exacto de tu escena de “Win” o “Fin de la Beta”
        else if (scene.name == "Win")
        {
            src.clip = winClip;
            src.loop = false;
            src.Play();
        }
        else
        {
            // cualquier otra escena → volvemos a fondo
            if (src.clip != backgroundClip)
            {
                src.clip = backgroundClip;
                src.loop = true;
                src.Play();
            }
        }
    }

    void OnDestroy()
    {
        // limpiamos la suscripción si se destruye
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
