using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    [HideInInspector] public bool attackTrigger = false;
    public int health;
    public int damage;
    private float timeBtwCollision = 0.1f;  //this is to prevent multiple hitboxes hitting each other at once
    


    //public Animator camAnim;
    public Slider healthBar;
    private Animator anim;
    [HideInInspector] public bool isDead;
    public PlayerController player;
    public Collider2D beak;
    public bool collisionFlag = false;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(health <= 0)
            isDead = true;

        if (health <= 25) {
            anim.SetTrigger("stageTwo");
        }

        if (health <= 0) {
            anim.SetTrigger("death");
        }

        

        //healthBar.value = health;
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
        if(!collisionFlag)
        {
            //collisionFlag = true;
            StartCoroutine(CollisionTimer());
            // deal the player damage ! 
            if (col.gameObject.CompareTag("Player") && isDead == false && player.cantDamage == false) {
                Debug.Log("Player has taken Damage: " + damage);
                player.PlayerDamage(damage);
                //if (timeBtwDamage <= 0) {
                    //camAnim.SetTrigger("shake");
                    //send player flying back
                    
                //}
            } 
            else if(col.gameObject.CompareTag("Weapon") && isDead == false)
            {
                Debug.Log("Boss has taken Damage: " + player.strength);
                BossDamage(player.strength);
            }
            else if(col.gameObject.CompareTag("DownWeapon") && isDead == false)
            {
                Debug.Log("Boss has taken Damage: " + player.strength);
                BossDamage(player.strength);
                Debug.Log("Initiate pogo");
                player.ConstantJump();
            }
            //collisionFlag = false;
        }
    }

    public void BossDamage(int damage)
    {
        health -= damage;
    }

    public IEnumerator CollisionTimer()
    {
        float copy = timeBtwCollision;
        collisionFlag = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        collisionFlag = false;
    }
}