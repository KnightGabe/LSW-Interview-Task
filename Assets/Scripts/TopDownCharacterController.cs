using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterController : TopDownBaseCharacter
{
    //setting up a singleton
    private static TopDownCharacterController instance;
    public static TopDownCharacterController Instance { get { return instance; } }

    //the speed of the character
    public float movementSpeed = 5;

    //this allows for us to stop player input
    public bool canMove = true;

    //we will need the rigidbody to control movement
    public Rigidbody2D rigid;

    //reference to player equipment inventory
    public CharacterWardrobe equipment;

    public int gold;

    private void Awake()
    {
        instance = this;
    }

    protected void FixedUpdate()
    {
        //we will listen for player input and use it for movement, state management and animation, but only if player input is on
        if (canMove)
        {
            Vector2 movAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            ManageState(movAxis);
            MoveCharacter(movAxis);
        }
    }

    protected void MoveCharacter(Vector2 movement)
    {
        rigid.velocity = movement * movementSpeed;
        currentAnimator.SetBool("Moving", movement.magnitude > 0.1f);
    }

    /// <summary>
    /// Activate or Deactivate player input
    /// </summary>
    /// <param name="value">true to activate, false to deactivate</param>
    public void ToggleMovement(bool value)
    {
        if(canMove != value)
        {
            canMove = value;
            //if movement is being deactivated, stop animation and movement
            if (!canMove)
            {
                rigid.velocity = Vector2.zero;
                currentAnimator.SetBool("Moving", false);
            }
        }
    }
}
