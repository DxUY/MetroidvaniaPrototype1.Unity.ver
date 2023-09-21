using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DamageCalculation : MonoBehaviour
{
    [SerializeField] int _maxHealth = 300;
    Animator animator;
    public UnityEvent<int, Vector2> damageableHits;
    public HealthBar healthBar;
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        private set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    int _health = 300;

    public int Health
    {
        get
        {
            return _health;
        }

        private set 
        {
            _health = value;

            if(_health <= 0)
                Alive = false;
        }
    }

    public bool lockVelocity
    {
        get
        {
            return animator.GetBool(AnimationsHash.lockVelocityHash);
        }
        set
        {
            animator.SetBool(AnimationsHash.lockVelocityHash, value);
        }
    }

    [SerializeField] bool _alive = true;

    bool isInvicible = false;

    float timeSinceHit = 0f;

    [SerializeField]
    float invicibilityTime = .25f;

    public string endSceneName;
    public bool Alive
    {
        get 
        {
            return _alive;
        }
        private set
        {
            _alive = value;
            animator.SetBool(AnimationsHash.isAliveHash, value);

            if (!_alive)
            {
                // Transition to the end scene
                SceneManager.LoadScene(endSceneName);
            }
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        healthBar.SetMaxHealth(MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInvicible)
        {
            if (timeSinceHit > invicibilityTime)
            {
                isInvicible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public void Hit(int damage, Vector2 knockBack)
    {
        if (Alive && !isInvicible)
        {
            Health -= damage;
            isInvicible = true;

            animator.SetTrigger(AnimationsHash.hitTriggerHash);
            //notify other subcribed components that the hit was successful to handle knockback
            damageableHits?.Invoke(damage, knockBack);   

            healthBar.SetHealth(Health);
        }
    }
}
