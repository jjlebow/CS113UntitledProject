using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{  
    private PlayerController player;
    private Boss boss;

    private void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();    
    }
    private void Update()
    {
        //runs the game over function when the player has died, regardless of whether or not the boss has died
        if(player.isDead)
        {
            GameOver();
        }
        else if(boss.isDead & !player.isDead)
        {
            Victory();
        }
    }

    private void GameOver()
    {
        Debug.Log("YOU ARE DEAD");
        //bring up UI element telling player they died
    } 
    private void Victory()
    {
        Debug.Log("YOU WIN");
        //bring up a UI element telling player theyve won
    }
    public static void KillPlayer(PlayerController player)
    {
        Destroy(player.gameObject);
        //trigger some kind of animation
        //end the game here
    }
    public static void KillBoss(Boss boss)
    {
        Destroy(boss.gameObject);
        //play some kind of animation here
        //transition to some kind of win screen now
    }
}
