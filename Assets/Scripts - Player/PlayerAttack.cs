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
    public GameObject attackTriggerNeutralCombo2;
    public GameObject attackTriggerNeutralCombo3;
    public GameObject attackTriggerAerial;
    //private IEnumerator attacking;
    //public float attackCooldown;
    public float attackActiveTime;
    private float attackTimer;
    public static int strength = 15;
    private float timeBtwAttack = 0.7f;   //time in between each attack
    private IEnumerator attackCooldownCoroutine;

    //consider a cancel bool value that puts all other states to false 


    void Awake()
    {    
        NullifyAttackHitboxes();
        //boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }

    // Update is called once per frame


/*
    public IEnumerator attackTime()
    {
        StateManager.instance.attackCooldown = true;
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
        StateManager.instance.attackCooldown = false;
    }*/

    public void Attack()
    {
    
        //StateManager.instance.attackCooldown = true;
        StateManager.instance.attackInitiate = true;
        StateManager.instance.isAttacking = true;
        attackCooldownCoroutine = AttackCooldownTimer();
        if(StateManager.instance.playerGrounded == true && StateManager.instance.stance == false)
        {
            StateManager.instance.playerState = StateManager.PlayerStates.HOLD;
            //Debug.Log("WE ARE HERE");
            player.m_Rigidbody2D.velocity = new Vector3(0,0,0);
        }
        if(StateManager.instance.directionalFacing == StateManager.Directional.UP)
        {
            //attackTriggerUp.SetActive(true);
            player.playerAnim.SetInteger("attackDirection", 3);
            StartCoroutine(attackCooldownCoroutine);
        }
        else if(StateManager.instance.directionalFacing == StateManager.Directional.DOWN && !StateManager.instance.playerGrounded)
        {
            player.playerAnim.SetInteger("attackDirection", 2);
            StartCoroutine(attackCooldownCoroutine);
            //attackTriggerDown.SetActive(true);
            
            //if()
            //{
              //  MidairJump();
            //}
        }
        else if(StateManager.instance.directionalFacing == StateManager.Directional.NEUTRAL)
        {
            //Debug.Log("AND HERE");
            player.playerAnim.SetInteger("attackDirection", 1);
            StartCoroutine(attackCooldownCoroutine);
            //attackTriggerNeutral.SetActive(true);
            
        }
        else
        {
            StopCoroutine(attackCooldownCoroutine);
            StateManager.instance.attackCooldown = false;
            StateManager.instance.attackInitiate = false;
            StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
            StateManager.instance.isAttacking = false;
        }
        //attacking = attackTime();
        //StartCoroutine(attacking);
    }

    public void attackCancel()   //add some kind of cancel variable here
    {
        if(StateManager.instance.attackCooldown)
        {
            //Debug.Log("FJDLSJFKDSF");
            StopCoroutine(attackCooldownCoroutine);
            StateManager.instance.attackCooldown = false;
            StateManager.instance.isAttacking = false;
            StateManager.instance.attackInitiate = false;
            StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
            //StateManager.instance.cantAttack = false;
            NullifyAttackHitboxes();
            //StopCoroutine(attacking);
        }
    }

    public IEnumerator AttackCooldownTimer()
    {
        float copy = timeBtwAttack;
        StateManager.instance.attackCooldown = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        StateManager.instance.attackCooldown = false;
    }

    private void NullifyAttackHitboxes()
    {
        attackTriggerNeutral.SetActive(false);
        attackTriggerUp.SetActive(false);
        attackTriggerDown.SetActive(false);
        attackTriggerNeutralCombo2.SetActive(false);
        attackTriggerNeutralCombo3.SetActive(false);
        attackTriggerAerial.SetActive(false); 
        StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
    }

    public void SwitchStance()
    {
        //StateManager.instance.stance = true;
    }
}


