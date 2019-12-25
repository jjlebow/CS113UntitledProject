using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    //handles player states that are unique and can not be shared through the use of enums
    public PlayerController player;
    public static StateManager instance = null;

    public bool playerGrounded;
    public bool inCooldown = false;
    public bool isAttacking = false;
    public bool jump = false;
    public bool isJumping = false;
    public bool crouch = false;
    public bool cantDamage = false;
    public bool faceRight = true;
    public bool knockback = false;

    public enum PlayerStates
    {
        IDLE,
        MOVING,
        DEAD,
        HOLD
        //AIRFALLING,
        //AIRRISING
        //BACKWARDS MOVE (Strafe mid combo option)
    }

    public enum Directional
    {
        NEUTRAL,
        DOWN,
        UP
    }
    public PlayerStates playerState;
    public Directional directionalFacing;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        playerState = PlayerStates.IDLE;
    }



}
