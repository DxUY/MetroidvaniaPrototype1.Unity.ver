using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera camera;
    public Transform followTarget;

    Vector2 startingPos;

    //distance from the camera to the parallax object 
    float startingZ;
    
    //distance that the camera have move from the starting position
    Vector2 CamMoveSinceStart => (Vector2)camera.transform.position - startingPos;

    float ZDistanceFromTarget => (float)transform.position.z - followTarget.transform.position.z;

    float ClippingPlain => (float)camera.transform.position.z + (ZDistanceFromTarget > 0 ? camera.farClipPlane : camera.nearClipPlane);

    //the further from the player, the faster the parallax object will move, the closer the slower
    float ParallaxFactor => (float)Mathf.Abs(ZDistanceFromTarget)/ ClippingPlain;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = startingPos + CamMoveSinceStart * ParallaxFactor;

        transform.position = new Vector3(newPos.x, newPos.y, startingZ);
    }
}
