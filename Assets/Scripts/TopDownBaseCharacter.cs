using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Down = 0, Right = 1, Left = 2, Up = 3 }
public class TopDownBaseCharacter : MonoBehaviour
{

    //for this script, we are going to have four different states, for the different directions the character will face
    public FacingDirection currentDirection = 0;

    //for each direction, we will have to activate and deactivate different objects, and control that object's animator
    [Tooltip("0 - Down, 1 - Right, 2 - Left, 3 - Up")]
    public Animator[] animators = new Animator[4];

    //we will use this variable to assign the animator for the activated object
    protected Animator currentAnimator;

    private void Start()
    {
        currentAnimator = animators[(int)currentDirection];
        currentAnimator.enabled = true;
    }

    /// <summary>
    /// Reads a input vector 2 to define where character is facing
    /// </summary>
    /// <param name="movement"></param>
    protected void ManageState(Vector2 movement)
    {
        if (movement.magnitude > 0.05f)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if (movement.x > 0.1f)
                    ChangeState((int)FacingDirection.Right);
                else if (movement.x < -0.1f)
                    ChangeState((int)FacingDirection.Left);
            }
            else
            {
                if (movement.y > 0.1f)
                    ChangeState((int)FacingDirection.Up);
                else if (movement.y < -0.1f)
                    ChangeState((int)FacingDirection.Down);
            }
        }
    }

    /// <summary>
    /// Manually forces a especific state
    /// </summary>
    /// <param name="state">0 - Down, 1 - Right, 2 - Left, 3 - Up</param>
    public void ChangeState(int state)
    {
        if (state != (int)currentDirection)
        {
            currentAnimator.gameObject.SetActive(false);
            currentAnimator.enabled = false;
            currentDirection = (FacingDirection)state;
            currentAnimator = animators[(int)currentDirection];
            currentAnimator.gameObject.SetActive(true);
            currentAnimator.enabled = true;
        }
    }

    /// <summary>
    /// Used to turn the character to a certain position
    /// </summary>
    public void FacePosition(Transform position)
    {
        Vector2 direction = (position.position - transform.position).normalized;
        ManageState(direction);
    }
}
