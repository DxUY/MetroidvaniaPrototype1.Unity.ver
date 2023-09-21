using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class spellProperty : MonoBehaviour
{
    [SerializeField] Vector2 knockBack;
    [SerializeField] int spellDamage = 70;
    [SerializeField] private float delayTime = .75f;

    private void Start()
    {
        // Perform raycast to detect ground and scale based on distance
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            float distanceToGround = hit.distance;

            Debug.Log(distanceToGround);

            float scaleFactor = distanceToGround; // Adjust the factor as needed
            transform.localScale = new Vector3(scaleFactor, 0.4f + (scaleFactor * 2f), 1);

            // Debug draw the ray
            Debug.DrawRay(transform.position, Vector2.down * distanceToGround, Color.green, 5.0f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DamageCalculation damageable = collision.GetComponent<DamageCalculation>();

        if (damageable != null)
        {
            // Calculate the knockback direction based on character's facing direction
            Vector2 knockBackDirection = transform.position.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            damageable.Hit(spellDamage, knockBackDirection);

            Rigidbody2D otherRigidbody = collision.GetComponent<Rigidbody2D>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(knockBackDirection, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    public void DelayAnimation()
    {
        StartCoroutine(SpellDelay());
    }

    IEnumerator SpellDelay()
    {
        yield return new WaitForSeconds(delayTime);
    }
}
