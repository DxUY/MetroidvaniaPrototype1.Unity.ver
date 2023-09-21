using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] int arrowDamage = 10;
    [SerializeField] Vector2 Speed = new(100f, 0f);

    Rigidbody2D rb2d;
    DamageCalculation damageCalculation;
    private Vector2 knockBack = Vector2.zero;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        damageCalculation = GetComponent<DamageCalculation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.AddForce(Speed * transform.localScale.x, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageCalculation damageable = collision.GetComponent<DamageCalculation>();

        if (damageable != null)
        {
                // Calculate the knockback direction based on character's facing direction
                Vector2 knockBackDirection = transform.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

                damageable.Hit(arrowDamage, knockBackDirection);

                Destroy(gameObject);
        }
    }
}
