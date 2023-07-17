using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerState 
{
    public bool IsCollidingRight;
    public bool IsCollidingLeft;
    public bool IsCollidingAbove;
    public bool IsCollidingBelow;
    public bool HasCollisions {  get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }
    public bool IsGrounded { get { return IsCollidingBelow; } }
    public bool IsFalling;
    public bool IsJumping;
    public bool WasGroundedLastFrame;
    public bool WasTouchingTheCeilingLastFrame;
    public bool JustGotGrounded;

    public void Reset()
    {
        IsCollidingLeft = false;
        IsCollidingRight = false;
        IsCollidingAbove = false;
        IsCollidingBelow = false;
        JustGotGrounded = false;
        IsFalling = true;
    }
}
