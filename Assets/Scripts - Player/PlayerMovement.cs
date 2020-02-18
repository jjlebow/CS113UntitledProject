using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private PlayerController controller;
	[HideInInspector]public float horizontal = 0f;
    public float runningSpeed = 20f;
    private float timeBtwJump = 0.05f;  
    public bool cantJump = false;

    //determines how many jumps the player has
    public int extraJumps;
    public float jumpTime;
    private float jumpTimeCounter;
    private int availJumps;

    private Vector3 m_velocity = Vector3.zero;
    [Range(0, 0.3f)][SerializeField] private float movementSmoothing = 0.05f;
    
    [SerializeField] public float jumpForce;

    private Vector2 direction = new Vector2(0,-1);
    //this adjusts the length for the raycast that recognizes if grounded or not
    private float raycastMaxDistance = 0.32f; //this needs to change if you change the size of the player
    public float fallMultiplier;
    public float lowJumpMultiplier;

    public float dashDuration;
    public float dashCooldown;

    private bool canDash = true;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<PlayerController>();
        availJumps = extraJumps;
        
    }

    void FixedUpdate()
    {
        if(!StateManager.instance.playerGrounded)
        {
            AerialPhysics();
        }
        RaycastCheckUpdateGround();
        if(StateManager.instance.knockback)
            transform.position = Vector3.Lerp(transform.position, controller.knockbackDir, Time.deltaTime * 0.5f);
        else if(StateManager.instance.playerState != StateManager.PlayerStates.HOLD)//StateManager.instance.knockback == false)//
        {
            horizontal = Input.GetAxisRaw("Horizontal") * runningSpeed;
            if(Input.GetButtonDown("Jump") && !cantJump)
            {
                StateManager.instance.isJumping = true;
                StateManager.instance.jump = true;
            }
            else if(Input.GetButtonUp("Jump") && !cantJump)
            {
                StartCoroutine(JumpTimer());
                StateManager.instance.isJumping = false;
                //Debug.Log("here");
                //jump = false;
            }

            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StateManager.instance.crouch = true;
            } 
            else if(Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.DownArrow))
            {
                StateManager.instance.crouch = false;
            }


            Move(horizontal * Time.fixedDeltaTime);
            StateManager.instance.jump = false;
        }
    }

//try to find a better solution to the problem of spamming spacebar makes the player float a bit
    private IEnumerator JumpTimer()
    {
        float copy = timeBtwJump;
        cantJump = true;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        cantJump = false;
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
            if(StateManager.instance.playerGrounded || controller.airControl)
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
                Vector3 targetVelocity = new Vector2(move * 10f, controller.m_Rigidbody2D.velocity.y);
                //this is for smoothing out movement
                controller.m_Rigidbody2D.velocity = Vector3.SmoothDamp(controller.m_Rigidbody2D.velocity, targetVelocity, ref m_velocity, movementSmoothing);

                //if the input is moving the player to the right and the player is facing left
                if(move > 0 && !StateManager.instance.faceRight)
                {
                    Flip();
                    //StateManager.instance.faceRight = true;
                }
                else if(move < 0 && StateManager.instance.faceRight)
                {
                    Flip();
                    //StateManager.instance.faceRight = false;
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
                controller.m_Rigidbody2D.velocity = new Vector3(controller.m_Rigidbody2D.velocity.x,0,0);
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

    public void Jump(float jForce)
    {
        controller.m_Rigidbody2D.AddForce(new Vector2(0f, jForce));
    }

    public void jumpReset()
    {
        availJumps = extraJumps;
    }

    //this is for when the Jump function is only called once(the *11 is to make up for the fact that its only called once)
    public void ConstantJump()
    {
        controller.m_Rigidbody2D.velocity = new Vector3(controller.m_Rigidbody2D.velocity.x,0,0);
        controller.m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce * 11));    //more force to the double jump to counterbalance the negative velocity
    }

    private void Flip()
    {
        if(!StateManager.instance.isAttacking && !StateManager.instance.stance)
        {
            //switches the way the player is facing
            StateManager.instance.faceRight = !StateManager.instance.faceRight;
            //multiplies the players x local scale by -1
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    //Adjusts player phsyics for use any time they are airborne
    private void AerialPhysics()
    {
        if(controller.m_Rigidbody2D.velocity.y < 0)
        {
            controller.m_Rigidbody2D.velocity += (Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        else if(controller.m_Rigidbody2D.velocity.y > 0 && !Input.GetButtonDown("Jump"))
        {
            controller.m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
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
        if(hit.collider && StateManager.instance.playerGrounded == false)
        {
            StateManager.instance.playerGrounded = true;
            controller.TriggerLandEvent();
        }
        else if(!hit.collider)
        {
            StateManager.instance.playerGrounded = false;
        }
    }

    public void BackDash()
    {
        if(canDash)
        {
            if(!StateManager.instance.playerGrounded)
                canDash = false;
            //controller.m_Rigidbody2D.gravityScale = 0.0f;
            if(StateManager.instance.faceRight)
                controller.m_Rigidbody2D.velocity = new Vector3(-5,0,0);
            else
                controller.m_Rigidbody2D.velocity = new Vector3(5,0,0);
            StartCoroutine(DashDuration());
        }
    }

    public void ForwardDash()
    {
        if(canDash)
        {
            canDash = false;
            controller.m_Rigidbody2D.gravityScale = 0.0f;
            if(StateManager.instance.faceRight)
                controller.m_Rigidbody2D.velocity = new Vector3(7,0,0);
            else
                controller.m_Rigidbody2D.velocity = new Vector3(-7,0,0);
            StartCoroutine(DashDuration());
        }
    }

    public void DashReset()
    {
        //canDash = true;
    }
    private IEnumerator DashDuration()
    {
        float copy = dashDuration;
        canDash = false;
        StateManager.instance.playerState = StateManager.PlayerStates.HOLD;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
        if(StateManager.instance.stance == false)
            controller.m_Rigidbody2D.gravityScale = 1.0f;
        copy = dashCooldown - dashDuration;
        while(copy > 0)
        {
            copy -= Time.deltaTime;
            yield return null;
        }
        canDash = true;
    }
    
}
