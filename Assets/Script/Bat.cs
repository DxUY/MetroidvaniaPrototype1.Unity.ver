using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public DetectionZone attackZone;
    Animator animator;
    Rigidbody2D rb;
    public List<Transform> wayPoints;
    public GameObject player;
    DamageCalculation damage;

    [SerializeField] float Speed = 2f;

    bool _hasTarget = false;
    public bool hasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationsHash.hasTargetHash, value);
        }
    }

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

    Transform nextWayPoint;
    int wayPointNum = 0;
    float wayPointReachDistance = .1f;
    [SerializeField]float minHorizontalDistance = 0.2f;
    [SerializeField]float minVerticalDistance = 0.07f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damage = GetComponent<DamageCalculation>();
    }
    private void Start()
    {
        nextWayPoint = wayPoints[wayPointNum];
    }

    private void Update()
    {
        hasTarget = attackZone.detectedColliders.Count > 0;

        AttackCoolDown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (damage.Alive)
        {
            if (CanMove && !hasTarget)
            {
                Fly();
            }
            else if (hasTarget)
            {
                chasePlayer();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            rb.gravityScale = 2f;
        }
    }

    private void chasePlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;

        // Calculate the horizontal distance between the enemy and the player
        float horizontalDistance = playerPosition.x - transform.position.x;
        float vericalDistance = playerPosition.y - transform.position.y;

        // Check if the horizontal distance is greater than a threshold before moving
        if (Mathf.Abs(horizontalDistance) > minHorizontalDistance && Mathf.Abs(vericalDistance) > minVerticalDistance)
            rb.velocity = directionToPlayer * Speed;
        else
            rb.velocity = Vector2.zero;
    }

    private void Fly()
    {
        //direction to the next way point
        Vector2 directionToWayPoint = (nextWayPoint.position - transform.position).normalized;

        //check to see if we reach the way point or not
        float distance = Vector2.Distance(nextWayPoint.position, transform.position);

        rb.velocity = directionToWayPoint * Speed;
        ChangingDirection();

        if (distance <= wayPointReachDistance)
        {
            wayPointNum++;
            if (wayPointNum >= wayPoints.Count)
            {
                wayPointNum = 0;
            }
        }

        nextWayPoint = wayPoints[wayPointNum];
    }

    private void ChangingDirection()
    {
        if((transform.localScale.x > 0f && rb.velocity.x < 0f) 
            || (transform.localScale.x < 0f && rb.velocity.x > 0f))
                // Flip the local scale to make the character face the other direction
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            
    }
}
