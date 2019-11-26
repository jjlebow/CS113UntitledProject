using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    public int health;
    public int damage;
    private float timeBtwDamage = 1.5f;


    //public Animator camAnim;
    public Slider healthBar;
    private Animator anim;
    public bool isDead;
    public PlayerController player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

    private void Update()
    {

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

    //not sure if this should be on collision or on trigger
    private void OnTriggerEnter2D(Collider2D col)
    {
        // deal the player damage ! 
        if (col.gameObject.CompareTag("Player") && isDead == false) {
            Debug.Log("Here");
            //if (timeBtwDamage <= 0) {
                //camAnim.SetTrigger("shake");
                player.health -= damage;
                //send player flying back
                StartCoroutine(player.Knockback(0.02f, 250 , player.transform.position));
            //}
        } 
    }
}