using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{  
    private PlayerController player;
    private Boss boss;

    public GameObject GameOverPanel;
    public GameObject VictoryPanel;

    private void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();    
    }
    private void Update()
    {
        //runs the game over function when the player has died, regardless of whether or not the boss has died
        if(StateManager.instance.playerState == StateManager.PlayerStates.DEAD)
        {
            GameOver();
        }
        else if(boss.isDead & StateManager.instance.playerState != StateManager.PlayerStates.DEAD)
        {
            Victory();
        }
    }

    private void GameOver()
    {
        //KillPlayer(player);
        GameOverPanel.SetActive(true);
        Time.timeScale = 0;
    } 
    private void Victory()
    {
        //KillBoss(boss);
        VictoryPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public static void KillPlayer(PlayerController player)
    {
        //Destroy(player.gameObject);
        //trigger some kind of animation
        //end the game here
    }
    public static void KillBoss(Boss boss)
    {
        //Destroy(boss.gameObject);
        //play some kind of animation here
        //transition to some kind of win screen now
    }
}
