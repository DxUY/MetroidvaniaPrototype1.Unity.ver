using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections direction;
    Animator animator;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public DetectionZone rangeAttackZone;
    DamageCalculation damageCalculation;

    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float speedAccelerattion = 3f;
    [SerializeField] float delayTime = 1.3f;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float flipDelay = 2.1f;
    float flipDelayTimer;

    float movementSpeed;
    float currentSpeed;

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationsHash.canMoveHash);
        }
    }

    public float AttackCoolDown 
    {
        get
        {
            return animator.GetFloat(AnimationsHash.attackCoolDownHash);
        }
        private set
        {
            animator.SetFloat(AnimationsHash.attackCoolDownHash, Mathf.Max(value, 0));
        }
    }

    public GameObject player;

    public bool canFlip { get; private set; }

    [SerializeField] bool _closeTarget = false;
    public bool closeTarget 
    {
        get
        {
            return _closeTarget;
        }
        private set
        {
            _closeTarget = value;
            animator.SetBool(AnimationsHash.closeTargetHash, value);
        }
    }

    [SerializeField] bool _farTarget = false;
    public bool farTarget
    {
        get
        {
            return _farTarget;
        }
        private set
        {
            _farTarget = value;
            animator.SetBool(AnimationsHash.farTargetHash, value);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageCalculation = GetComponent<DamageCalculation>();
    }

    private void Start()
    {
        StartCoroutine(startTime());
    }

    private void CheckForPlayer()
    {
        Vector2 rayDirection = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = transform.position;

        Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, maxDistance, 1 << 7);

        if (hit.collider)
        {
            flipDelayTimer = flipDelay;
        }
        else if (flipDelayTimer > 0f)
        {
            flipDelayTimer -= Time.deltaTime;
        }
        else
        {
            FlipDirection();
        }
    }


    IEnumerator startTime()
    {
        damageCalculation.lockVelocity = true;

        yield return new WaitForSeconds(delayTime);

        damageCalculation.lockVelocity = false;
    }

    private void Update()
    {
        closeTarget = attackZone.detectedColliders.Count > 0;
        farTarget = rangeAttackZone.detectedColliders.Count > 0;

        AttackCoolDown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckForPlayer();

        // Check if the enemy is on a wall
        if (direction.IsOnWall && direction.IsGrounded)
            FlipDirection();
        
        if(!damageCalculation.lockVelocity)
        {
           float targetSpeed = CanMove ? maxSpeed : 0f;
           currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedAccelerattion);

            // Set velocity based on the flipped direction and current speed
            movementSpeed = (transform.localScale.x > 0) ? currentSpeed : -currentSpeed;
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if (player == null)
            return;

        float DistanceBetween = Mathf.Abs(transform.position.x - player.transform.position.x);

        if (DistanceBetween >= 1.9f)
            // Flip the local scale to make the character face the other direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void OnHit(int damage, Vector2 knockBack)
    {
        damageCalculation.lockVelocity = true;

        // Calculate the knockback direction based on player's facing direction
        Vector2 knockbackDirection = new Vector2(movementSpeed > 0 ? -knockBack.x : knockBack.x, knockBack.y);

        rb.velocity = knockbackDirection;
    }

    public void OnCliffDetected()
    {
        if (direction.IsGrounded)
            FlipDirection();
    }
}
