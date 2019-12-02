using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class PlayerController : MonoBehaviour
{
    Animator anim;

    //initialized private variables that can be edited in the editor
    [SerializeField] public float jumpForce;
    [Range(0,1)] [SerializeField] private float crouchSpeed = .36f;
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

    //tells us whether or not the player is grounded
    [HideInInspector] public bool grounded;
    //Radius of the circle that determines if we are grounded or not
    const float groundedRadius = 0.2f;
    //radius of the circle that determinds if the player is touching a ceiling or not
    const float ceilingRadius = 0.2f;
    private Rigidbody2D m_Rigidbody2D;
    //determines whether the player is facing right or not
    private bool facingRight = true;
    private Vector3 m_velocity = Vector3.zero;

    //variables for player attacks
    [HideInInspector] public bool CR_Running = false;
    [HideInInspector] public bool isAttacking = false;
    public GameObject attackTriggerNeutral;
    public GameObject attackTriggerDown;
    public GameObject attackTriggerUp;
    private IEnumerator attacking;
    public float attackCooldown;
    public float attackActiveTime;
    private float attackTimer;
    public int strength;

    //this adjusts the length for the raycast that recognizes if grounded or not
    private float raycastMaxDistance = 1.15f;
    private Vector2 direction = new Vector2(0,-1);

    public float fallMultiplier;
    public float lowJumpMultiplier;
    private bool isFalling;

    private float timeBtwDamage = 1.5f; //this is the cooldown between which the player can take damage
    public bool cantDamage = false;
    
    public int health;
    [HideInInspector] public bool isDead;




    private bool wasCrouching = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        availJumps = extraJumps;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        attackTriggerNeutral.SetActive(false);
        attackTriggerUp.SetActive(false);
        attackTriggerDown.SetActive(false);
        landedEvent += attackCancel;
        landedEvent += jumpReset;
        isDead = false;
        
    }

    private void FixedUpdate()
    {
        if(!grounded)
        {
            AerialPhysics();
        }
        RaycastCheckUpdateGround();
    }
    private void Update()
    {
        if(health <= 0)
        {
            isDead = true;
        }
        anim.SetBool("Attacking", isAttacking);
    }


    public void Move(float move, bool crouch, bool jump, bool isJumping)
    {
        //if(!CR_Running)
        //{
            if(!crouch)
            {
                if(Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
                {
                    crouch = true;
                }
            }

            //can only control the player if grounded or airControl is on
            if(grounded || airControl)
            {
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
                }

                //finding target velocity to move the player
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                //this is for smoothing out movement
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_velocity, movementSmoothing);

                //if the input is moving the player to the right and the player is facing left
                if(move > 0 && !facingRight)
                {
                    Flip();
                }
                else if(move < 0 && facingRight)
                {
                    Flip();
                }
            }
            //This is called when the player jumps and they are grounded
            if(grounded && jump)
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;
                Jump(jumpForce);
            }
            //this is called when the player is not grounded but still has 
            //available jumps(double jump)
            
            else if(!grounded && jump && availJumps > 0)
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;        
                m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x,0,0);
                Jump((float)(jumpForce * 2));    //more force to the double jump to counterbalance the negative velocity
                --availJumps;
            }
            //This is when the jump button is being held down
            if(isJumping)
            {
                //This confirms that the timer for the jump does not exceed
                if(jumpTimeCounter > 0)
                {
                    Jump(jumpForce);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                    isJumping = false;
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
        if(!isAttacking)
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
        if(hit.collider && grounded == false)
        {
            grounded = true;
            if(landedEvent != null)
                landedEvent();
        }
        else if(!hit.collider)
        {
            grounded = false;
        }
    }


    private void jumpReset()
    {
        availJumps = extraJumps;
    }


    public IEnumerator attackTime()
    {
        CR_Running = true;
        isAttacking = true;
        attackTimer = attackCooldown + attackActiveTime;
        //this is the amount of time that the weapon hitbox is active (attackCooldown - this time)
        while(attackTimer > attackCooldown)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        //the weapon hitbox deactivates and there is a cooldown before
        //the player is able to use it again. the remaining time(above is the actual Cooldown before the next strike)
        isAttacking = false;
        attackTriggerNeutral.SetActive(false);
        attackTriggerUp.SetActive(false);
        attackTriggerDown.SetActive(false);
        while(attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        CR_Running = false;
    }

    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir)
    {
        float timer = 0;
        while(knockDur > timer)
        {
            timer += Time.deltaTime;
            m_Rigidbody2D.AddForce(new Vector3(knockbackDir.x * knockbackPwr, knockbackDir.y * knockbackPwr, transform.position.z));
        }
        yield return 0;
    }

    public void Attack(string s)
    {
        if(s == "UP")
        {
            attackTriggerUp.SetActive(true);
        }
        else if(s == "DOWN")
        {
            attackTriggerDown.SetActive(true);
            //if()
            //{
              //  MidairJump();
            //}
        }
        else
        {
            attackTriggerNeutral.SetActive(true);
        }
        attacking = attackTime();
        StartCoroutine(attacking);
    }

    private void attackCancel()
    {
        if(CR_Running)
        {
            Debug.Log("FJDLSJFKDSF");
            CR_Running = false;
            isAttacking = false;
            attackTriggerNeutral.SetActive(false);
            attackTriggerUp.SetActive(false);
            attackTriggerDown.SetActive(false);
            StopCoroutine(attacking);
        }
    }

    //this is for when the Jump function is only called once(the *11 is to make up for the fact that its only called once)
    public void ConstantJump()
    {
        m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x,0,0);
        m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce * 11));    //more force to the double jump to counterbalance the negative velocity
    }


    public void PlayerDamage(int damage)
    {
        StartCoroutine(Knockback(0.02f, 250 , transform.position));
        StartCoroutine(DamageTimer());
        health -= damage;
    }

    public IEnumerator DamageTimer()
    {
        float copy = timeBtwDamage;
        cantDamage = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        cantDamage = false;
    }
}





    


//still need to add collisions, hit boxes, and a jump when you get the
//down hit box. should reset the velocity to 0 as well and do a jump
//that isnt dependant on the duration of holding the space key


