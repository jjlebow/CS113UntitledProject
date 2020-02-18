using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackBehavior : StateMachineBehaviour
{
    [HideInInspector] public FirstBoss boss;
    public float jumpTimer = 0.1f;
    private float copy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        copy = jumpTimer;
        boss = animator.GetComponent<FirstBoss>(); 
        if(boss.attackType == 1)
            boss.Jump();
        else if(boss.attackType == 2)
            boss.SpikeAttack();
        else if(boss.attackType == 3)
            boss.ShortAttack();
        else if(boss.attackType == 4)
            boss.MeteorAttack();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(copy <= 0)
        {
            boss.enableJump = false;
            copy -= Time.deltaTime;
        }
        else
        {
            copy -= Time.deltaTime;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.shortAttackTrigger.SetActive(false);
        //make a function to deactivate all hitboxes?
        animator.SetInteger("AttackType", 0);
        //boss.attackType = 0; leave this variable to represent the last attack that was used in case we need this for some 
        //kind of combo attack that the boss does
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
