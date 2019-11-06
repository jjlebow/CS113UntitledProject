using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public CharacterController2D controller;
	float horizon = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizon = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
    		controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
    }
}
