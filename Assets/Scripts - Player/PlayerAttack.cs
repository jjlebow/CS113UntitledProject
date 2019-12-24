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
    void Update()
    {
        //this chain of if statements is used to determine which direction the attack is used in. GetKey is used instead so that we can read 
        //multiple inputs at once
        if((Input.GetAxisRaw("Vertical") < 0) && Input.GetKey(KeyCode.K) && (StateManager.instance.playerGrounded == false) && !StateManager.instance.inCooldown)
        {
            StateManager.instance.attackDir = StateManager.AttackDirection.DOWN;
            Attack();
        }
        else if((Input.GetAxisRaw("Vertical") > 0) && Input.GetKey(KeyCode.K) &&!StateManager.instance.inCooldown)
        {
            StateManager.instance.attackDir = StateManager.AttackDirection.UP;
            Attack();
        }
        else if(Input.GetKeyDown(KeyCode.K) && !StateManager.instance.inCooldown)
        {
            StateManager.instance.attackDir = StateManager.AttackDirection.NEUTRAL;
            Attack();
        }
        else if(!StateManager.instance.inCooldown)
        {
            StateManager.instance.attackDir = StateManager.AttackDirection.NOATTACK;
        }
    }

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
        if(StateManager.instance.attackDir == StateManager.AttackDirection.UP)
        {
            attackTriggerUp.SetActive(true);
        }
        else if(StateManager.instance.attackDir == StateManager.AttackDirection.DOWN)
        {
            attackTriggerDown.SetActive(true);
            //if()
            //{
              //  MidairJump();
            //}
        }
        else
        {
            attackTriggerNeutral.SetActive(true);
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


