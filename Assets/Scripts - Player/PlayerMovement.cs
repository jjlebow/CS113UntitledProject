using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private PlayerController controller;
	float horizontal = 0f;
    public float runningSpeed = 20f;
    public bool isJumping = false;
    bool jump = false;
    bool crouch = false;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * runningSpeed;
        if(Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            jump = true;
        }
        else if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            //Debug.Log("here");
            //jump = false;
        }

        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
        	crouch = true;
        } 
        else if(Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.DownArrow))
        {
        	crouch = false;
        }

    }

    void FixedUpdate()
    {
    		controller.Move(horizontal * Time.fixedDeltaTime, crouch, jump, isJumping);
    		jump = false;
    }
}
