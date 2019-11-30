using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakHitbox : MonoBehaviour
{
    private PlayerController player;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        // deal the player damage ! 
        if (col.gameObject.CompareTag("Player")) {
            Debug.Log("Player has taken Damage from beak: " + damage);
            player.PlayerDamage(damage);
            //if (timeBtwDamage <= 0) {
                //camAnim.SetTrigger("shake");
                //send player flying back
                
            //}
        } 
    }
}
