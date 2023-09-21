using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D contactFilter;
    CapsuleCollider2D capsule;
    Animator animator;
    
    List<RaycastHit2D> groundHitResults = new();
    List<RaycastHit2D> wallHitResults = new();
    List<RaycastHit2D> cellingHitResults = new();


    [SerializeField] float groundDistance = .05f;
    [SerializeField] float wallDistance = 0.2f;
    [SerializeField] float cellingDistance = 0.05f;


    public Vector2 wallDirection => (float)gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [SerializeField]
    bool _isGrounded;
    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationsHash.isGroundedHash, value);
        }
    }

    [SerializeField]
    bool _isOnWall;
    public bool IsOnWall 
    { 
        get
        {
  
            return _isOnWall;
        }
        set
        {
            _isOnWall = value;
            animator.SetBool(AnimationsHash.isOnWallHash, value);
        }
    }

    bool _isOnCelling;

    public bool IsOnCelling
    {
        get
        {
            return _isOnCelling;
        }
        private set
        {
            _isOnCelling = value;
            animator.SetBool(AnimationsHash.isOnCellingHash, value);
        }
    }

    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        IsGrounded = capsule.Cast(Vector2.down, contactFilter, groundHitResults, groundDistance) > 0;

        IsOnWall = capsule.Cast(wallDirection, contactFilter, wallHitResults, wallDistance) > 0;

        IsOnCelling = capsule.Cast(Vector2.up, contactFilter, cellingHitResults, cellingDistance) > 0;
    }
}
