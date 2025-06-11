using UnityEngine;
using UnityEngine.Audio;

public class RunningStepSfx : StateMachineBehaviour
{
    [Header("Clip y Volumen")]
    public AudioClip clip;
    [Range(0.1f, 1f)] public float volume = 1f;

    [Header("Rolloff personalizado")]
    public AnimationCurve rolloffCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(20f, 0f)
    );
    public float minDistance = 0f;
    public float maxDistance = 20f;

    [Header("Mixer (arrástralo a SFX3D)")]
    public AudioMixerGroup mixerGroup;

    private AudioSource audioSource;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ENTER RUN");

        // 1) Busca tu AudioSource en el objeto o cualquiera de sus hijos
        audioSource = animator.gameObject.GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError($"[RunningStepSfx] No se encontró AudioSource en '{animator.gameObject.name}' ni en sus hijos.");
            return;
        }

        // 2) Configura SIEMPRE el AudioSource (aunque ya exista)
        audioSource.spatialBlend                = 1f;                        // 100% 3D
        audioSource.rolloffMode                 = AudioRolloffMode.Custom;    // curva custom
        audioSource.SetCustomCurve(
            AudioSourceCurveType.CustomRolloff, 
            rolloffCurve
        );
        audioSource.minDistance                 = minDistance;
        audioSource.maxDistance                 = maxDistance;
        audioSource.outputAudioMixerGroup       = mixerGroup;
        audioSource.playOnAwake                 = false;
        audioSource.loop                        = true;

        // 3) Asigna clip y volumen
        audioSource.clip   = clip;
        audioSource.volume = volume;

        // 4) Arranca la reproducción y haz un log para verificar
        audioSource.Play();
        Debug.Log(
          $"[RUNNING SFX] clip={audioSource.clip.name}, " +
          $"isPlaying={audioSource.isPlaying}, vol={audioSource.volume}"
        );
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}
