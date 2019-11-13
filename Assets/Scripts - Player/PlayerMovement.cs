﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public PlayerController controller;
	float horizontal = 0f;
    public float runningSpeed = 20f;
    bool jump = false;
    bool crouch = false;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * runningSpeed;
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
        	crouch = true;
        } 
        else if(Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.DownArrow))
        {
        	crouch = false;
        }
        //this chain of if statements is used to determine which direction the attack is used in. GetKey is used instead so that we can read 
        //multiple inputs at once
        if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Input.GetKey(KeyCode.E) && (controller.grounded == false) && !controller.CR_Running)
        {
            controller.Attack("DOWN");
        }
        else if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Input.GetKey(KeyCode.E) &&!controller.CR_Running)
        {
            controller.Attack("UP");
        }
        else if(Input.GetKeyDown(KeyCode.E) && !controller.CR_Running)
        {
            controller.Attack("NEUTRAL");
        }
    }

    void FixedUpdate()
    {
    		controller.Move(horizontal * Time.fixedDeltaTime, crouch, jump);
    		jump = false;
    }
}
