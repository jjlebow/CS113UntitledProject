using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private PlayerController controller;
	float horizontal = 0f;
    public float runningSpeed = 20f;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(StateManager.instance.playerState != StateManager.PlayerStates.KNOCKBACK)
        {
            if(horizontal != 0 && StateManager.instance.playerGrounded)
                StateManager.instance.playerState = StateManager.PlayerStates.MOVING;
            else if(horizontal == 0 && StateManager.instance.playerGrounded)
                StateManager.instance.playerState = StateManager.PlayerStates.IDLE;
        }
    }

    void FixedUpdate()
    {
        if(StateManager.instance.playerState != StateManager.PlayerStates.KNOCKBACK)
        {
            horizontal = Input.GetAxisRaw("Horizontal") * runningSpeed;
            if(Input.GetButtonDown("Jump"))
            {
                StateManager.instance.isJumping = true;
                StateManager.instance.jump = true;
            }
            else if(Input.GetButtonUp("Jump"))
            {
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


            controller.Move(horizontal * Time.fixedDeltaTime);
            StateManager.instance.jump = false;
        }
    }
}
