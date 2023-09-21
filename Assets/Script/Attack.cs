using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] int Damage = 50;
    public Vector2 knockBack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageCalculation damageable = collision.GetComponent<DamageCalculation>();

        if (damageable != null)
        {
            // Calculate the knockback direction based on characyer's facing direction
            Vector2 knockBackDirection = transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            damageable.Hit(Damage, knockBackDirection);
        }
    }
}
