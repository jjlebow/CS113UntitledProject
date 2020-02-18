using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBoss : MonoBehaviour {

    public Boss boss;
    private Animator anim;
    public PlayerController player;
    public PlayerMovement playerMove;
    public int attackType;  //this represents the attaack type that was used, but will remain in this until a new attack is used allowing the code to see the last attack the boss used as well while determining the next attack
    private Rigidbody2D m_Rigidbody2D;
    public bool enableJump = false;
    private float raycastMaxDistance = 1.50f;
    public bool bossGrounded;
    private Vector2 direction = new Vector2(0,-1);
    public GameObject shortAttackTrigger;
    


    private void Awake()
    {
        anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        boss.healthBar.value = boss.health;
        RaycastCheckUpdateGround();
        if(boss.health <= 0)
        {
            boss.isDead = true;
            anim.SetBool("isDead", true);
        }

        anim.SetBool("Grounded", bossGrounded);
        anim.SetFloat("YVelocity", m_Rigidbody2D.velocity.y);
    }

    private void FixedUpdate()
    {
       
        if(enableJump)
        {
            m_Rigidbody2D.velocity = Vector2.up * 5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
            if (col.gameObject.CompareTag("Player") && boss.isDead == false && StateManager.instance.cantDamage == false) {
                //Debug.Log("Player has taken Damage: " + damage);
                player.PlayerDamage(boss.damage, transform.position);
                //if (timeBtwDamage <= 0) {
                    //camAnim.SetTrigger("shake");
                    //send player flying back
                //}
            } 
            else if(col.gameObject.CompareTag("Weapon") && boss.isDead == false && boss.bossCantDamage == false)
            {
                Debug.Log("Boss has taken Damage: " + PlayerAttack.strength);
                boss.health -= PlayerAttack.strength;
            }
            else if(col.gameObject.CompareTag("DownWeapon") && boss.isDead == false && boss.bossCantDamage == false)
            {
                //Debug.Log("Boss has taken Damage: " + player.strength);
                boss.health -= PlayerAttack.strength;
                //Debug.Log("Initiate pogo");
                playerMove.ConstantJump();
            }
            StartCoroutine(boss.CollisionTimer());
            //collisionFlag = false;
    }

    //this funtion is called every time the boss is idle and looking for the next attack to use
    //try to add some type of randomness beyond this to the attack patterns though
    public void DetermineAttack()
    {
        float distX = Mathf.Abs(transform.position.x - player.transform.position.x);
        float distY = Mathf.Abs(transform.position.y - player.transform.position.y);
        if(distX < 2 && distY > 1)
        {
            //Debug.Log("JUMP ATTACK");
            attackType = 1;
            anim.SetInteger("AttackType", 1);
            //choose long range attack  //choosing an attack just means
            //changing the value of a variable that connects to the animator
            //so that the animator knows which attack to go to and the 
            //scripts in the animator will handle the actual hitboxes and 
            //attack animations
        }
        //make a condition
        else if(distX <= 5)
        {
            //Debug.Log("Short Range Attack");
            attackType = 3;
            anim.SetInteger("AttackType", 3);
            //short range attack
        }
        else if(distX > 5 && distY <= 2)
        {
            //Debug.Log("Spike Attack");
            attackType = 2;
            anim.SetInteger("AttackType", 2);
            //make the boss jump to counter the player from bouncing off
        }

        else if(distX > 5 && distY > 2)
        {
            attackType = 4;
            anim.SetInteger("AttackType", 4);
        }
    }

    public void Jump()
    {
        Debug.Log("Jump");
        enableJump = true;

    }

    public void SpikeAttack()
    {
        Debug.Log("Spike Attack");
    }

    public void ShortAttack()
    {
        Debug.Log("Short Attack");
        shortAttackTrigger.SetActive(true);
    }

    public void MeteorAttack()
    {
        Debug.Log("MeteorAttack");
    }

    //this is for creating the raycast given a direction, and returning that raycast
    private RaycastHit2D CheckRaycastGround(Vector2 direction)
    {
        Vector2 startingPosition = new Vector2(transform.position.x, transform.position.y);// + directionOriginOffset);
        //Debug.DrawRay(startingPosition,direction, Color.red, raycastMaxDistance); //this is not accurate
        //the final argument is a layer mask telling the raycast to only pay attention to layer 10 (ground)
        
        return Physics2D.Raycast(startingPosition, direction, raycastMaxDistance, 1 << 10);
    }

    //This function is called in fixed update and constantly checks to see if there is ground
    private void RaycastCheckUpdateGround()
    {
        RaycastHit2D hit = CheckRaycastGround(direction);
        //Debug.DrawRay(transform.position, direction, Color.red);
        if(hit.collider && bossGrounded == false)
        {
            bossGrounded = true;
        }
        else if(!hit.collider)
        {
            bossGrounded = false;
        }
    }
}