using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class DetectionZone : MonoBehaviour
{
    Collider2D col;
    public List<Collider2D> detectedColliders = new();
    public UnityEvent onCliffDetected;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);

        onCliffDetected.Invoke();
    }
}
