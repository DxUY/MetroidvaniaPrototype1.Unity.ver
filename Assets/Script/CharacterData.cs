using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Player")]
public class CharacterData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;

    [Space(3)]
    public float fallMultiplier;
    public float maxFallspeed;

    [Space(3)]
    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
    public float maxFastFallSpeed;

    [Space(20)]

    [Header("Run")]
    public float walkSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelerationAmount;
    [HideInInspector] public float runDeccelerationAmount;
    public float runDecceleration;


    [Space(3)]
    [Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0f, 1)] public float deccelInAir;
    
    [Space(3)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight; //the jump height of the player
    public float timeToApex; //the time between applying the jump force and reaching the apex of the jump 
    [HideInInspector] public float jumpForce; //the actual jump force of the player 

    [Space(20)]

    [Header("Hang Air Time")]
    public float jumpCutGravityMultiply; //Multiplier to increase gravity if the player release the jump button when still jumping
    [Range(0f, 1f)]public float jumpHangGravityMultiplier; //reduce gravity while nearing the apex of the jump
    [Range(0f, 1f)] public float jumpHangThresHold; //Speed (close to 0) where the character will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Space(20)]

    [Header("Wall Jump")]
    public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.

    [Space(20)]

    [Header("Slide")]
    public float slideSpeed;
    public float slideAccel;
    
    [Space(20)]
    [Header("Coyote Time & Jump Buffer")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    public void OnValidate()
    {
        //calculate the gravity using kinematic equation of motion g = -2h/t^2
        gravityStrength = -(2 * jumpHeight) / (timeToApex * timeToApex);

        // Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings / Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelerationAmount = (50 * runAcceleration) / walkSpeed;
        runDeccelerationAmount = (50 * runDecceleration) / walkSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * timeToApex;
    }

}
