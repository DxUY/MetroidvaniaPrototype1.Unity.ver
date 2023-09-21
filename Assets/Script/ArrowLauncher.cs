using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;

    public void FireArrow()
    {
       GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Vector2 originalScale = arrow.transform.localScale;

        arrow.transform.localScale = new Vector2(
            originalScale.x * (transform.localScale.x > 0 ? 1 : -1), originalScale.y);
    }
}
