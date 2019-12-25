using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    //initialized private variables that can be edited in the editor
    //[Range(0,1)] [SerializeField] private float crouchSpeed = .36f;
    
    public bool airControl = false;
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform ceilingCheck;
    [SerializeField] public Collider2D crouchDisableCollider;

    public delegate void LandedDelegate();
    public event LandedDelegate landedEvent;

    

    //Radius of the circle that determines if we are grounded or not
    const float groundedRadius = 0.2f;
    //radius of the circle that determinds if the player is touching a ceiling or not
    const float ceilingRadius = 0.2f;
    [HideInInspector] public Rigidbody2D m_Rigidbody2D;
    //determines whether the player is facing right or not
    
    

    

    
    


    //private bool isFalling;

    private float knockbackDuration = 0.4f; //how long player is knocked back for
    private float timeBtwDamage = 1f; //this is the cooldown between which the player can take damage
    
    
    public int health;
    public Slider healthBar;

    private PlayerAttack playerAttacker;
    private PlayerMovement playerMove;

    public Boss boss;

    //private Vector3 knockbackDirRight;
    //private Vector3 knockbackDirLeft;
    [HideInInspector]public Vector3 knockbackDir;

    


    //private bool wasCrouching = false;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttack>();
        playerMove = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        
        landedEvent += playerAttacker.attackCancel;
        landedEvent += playerMove.jumpReset;
        landedEvent += landingAnimation;
        //landedEvent += playerMove.DashReset;
        
        
    }

    private void Update()
    {
        if(health <= 0)
        {
            StateManager.instance.playerState = StateManager.PlayerStates.DEAD;
        }
        healthBar.value = health;

        if(StateManager.instance.playerState != StateManager.PlayerStates.HOLD)
        {
            if(playerMove.horizontal != 0 && StateManager.instance.playerGrounded)
                StateManager.instance.playerState = StateManager.PlayerStates.MOVING;
            else if(playerMove.horizontal == 0 && StateManager.instance.playerGrounded)
                StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
        }

        //this chain of if statements is used to determine which direction the attack is used in. GetKey is used instead so that we can read 
        //multiple inputs at once
        if(Input.GetAxisRaw("Vertical") < 0)
        {
            StateManager.instance.directionalFacing = StateManager.Directional.DOWN;
        }
        else if(Input.GetAxisRaw("Vertical") > 0)
        {
            StateManager.instance.directionalFacing = StateManager.Directional.UP;
        }
        else if(Input.GetAxisRaw("Vertical") == 0)
        {
            StateManager.instance.directionalFacing = StateManager.Directional.NEUTRAL;
        }

        if(!StateManager.instance.inCooldown && Input.GetKey(KeyCode.K) && StateManager.instance.playerState != StateManager.PlayerStates.HOLD)
            playerAttacker.Attack();

        if(Input.GetKey(KeyCode.J) && StateManager.instance.playerState != StateManager.PlayerStates.HOLD)
            playerMove.QuickDash();


        //animation triggers
        if(m_Rigidbody2D.velocity.y <= 0 && !StateManager.instance.playerGrounded)
        {
            //anim.SetBool("airRising", false);
            anim.SetTrigger("airFalling");
        }
        else if(m_Rigidbody2D.velocity.y > 0 && !StateManager.instance.playerGrounded)
        {
            anim.SetTrigger("airRising");
            //anim.SetBool("airFalling", false);
        }
        anim.SetBool("Attacking", StateManager.instance.isAttacking);
        anim.SetBool("attackCooldown", StateManager.instance.inCooldown);
        if(StateManager.instance.playerState == StateManager.PlayerStates.MOVING)
            anim.SetBool("Walking", true);
        else
            anim.SetBool("Walking", false);
    }

    private void landingAnimation()
    {
        //anim.SetBool("airRising", false);
        //anim.SetBool("airFalling", false);
        anim.SetTrigger("Landing");
    }

    public void TriggerLandEvent()
    {
        if(landedEvent != null)
            landedEvent();
    }

    

    

    

    

    

    
    
    /*
    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir)
    {
        float timer = 0;
        while(knockDur > timer)
        {
            //knockbackPwr = knockbackPwr *2;
            timer += Time.deltaTime;
            //m_Rigidbody2D.velocity = new Vector3(knockbackDir.x * knockbackPwr, knockbackDir.y * knockbackPwr, knockbackDir.z * knockbackPwr);
            m_Rigidbody2D.AddForce(new Vector3(knockbackDir.x * knockbackPwr, knockbackDir.y * knockbackPwr, transform.position.z));
        }
        yield return 0;
    }
    */
    /*
    public void Knockback()
    {
        Vector3 knockbackDir;
        if(StateManager.instance.faceRight == true)
            knockbackDir = new Vector3(transform.position.x - 100, transform.position.y + 100, transform.position.z);
        else
            knockbackDir = new Vector3(transform.position.x + 100, transform.position.y + 100, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, knockbackDir, Time.deltaTime * 2);
    }
    */

    

    //this includes the knockback but want to edit it to make the knockback move slower
    public void PlayerDamage(int damage, Vector3 attacker)
    {
        //Vector3 knockbackDir = (m_Rigidbody2D.transform.position - boss.transform.position).normalized;
        //m_Rigidbody2D.velocity = new Vector3(knockbackDir.x * 65, knockbackDir.y * 65, knockbackDir.z * 65);
        //m_Rigidbody2D.AddForce(new Vector3(knockbackDir.x * 5000, knockbackDir.y * 5000, knockbackDir.z * 5000));
        //StartCoroutine(Knockback(5f, 10, knockbackDir));
        //StartCoroutine(Knockback(0.5f));
        //ensures that current velocity does not impact knockback
        m_Rigidbody2D.velocity = new Vector3(0,0,0);
        //determines which direction you will be going in 
        Vector3 oppositeDir = (attacker - m_Rigidbody2D.transform.position).normalized;
        //sets up the angle at which the player will be knocked back, using the direction determined from the above collision data
        if(oppositeDir.x >= 0)
            knockbackDir = new Vector3(transform.position.x - 13, transform.position.y + 15, transform.position.z);
        else
            knockbackDir = new Vector3(transform.position.x + 13, transform.position.y + 15, transform.position.z);
        //the x and y position modifiers are mostly for finding which angle to send the player at during knockback but also affects magnitude
        //knockbackDirRight = new Vector3(transform.position.x - 12, transform.position.y + 10, transform.position.z);
        //knockbackDirLeft = new Vector3(transform.position.x + 12, transform.position.y + 10, transform.position.z);
        StartCoroutine(KnockbackTimer());
        StartCoroutine(DamageTimer());
        health -= damage;
    }

    //determines how long the player will be in the knockback phase
    private IEnumerator KnockbackTimer()
    {
        float copy = knockbackDuration;
        StateManager.instance.playerState = StateManager.PlayerStates.HOLD;
        StateManager.instance.knockback = true;
        //StateManager.instance.cantDamage = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        //StateManager.instance.cantDamage = false;
        StateManager.instance.knockback = false;
        StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
    }
    //determines how long the player has invulnerability between attacks
    private IEnumerator DamageTimer()
    {
        float copy = timeBtwDamage;
        StateManager.instance.cantDamage = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        StateManager.instance.cantDamage = false;
    }
}



