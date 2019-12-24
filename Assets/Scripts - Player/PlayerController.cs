using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class PlayerController : MonoBehaviour
{
    Animator anim;

    //initialized private variables that can be edited in the editor
    [SerializeField] public float jumpForce;
    //[Range(0,1)] [SerializeField] private float crouchSpeed = .36f;
    [Range(0, 0.3f)][SerializeField] private float movementSmoothing = 0.05f;
    [SerializeField] private bool airControl = false;
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform ceilingCheck;
    [SerializeField] public Collider2D crouchDisableCollider;

    public delegate void LandedDelegate();
    public event LandedDelegate landedEvent;

    //determines how many jumps the player has
    public int extraJumps;
    public float jumpTime;
    private float jumpTimeCounter;
    private int availJumps;

    //Radius of the circle that determines if we are grounded or not
    const float groundedRadius = 0.2f;
    //radius of the circle that determinds if the player is touching a ceiling or not
    const float ceilingRadius = 0.2f;
    private Rigidbody2D m_Rigidbody2D;
    //determines whether the player is facing right or not
    private bool facingRight = true;
    private Vector3 m_velocity = Vector3.zero;

    

    //this adjusts the length for the raycast that recognizes if grounded or not
    private float raycastMaxDistance = 1.15f;
    private Vector2 direction = new Vector2(0,-1);

    public float fallMultiplier;
    public float lowJumpMultiplier;
    private bool isFalling;

    private float knockbackDuration = 0.4f; //how long player is knocked back for
    private float timeBtwDamage = 1f; //this is the cooldown between which the player can take damage
    
    
    public int health;
    public Slider healthBar;

    private PlayerAttack playerAttacker;

    public Boss boss;

    private Vector3 knockbackDirRight;
    private Vector3 knockbackDirLeft;
    private Vector3 knockbackDir;

    


    //private bool wasCrouching = false;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttack>();
        anim = GetComponent<Animator>();
        availJumps = extraJumps;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        
        landedEvent += playerAttacker.attackCancel;
        landedEvent += jumpReset;
        landedEvent += landingAnimation;
        
        
    }

    private void FixedUpdate()
    {
        if(!StateManager.instance.playerGrounded)
        {
            AerialPhysics();
        }
        RaycastCheckUpdateGround();
        if(StateManager.instance.playerState == StateManager.PlayerStates.KNOCKBACK)
            transform.position = Vector3.Lerp(transform.position, knockbackDir, Time.deltaTime * 0.5f);
        /*
        //Lerp(current position, position we are trying to get to, speed of movement(higher number is faster))
        if(StateManager.instance.playerState == StateManager.PlayerStates.KNOCKBACK && StateManager.instance.faceRight)
        {
            //m_Rigidbody2D.velocity = new Vector3(knockbackDirRight.x *2, knockbackDirRight.y * 2, knockbackDirRight.z);
            transform.position = Vector3.Lerp(transform.position, knockbackDirRight, Time.deltaTime * 0.7f);
        }
        else if(StateManager.instance.playerState == StateManager.PlayerStates.KNOCKBACK && !StateManager.instance.faceRight)
        {
            //m_Rigidbody2D.velocity = new Vector3(knockbackDirLeft.x * 2, knockbackDirLeft.y * 2, knockbackDirLeft.z);
            transform.position = Vector3.Lerp(transform.position, knockbackDirLeft, Time.deltaTime * 0.8f);
        }
        */
    }
    private void Update()
    {
        if(health <= 0)
        {
            StateManager.instance.playerState = StateManager.PlayerStates.DEAD;
        }
        healthBar.value = health;


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
        if(StateManager.instance.attackDir == StateManager.AttackDirection.NEUTRAL)
            anim.SetInteger("attackDirection", 1);
        else if(StateManager.instance.attackDir == StateManager.AttackDirection.UP)
            anim.SetInteger("attackDirection", 2);
        else if(StateManager.instance.attackDir == StateManager.AttackDirection.DOWN)
            anim.SetInteger("attackDirection", 3);
        else if(StateManager.instance.attackDir == StateManager.AttackDirection.NOATTACK)
            anim.SetInteger("attackDirection", 0);
    }

    private void landingAnimation()
    {
        //anim.SetBool("airRising", false);
        //anim.SetBool("airFalling", false);
        anim.SetTrigger("Landing");
    }


    public void Move(float move)
    {
        //if(!inCooldown)
        //{
            /*
            if(!crouch)
            {
                if(Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
                {
                    crouch = true;
                }
            }*/

            //can only control the player if grounded or airControl is on
            if(StateManager.instance.playerGrounded || airControl)
            {
                /*
                if(crouch)
                {
                    if(!wasCrouching)
                    {
                        wasCrouching = true;
                        //OnCrouchEvent.Invoke(true);
                    }

                    //Reduce speed my the crouchSpeed multiplier
                    move *= crouchSpeed;

                    if(crouchDisableCollider != null)
                        crouchDisableCollider.enabled = false;
                }
                else
                {
                    if(crouchDisableCollider != null)
                        crouchDisableCollider.enabled = true;

                    if(wasCrouching)
                    {
                        wasCrouching = false;
                        //OnCrouchEvent.Invoke(false);
                    }
                }*/

                //finding target velocity to move the player
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                //this is for smoothing out movement
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_velocity, movementSmoothing);

                //if the input is moving the player to the right and the player is facing left
                if(move > 0 && !facingRight)
                {
                    Flip();
                    StateManager.instance.faceRight = true;
                }
                else if(move < 0 && facingRight)
                {
                    Flip();
                    StateManager.instance.faceRight = false;
                }
            }
            //This is called when the player jumps and they are grounded
            if(StateManager.instance.playerGrounded && StateManager.instance.jump)
            {
                StateManager.instance.isJumping = true;
                jumpTimeCounter = jumpTime;
                Jump(jumpForce);
            }
            //this is called when the player is not grounded but still has 
            //available jumps(double jump)
            
            else if(!StateManager.instance.playerGrounded && StateManager.instance.jump && availJumps > 0)
            {
                StateManager.instance.isJumping = true;
                jumpTimeCounter = jumpTime;        
                m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x,0,0);
                Jump((float)(jumpForce * 2));    //more force to the double jump to counterbalance the negative velocity
                --availJumps;
            }
            //This is when the jump button is being held down
            if(StateManager.instance.isJumping)
            {
                //This confirms that the timer for the jump does not exceed
                if(jumpTimeCounter > 0)
                {
                    Jump(jumpForce);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                    StateManager.instance.isJumping = false;
            }
        //}
    }

    //Adjusts player phsyics for use any time they are airborne
    private void AerialPhysics()
    {
        if(m_Rigidbody2D.velocity.y < 0)
        {
            m_Rigidbody2D.velocity += (Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        else if(m_Rigidbody2D.velocity.y > 0 && !Input.GetButtonDown("Jump"))
        {
            m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Flip()
    {
        if(!StateManager.instance.isAttacking)
        {
            //switches the way the player is facing
            facingRight = !facingRight;
            //multiplies the players x local scale by -1
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public void Jump(float jForce)
    {
        m_Rigidbody2D.AddForce(new Vector2(0f, jForce));
    }

    //this is for creating the raycast given a direction, and returning that raycast
    private RaycastHit2D CheckRaycastGround(Vector2 direction)
    {
        Vector2 startingPosition = new Vector2(transform.position.x, transform.position.y);// + directionOriginOffset);
        //the final argument is a layer mask telling the raycast to only pay attention to layer 10 (ground)
        return Physics2D.Raycast(startingPosition, direction, raycastMaxDistance, 1 << 10);
    }

    //This function is called in fixed update and constantly checks to see if there is ground
    private void RaycastCheckUpdateGround()
    {
        RaycastHit2D hit = CheckRaycastGround(direction);
        //Debug.DrawRay(transform.position, direction, Color.red);
        if(hit.collider && StateManager.instance.playerGrounded == false)
        {
            StateManager.instance.playerGrounded = true;
            if(landedEvent != null)
                landedEvent();
        }
        else if(!hit.collider)
        {
            StateManager.instance.playerGrounded = false;
        }
    }

    private void jumpReset()
    {
        availJumps = extraJumps;
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

    //this is for when the Jump function is only called once(the *11 is to make up for the fact that its only called once)
    public void ConstantJump()
    {
        m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x,0,0);
        m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce * 11));    //more force to the double jump to counterbalance the negative velocity
    }

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
        StateManager.instance.playerState = StateManager.PlayerStates.KNOCKBACK;
        StartCoroutine(KnockbackTimer());
        StartCoroutine(DamageTimer());
        health -= damage;
    }

    //determines how long the player will be in the knockback phase
    private IEnumerator KnockbackTimer()
    {
        float copy = knockbackDuration;
        //StateManager.instance.cantDamage = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        //StateManager.instance.cantDamage = false;
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



