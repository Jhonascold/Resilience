using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroWithSFX : MonoBehaviour
{
    [Header("Intro Prefabs")]
    public GameObject emitterPrefab;     // tu sistema de shards animados
    public GameObject builtPrefab;       // el prefab ya ensamblado

    [Header("Flash Settings")]
    public Image flashImage;             // la UI Image blanca fullscreen
    public float flashDuration = 0.2f;   // duración total del flash
    public float swapDelay = 0f;         // retardo tras el flash

    [Header("Timing")]
    public float waitTime = 3.7f;        // segundos hasta el final de la animación

    [Header("Audio")]
    public AudioSource emitterAudio;     // AudioSource CONTENTIVO del loop
    public AudioClip buildLoopClip;      // sonido durante la animación (loop)
    public AudioClip completeClip;       // sonido al acabar (one-shot)

    void Start()
    {
        // Estado inicial
        emitterPrefab.SetActive(true);
        builtPrefab.SetActive(false);
        flashImage.gameObject.SetActive(false);

        // Arranca la secuencia
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // 1) Espera un frame para que la simulación arranque
        yield return new WaitForEndOfFrame();

        // 2) Inicia el loop de construcción
        if (emitterAudio != null && buildLoopClip != null)
        {
            emitterAudio.clip = buildLoopClip;
            //emitterAudio.loop = true;
            emitterAudio.Play();
        }

        // 3) Espera hasta el final de la animación
        yield return new WaitForSeconds(waitTime);

        // 4) Para el loop y dispara el clip de completado
        if (emitterAudio != null)
        {
            emitterAudio.Stop();
        }
        if (completeClip != null)
        {
            // PlayClipAtPoint usa un AudioSource temporal que no se corta
            AudioSource.PlayClipAtPoint(completeClip, Camera.main.transform.position);
        }

        // 5) Flash blanco
        yield return StartCoroutine(DoFlash());

        // 6) Pequeño retardo opcional
        if (swapDelay > 0f)
            yield return new WaitForSeconds(swapDelay);

        // 7) Swap de prefabs
        emitterPrefab.SetActive(false);
        builtPrefab.SetActive(true);
        StartCoroutine(LoadNextSceneAfterDelay(3f));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MenuPrincipal");
    }

    IEnumerator DoFlash()
    {
        flashImage.gameObject.SetActive(true);
        Color c = flashImage.color;
        c.a = 1f;
        flashImage.color = c;

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = 1f - (t / flashDuration);
            flashImage.color = c;
            yield return null;
        }

        c.a = 0f;
        flashImage.color = c;
        flashImage.gameObject.SetActive(false);
    }
}
