using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAlerted : StateMachineBehaviour
{
    private EnemyBehaviour _eBehavior;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _eBehavior = animator.GetComponent<EnemyBehaviour>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_eBehavior.isDead) animator.SetTrigger("triggerDead");
        if (_eBehavior.isStunned) animator.SetBool("isStunned", true);
        _eBehavior.StopMoving();
        
        if (_eBehavior.isAttackOffCooldown) {
            animator.SetBool(
                _eBehavior.maxDistanceToAttack < _eBehavior.GetDistanceToPlayer() ? "isChasing" : "isAttackReady",
                true);
        } 
        else {
            animator.SetBool("isFleeing", true);
        }
    }
}
