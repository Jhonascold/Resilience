using UnityEngine;
using UnityEngine.AI;

public class AttackBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var melee = animator.GetComponentInParent<EnemyMelee>();
        if (melee != null)
        {
            melee.EnableHitboxes();
        
        }

        var ai = animator.GetComponentInParent<EnemyAI>();
        if (ai != null)
        {
            ai.PauseAgent();
            ai.SnapFacePlayer();
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var melee = animator.GetComponentInParent<EnemyMelee>();
        if (melee != null)
        {
            melee.DisableHitboxes();
        }

        var ai = animator.GetComponentInParent<EnemyAI>();
        if (ai != null) ai.ResumeAgentAfterAttack();
    }
}
