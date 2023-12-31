using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : StateMachineBehaviour
{
    public float fadeTime = 1.3f;
    public float fadeDelay = 0f;
    public float timeElapsed = 0f;
    public float fadeDelayElapsed = 0f;
    SpriteRenderer spriteRenderer;
    GameObject objToDestroy;
    Color startColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        objToDestroy = animator.gameObject;
        startColor = spriteRenderer.color;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(fadeDelay > fadeDelayElapsed)
        {
            fadeDelayElapsed += Time.deltaTime;
        }
        else
        {
            timeElapsed += Time.deltaTime;

            float newAlpha = startColor.a * (1 - timeElapsed / fadeTime);

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            if (timeElapsed > fadeTime)
                Destroy(objToDestroy);
        }
       
    }
}
