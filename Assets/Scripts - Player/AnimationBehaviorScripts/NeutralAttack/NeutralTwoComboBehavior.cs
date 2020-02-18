using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralTwoComboBehavior : StateMachineBehaviour
{
    private PlayerAttack playerAttacker;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerAttacker == null)
            playerAttacker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        if(StateManager.instance.playerGrounded && StateManager.instance.stance == false) 
            StateManager.instance.playerState = StateManager.PlayerStates.HOLD;
        Debug.Log("HERE");
        playerAttacker.attackTriggerNeutralCombo2.SetActive(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Input.GetKey(KeyCode.K))
            StateManager.instance.attackContinue = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAttacker.attackTriggerNeutralCombo2.SetActive(false);  
        StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
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
