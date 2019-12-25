using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    //private Boss boss;
    public PlayerController player;
    public GameObject attackTriggerNeutral;
    public GameObject attackTriggerDown;
    public GameObject attackTriggerUp;
    private IEnumerator attacking;
    public float attackCooldown;
    public float attackActiveTime;
    private float attackTimer;
    public static int strength;


    void Awake()
    {    
        attackTriggerNeutral.SetActive(false);
        attackTriggerUp.SetActive(false);
        attackTriggerDown.SetActive(false);
        //boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }

    // Update is called once per frame


    public IEnumerator attackTime()
    {
        StateManager.instance.inCooldown = true;
        StateManager.instance.isAttacking = true;
        attackTimer = attackCooldown + attackActiveTime;
        //this is the amount of time that the weapon hitbox is active (attackCooldown - this time)
        while(attackTimer > attackCooldown)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        //the weapon hitbox deactivates and there is a cooldown before
        //the player is able to use it again. the remaining time(above is the actual Cooldown before the next strike)
        StateManager.instance.isAttacking = false;
        attackTriggerNeutral.SetActive(false);
        attackTriggerUp.SetActive(false);
        attackTriggerDown.SetActive(false);
        while(attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        StateManager.instance.inCooldown = false;
    }

    public void Attack()
    {
        if(StateManager.instance.directionalFacing == StateManager.Directional.UP)
        {
            attackTriggerUp.SetActive(true);
            player.anim.SetInteger("attackDirection", 2);
        }
        else if(StateManager.instance.directionalFacing == StateManager.Directional.DOWN && !StateManager.instance.playerGrounded)
        {
            attackTriggerDown.SetActive(true);
            player.anim.SetInteger("attackDirection", 3);
            //if()
            //{
              //  MidairJump();
            //}
        }
        else if(StateManager.instance.directionalFacing == StateManager.Directional.NEUTRAL)
        {
            attackTriggerNeutral.SetActive(true);
            player.anim.SetInteger("attackDirection", 1);
        }
        attacking = attackTime();
        StartCoroutine(attacking);
    }

    public void attackCancel()
    {
        if(StateManager.instance.inCooldown)
        {
            //Debug.Log("FJDLSJFKDSF");
            StateManager.instance.inCooldown = false;
            StateManager.instance.isAttacking = false;
            attackTriggerNeutral.SetActive(false);
            attackTriggerUp.SetActive(false);
            attackTriggerDown.SetActive(false);
            StopCoroutine(attacking);
        }
    }
}


