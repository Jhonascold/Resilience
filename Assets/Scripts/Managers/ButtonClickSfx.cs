using UnityEngine;

public class ButtonClickSfx : MonoBehaviour
{
    [Tooltip("Clip que suena al hacer click en cualquier botón")]
    public AudioClip clickClip;

    AudioSource sfxSource;

    void Awake()
    {
        // Asume que el AudioSource para SFX está en el mismo GameObject
        sfxSource = GetComponent<AudioSource>();
    }

    // Método público para enlazar desde el OnClick() del Button
    public void PlayClick()
    {
        if (clickClip != null && sfxSource != null)
            sfxSource.PlayOneShot(clickClip);
    }
}