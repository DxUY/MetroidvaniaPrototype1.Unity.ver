using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : StateMachineBehaviour
{
    public GameObject objectToInstantiate; // Drag the prefab you want to instantiate into this field in the Inspector
    public float createTime = 0.2f;
    public float fadeDuration = .1f; // Time it takes for the color to fade in
    public float remainTime;

    SpriteRenderer spriteRenderer;
    GameObject objToCreate;
    Color startColor;
    Color targetColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        remainTime = createTime;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        objToCreate = animator.gameObject;

        startColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        targetColor = spriteRenderer.color;
        spriteRenderer.color = startColor;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        remainTime -= Time.deltaTime;

        if (remainTime <= 0f)
        {
            float progress = Mathf.Clamp01((createTime - remainTime) / fadeDuration); // Ensure progress is between 0 and 1
            spriteRenderer.color = Color.Lerp(startColor, targetColor, progress);

            Debug.Log(progress);

            if (progress >= 1f) // Check if progress reaches 100% (1.0), not an arbitrary value like 10f
            {
                // Instantiate the objectToInstantiate prefab at the same position as objToCreate
                Instantiate(objectToInstantiate, objToCreate.transform.position, Quaternion.identity);
            }
        }
    }

}

