using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class Boss
{
    [HideInInspector] public bool attackTrigger = false;
    public int health;
    public int damage;
    [HideInInspector] public float timeBtwCollision = 0.1f;  //this is to prevent multiple hitboxes hitting each other at once. basically boss invuln frames between damage
    public Slider healthBar;
    


    //public Animator camAnim;
    [HideInInspector] public bool isDead = false;
    //public Collider2D beak;
    [HideInInspector] public bool bossCantDamage = false;   //this is the flag that prevents the boss from taking damage. boss will not lose health while this is true

    
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
