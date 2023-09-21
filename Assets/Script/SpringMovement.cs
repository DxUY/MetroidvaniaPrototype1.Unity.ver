using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpringMovement : MonoBehaviour
{
    //normal height
    public float targetHeight = 0f;

    //current height
    public float height = 0f;

    public float velocity = 0f;
    public float force = 0f;

    private static SpriteShapeController spriteShapeController = null;
    private int waveIndex = 0;
    float resistance = 30f;

    public ParticleSystem splash;

    public void Init(SpriteShapeController ssc)
    {

        var index = transform.GetSiblingIndex();
        waveIndex = index + 1;

        spriteShapeController = ssc;
        velocity = 0;
        height = transform.localPosition.y;
        targetHeight = transform.localPosition.y;
    }

    public void WaveSpringUpdate(float springStiffness, float dampering)
    {
        height = transform.localPosition.y;

        //maximum retraction
        var x = height - targetHeight;
        var loss = -dampering * velocity;

        force = - springStiffness * x + loss;
        velocity += force;
        transform.localPosition = new Vector3(transform.localPosition.x, height + velocity * Time.deltaTime, transform.localPosition.z);
    }
    public void WavePointUpdate()
    {
        if (spriteShapeController != null)
        {
            Spline waterSpline = spriteShapeController.spline;
            Vector3 wavePosition = waterSpline.GetPosition(waveIndex);
            waterSpline.SetPosition(waveIndex, new Vector3(wavePosition.x, transform.localPosition.y, wavePosition.z));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the other object that collided with this object.
        GameObject otherObject = collision.gameObject;

        // Check if the other object is a rigidbody.
        if (otherObject.GetComponent<Rigidbody2D>())
        {
            // Apply a force to the other object in the opposite direction of the string's movement.
            Rigidbody2D otherRigidbody = otherObject.GetComponent<Rigidbody2D>();
            var speed = otherRigidbody.velocity;

            velocity += speed.y / resistance;
        }
    }

}
