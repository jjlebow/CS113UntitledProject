using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //initialized private variables that can be edited in the editor
    [SerializeField] private float jumpForce;
    [Range(0,1)] [SerializeField] private float crouchSpeed = .36f;
    [Range(0, 0.3f)][SerializeField] private float movementSmoothing = 0.05f;
    [SerializeField] private bool airControl = false;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Collider2D crouchDisableCollider;

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
    [HideInInspector] public bool CR_Running;
    public Collider2D attackTriggerNeutral;
    public Collider2D attackTriggerDown;
    public Collider2D attackTriggerUp;
    private IEnumerator attacking;
    private float attackCooldown = 1.0f;
    private float attackTimer = 0.0f;
    
    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> {}

    public BoolEvent OnCrouchEvent;
    private bool wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        attackTriggerNeutral.enabled = false;
        attackTriggerUp.enabled = false;
        attackTriggerDown.enabled = false;
        if(OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if(OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for(int i = 0; i < colliders.Length; ++i)
        {
            if(colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if(!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
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
                    OnCrouchEvent.Invoke(true);
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
                    OnCrouchEvent.Invoke(false);
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
        if(grounded && jump)
        {
            grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void Flip()
    {
        //switches the way the player is facing
        facingRight = !facingRight;

        //multiplies the players x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }

    public void Attack(string s)
    {
        if(s == "UP")
        {
            attackTriggerUp.enabled = true;
        }
        else if(s == "DOWN")
        {
            attackTriggerDown.enabled = true;
        }
        else
        {
            attackTriggerNeutral.enabled = true;
        }
        attacking = attackTime();
        StartCoroutine(attacking);
        
    }

    public IEnumerator attackTime()
    {
        CR_Running = true;
        attackTimer = attackCooldown;
        //this is the amount of time that the weapon hitbox is active
        while(attackTimer > 0.7f)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        //the weapon hitbox deactivates and there is a cooldown before
        //the player is able to use it again
        attackTriggerNeutral.enabled = false;
        attackTriggerUp.enabled = false;
        attackTriggerDown.enabled = false;
        while(attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            yield return null;
        }
        CR_Running = false;
        
    }

}
