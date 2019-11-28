using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{  
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
