using UnityEngine;
using UnityEngine.AI;

public class ScreamBehaviour : StateMachineBehaviour
{
    public AudioClip screamClip;

    public float volume = 1f;
    // Se ejecuta al entrar en el estado Scream
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var ai = animator.GetComponentInParent<EnemyAI>();
        if (ai != null) ai.PauseAgent();

        if (screamClip != null)
        {
            AudioSource.PlayClipAtPoint(screamClip, animator.transform.position, volume);
        }
    }

    // Se ejecuta al salir del estado Scream
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var ai = animator.GetComponentInParent<EnemyAI>();
        if (ai != null) ai.ResumeAgentAfterScream();
    }
}
