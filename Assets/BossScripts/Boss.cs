using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    [HideInInspector] public bool attackTrigger = false;
    public int health;
    public int damage;
    private float timeBtwDamage = 1.5f;
    


    //public Animator camAnim;
    public Slider healthBar;
    private Animator anim;
    [HideInInspector] public bool isDead;
    public PlayerController player;
    public Collider2D beak;

    private void Start()
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

        // give the player some time to recover before taking more damage !
        if (timeBtwDamage > 0) {
            timeBtwDamage -= Time.deltaTime;
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
        // deal the player damage ! 
        if (col.gameObject.CompareTag("Player") && isDead == false) {
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
    }

    public void BossDamage(int damage)
    {
        health -= damage;
    }
}