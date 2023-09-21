using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using UnityEngine.PlayerLoop;
using System.Runtime.ConstrainedExecution;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterData data;

    Rigidbody2D rb2d;
    Animator animator;
    TouchingDirections touchingDirection;
    DamageCalculation damageCalculation;

    Vector2 moveInput;
    bool _isMoving;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationsHash.isMovingHash, value);
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationsHash.canMoveHash); 
        }
    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set 
        { 
            if (_isFacingRight != value)
                //flip the local scale to make the character face the other direction
                transform.localScale *= new Vector2(-1, 1);

            _isFacingRight = value;
        }
    }

    bool _isSliding;
    public bool IsSliding 
    { 
        get
        {
            return _isSliding;
        } 
        private set
        {
            _isSliding = value;
            animator.SetBool(AnimationsHash.isOnWallHash, value);
        }
    }

    public bool IsWallJumping { get; private set; }

    float Friction = .7f;

    //Jump
    public bool IsJumping { get; private set; }
    bool _isSomersault;
    public bool IsSomerSault
    {
        get
        {
            return _isSomersault;
        }
        private set
        {
            _isSomersault = value;
            animator.SetBool(AnimationsHash.isSomersaultHash, value);
        }
    }

    //Timer
    public float LastOnGroundTime { get; private set; }
    public float LastPressJumpTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastWallTouchTime { get; private set; }

    //Jump
    public bool IsJumpCut { get; private set; }
    public bool IsJumpFalling { get; private set; }
    public bool isInteractPressed { get; private set; }

    //Wall Jump
    private float _wallJumpStartTime;

    //Walking Sfx
    bool wasGroundedAndMoving;
    [SerializeField] bool isGroundedAndMoving;
    [SerializeField] bool isWalkingSoundPlaying;

    public static PlayerMovement instance;
    public static PlayerMovement GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirections>();
        damageCalculation = GetComponent<DamageCalculation>();
    }

    private void Start()
    {
        SetGravityScale(data.gravityScale);
    }
    private void Update()
    {
        #region Timer
        LastOnGroundTime -= Time.deltaTime;
        LastPressJumpTime -= Time.deltaTime;
        LastWallTouchTime -= Time.deltaTime;
        #endregion

        #region CollisionCheck
        if(!IsJumping)
        {
            if (touchingDirection.IsGrounded)
                LastOnGroundTime = data.coyoteTime;

            if (touchingDirection.IsOnWall)
                LastOnWallTime = data.coyoteTime;

            LastWallTouchTime = LastOnWallTime;
        }
        #endregion

        #region Jump Check
        if (IsJumping && rb2d.velocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                IsJumpFalling = true;

        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > data.wallJumpTime)
            IsWallJumping = false;

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            IsJumpCut = false;

            if (!IsJumping)
                IsJumpFalling = false;
        }
        #endregion

        #region Slide Check
        if (touchingDirection.IsOnWall && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0 && IsMoving)
            IsSliding = true;
        else
            IsSliding = false;
            

        #endregion

        #region GRAVITY
        //Higher gravity if we've released the jump input or are falling
        if (IsSliding)
            SetGravityScale(0);
        
        else if (rb2d.velocity.y < 0 && moveInput.y < 0)
        {
            //Much higher gravity if holding down
            SetGravityScale(data.gravityScale * data.fastFallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max(rb2d.velocity.y, -data.maxFastFallSpeed));
        }
        else if (IsJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(data.gravityScale * data.jumpCutGravityMultiply);
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max(rb2d.velocity.y, -data.maxFallspeed));
        }
        else if ((IsJumping || IsWallJumping || IsJumpFalling) && Mathf.Abs(rb2d.velocity.y) < data.jumpHangThresHold)
        {
            SetGravityScale(data.gravityScale * data.jumpHangGravityMultiplier);
        }
        else if (rb2d.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(data.gravityScale * data.fastFallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max(rb2d.velocity.y, -data.maxFallspeed));
        }
        else
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(data.gravityScale);
        #endregion

        if (!touchingDirection.IsGrounded && IsMoving && IsJumping)
            IsSomerSault = true;
        else
            IsSomerSault = false;

        #region Walk Sfx
        bool isGroundedAndMoving = touchingDirection.IsGrounded && IsMoving;

        if (isGroundedAndMoving && !wasGroundedAndMoving)
        {
            PlayWalkSound();
        }
        else if (!isGroundedAndMoving && wasGroundedAndMoving)
        {
            StopWalkSound();
        }

        wasGroundedAndMoving = isGroundedAndMoving;
        #endregion
    }

    public void SetGravityScale(float scale)
    {
        rb2d.gravityScale = scale;
    }

    private void PlayWalkSound()
    {
        if (!isWalkingSoundPlaying && damageCalculation.Alive)
        {
            isWalkingSoundPlaying = true;
            FindObjectOfType<AudioManager>().Play("Walk");
        }
    }
    private void StopWalkSound()
    {
        if (isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = false;
            FindObjectOfType<AudioManager>().Stop("Walk");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (damageCalculation.Alive && !DialogManager.GetInstance().isDialogPlaying)
        {
            IsMoving = moveInput.x != 0f;

            Flip(moveInput);
        }

        if (IsMoving)
        {
            StartCoroutine(CheckDialogFinished(moveInput));
        }
    }

    IEnumerator CheckDialogFinished(Vector2 moveInput)
    {
        // Wait until the dialog is finished playing.
        yield return new WaitUntil(() => !DialogManager.GetInstance().isDialogPlaying);

        // Set the IsMoving to true and start the player moving in their desired direction.
        IsMoving = true;
        damageCalculation.lockVelocity = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            LastPressJumpTime = data.jumpInputBufferTime; // Initialize jump buffer timer
        }

        if (LastOnGroundTime > 0 && LastPressJumpTime > 0 && !IsJumping)
        {
            IsJumping = true;
            IsWallJumping = false;
            IsJumpCut = false;
            IsJumpFalling = false;
            Jump();
        }

        if(context.canceled && IsJumping && rb2d.velocity.y > 0)
        {
            IsJumpCut = true;
        }

        if(CanWallJump() && LastPressJumpTime > 0f)
        {
            IsJumping = false;
            IsWallJumping = true;
            IsJumpCut = false;
            _wallJumpStartTime = Time.time;
            IsJumpFalling = false;
            WallJump();
        }

    }
    private void Jump()
    {
        // Ensures we can't call Jump multiple times from one press
        LastPressJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        animator.SetTrigger(AnimationsHash.isJumpingHash);

        /*We increase the force applied if we are falling
         this means we'll always feel like we jump the same amount 
        (setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)*/
        float force = data.jumpForce;
        if (rb2d.velocity.y < 0)
            force -= rb2d.velocity.y;

        rb2d.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private bool CanWallJump()
    {
        return LastPressJumpTime > 0 && LastWallTouchTime > 0 && LastOnGroundTime <= 0 && !IsWallJumping && !IsJumpFalling;
    }
    private void WallJump()
    {
        LastOnGroundTime = 0;
        LastPressJumpTime = 0;
        LastOnWallTime = 0;

        #region PerForm Wall Jump

        Vector2 force = new Vector2(-data.wallJumpForce.x, data.wallJumpForce.y);
        
        if (Mathf.Sign(force.x) != Mathf.Sign(rb2d.velocity.x))
            force.x -= rb2d.velocity.x;

        if(rb2d.velocity.y < 0)
            force.y -= rb2d.velocity.y;

        rb2d.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    private void Run(float LerpAmount)
    {
        float targetSpeed = moveInput.x * data.walkSpeed;

        targetSpeed = Mathf.Lerp(rb2d.velocity.x, targetSpeed, LerpAmount);

        #region Calculate Acceleration
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelerationAmount : data.runDeccelerationAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.accelInAir * data.runAccelerationAmount : data.deccelInAir * data.runDeccelerationAmount;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || IsJumpFalling) && Mathf.Abs(rb2d.velocity.y) < data.jumpHangThresHold)
        {
            targetSpeed *= data.jumpHangAccelerationMult;
            accelRate *= data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (data.doConserveMomentum && Mathf.Abs(rb2d.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb2d.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve our current momentum
            accelRate = 0;
        }
        #endregion

        // Calculate the desired change in velocity
        float speedDif = targetSpeed - rb2d.velocity.x;

        float movement = speedDif * accelRate;

        // Apply the calculated force to the character
        rb2d.AddForce(Vector2.right * movement, ForceMode2D.Impulse);
    }

    private void Slide()
    {
        //Works the same as the Run but only in the y-axis
        float speedDif = data.slideSpeed - rb2d.velocity.y;
        float movement = speedDif * data.slideAccel;
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb2d.AddForce(movement * Vector2.down);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            if(touchingDirection.IsGrounded)
            {
                animator.SetTrigger(AnimationsHash.isAttackingHash);
            }
            else
            {
                animator.SetTrigger(AnimationsHash.AirAttackTrigger);
            }
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded)
            animator.SetTrigger(AnimationsHash.shootTriggerHash);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started)
            isInteractPressed = true;
        else if(context.canceled)
            isInteractPressed = false;
    }
    public bool GetInteractPressed()
    {
        bool result = isInteractPressed;
        isInteractPressed = false;
        return result;
    }

    private void Flip(Vector2 moveInput)
    {
        if (moveInput.x > 0f && !IsFacingRight)
            IsFacingRight = true;
        else if (moveInput.x < 0f && IsFacingRight)
            IsFacingRight = false;
    }
    private void FixedUpdate()
    {
        if (DialogManager.GetInstance().isDialogPlaying && touchingDirection.IsGrounded)
        {
            damageCalculation.lockVelocity = true;
            rb2d.velocity = Vector2.zero;
            IsMoving = false;
            return;
        }

        //Handle Run
        if (!damageCalculation.lockVelocity)
        {
            if (IsWallJumping)
                Run(data.wallJumpRunLerp);
            else
                Run(1);
        }

        //Handle Friction
        if (touchingDirection.IsGrounded && CanMove && Mathf.Abs(moveInput.x) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb2d.velocity.x), Mathf.Abs(Friction));

            amount *= rb2d.velocity.x;

            rb2d.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        animator.SetFloat(AnimationsHash.YvelocityHash, rb2d.velocity.y);

        //Handle Wall Sliding
        if (IsSliding)
            Slide();

    }
    public void OnHit(int damage, Vector2 knockBack)
    {
        damageCalculation.lockVelocity = true;

        // Calculate the knockback direction based on player's facing direction
        Vector2 knockbackDirection = new Vector2(IsFacingRight ? -knockBack.x : knockBack.x, knockBack.y);

        rb2d.velocity = knockbackDirection;
    }
}