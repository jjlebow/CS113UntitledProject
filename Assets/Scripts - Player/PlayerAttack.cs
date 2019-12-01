using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    //private Boss boss;
    public PlayerController player;


    void Awake()
    {    
        //boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        //this chain of if statements is used to determine which direction the attack is used in. GetKey is used instead so that we can read 
        //multiple inputs at once
        if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Input.GetKey(KeyCode.K) && (player.grounded == false) && !player.CR_Running)
        {
            player.Attack("DOWN");
        }
        else if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Input.GetKey(KeyCode.K) &&!player.CR_Running)
        {
            player.Attack("UP");
        }
        else if(Input.GetKeyDown(KeyCode.K) && !player.CR_Running)
        {
            player.Attack("NEUTRAL");
        }
    }
}


