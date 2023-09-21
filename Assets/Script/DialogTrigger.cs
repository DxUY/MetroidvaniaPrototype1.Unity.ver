using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    public GameObject visualCue;

    bool playerInRange;

    [Header("Ink JSON")]
    public TextAsset inkJson;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !DialogManager.GetInstance().isDialogPlaying)
        {
            visualCue.SetActive(true);
            if(PlayerMovement.GetInstance().GetInteractPressed())
            {
                DialogManager.GetInstance().EnterDialog(inkJson);
            }
        }
        else 
            visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
    }
}
