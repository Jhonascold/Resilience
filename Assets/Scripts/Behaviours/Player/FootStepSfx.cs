using UnityEngine;
using UnityEngine.Audio;

public class FootStepSfx : StateMachineBehaviour
{
    [Header("Clip y Volumen")]
    public AudioClip clip;
    [Range(0.1f, 1f)] public float volume = 1f;

    [Header("Atenuación 3D")]
    public AnimationCurve rolloffCurve = 
        new AnimationCurve(
            new Keyframe(0f, 1f),   // 100% a 0 m
            new Keyframe(20f, 0f)   //  0% a 20 m
        );
    public float minDistance = 0f;
    public float maxDistance = 20f;

    [Header("Mixer Group (SFX3D)")]
    public AudioMixerGroup mixerGroup;

    private AudioSource audioSource;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 1) Busca un AudioSource en el objeto o en cualquiera de sus hijos (p.ej. tu arma)
        audioSource = animator.gameObject.GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError(
                $"[FootStepSfx] No se encontró ningún AudioSource en {animator.gameObject.name} ni en sus hijos. " +
                "Por favor añade un AudioSource (configurado como 3D) en el arma o en el prefab."
            );
            return;
        }

        // 2) Configura la spatialización 3D y la curva de rolloff
        audioSource.spatialBlend = 1f;                         // sonido 100% 3D
        audioSource.rolloffMode = AudioRolloffMode.Custom;     // usaremos nuestra curva
        audioSource.SetCustomCurve(
            AudioSourceCurveType.CustomRolloff, 
            rolloffCurve
        );
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        // 3) Ajustes de reproducción
        audioSource.playOnAwake = false;                      
        audioSource.loop = true;                              
        audioSource.outputAudioMixerGroup = mixerGroup;       

        // 4) Asigna el clip y lanza el sonido
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}

