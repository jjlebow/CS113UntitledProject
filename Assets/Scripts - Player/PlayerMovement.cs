using System.Collections;
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
        else if(Input.GetKeyUp(KeyCode.S) && Input.GetKeyDown(KeyCode.DownArrow))
        {
        	crouch = false;
        }

        if(Input.GetKeyDown(KeyCode.E) && !controller.CR_Running )
        {
            controller.Attack();

        }
    }

    void FixedUpdate()
    {
    		controller.Move(horizontal * Time.fixedDeltaTime, crouch, jump);
    		jump = false;
    }
}
