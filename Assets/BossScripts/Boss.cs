using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    [HideInInspector] public bool attackTrigger = false;
    public int health;
    public int damage;
    private float timeBtwCollision = 0.1f;  //this is to prevent multiple hitboxes hitting each other at once. basically boss invuln frames between damage
    


    //public Animator camAnim;
    public Slider healthBar;
    private Animator anim;
    [HideInInspector] public bool isDead;
    private PlayerController player;
    private PlayerMovement playerMove;
    public Collider2D beak;
    public bool bossCantDamage = false;   //this is the flag that prevents the boss from taking damage. boss will not lose health while this is true
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(health <= 0)
            isDead = true;

        //if (health <= 25) {
          //  anim.SetTrigger("stageTwo");
        //}

        //if (health <= 0) {
        //    anim.SetTrigger("death");
        //}

        

        healthBar.value = health;
    }
    private void FixedUpdate()
    {
        if(attackTrigger)
            beak.enabled = true;
        else
            beak.enabled = false;
    }


    //not sure if this should be on collision or on trigger
    private void OnTriggerEnter2D(Collider2D col)
    {
            //collisionFlag = true;
            
            // deal the player damage ! 
            if (col.gameObject.CompareTag("Player") && isDead == false && StateManager.instance.cantDamage == false) {
                //Debug.Log("Player has taken Damage: " + damage);
                player.PlayerDamage(damage, transform.position);
                //if (timeBtwDamage <= 0) {
                    //camAnim.SetTrigger("shake");
                    //send player flying back
                    
                //}
            } 
            else if(col.gameObject.CompareTag("Weapon") && isDead == false && bossCantDamage == false)
            {
                Debug.Log("Boss has taken Damage: " + PlayerAttack.strength);
                health -= PlayerAttack.strength;
            }
            else if(col.gameObject.CompareTag("DownWeapon") && isDead == false && bossCantDamage == false)
            {
                //Debug.Log("Boss has taken Damage: " + player.strength);
                health -= PlayerAttack.strength;
                //Debug.Log("Initiate pogo");
                playerMove.ConstantJump();
            }
            StartCoroutine(CollisionTimer());
            //collisionFlag = false;
    }

    public void BossDamage(int damage)
    {
        if(!bossCantDamage)
            health -= damage;
    }

    public IEnumerator CollisionTimer()
    {
        float copy = timeBtwCollision;
        bossCantDamage = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        bossCantDamage = false;
    }
}