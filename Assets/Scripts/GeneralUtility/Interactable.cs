using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    //icon showing interaction is possible
    public GameObject interactionIcon;

    //stores the nearby player
    protected TopDownCharacterController nearbyPlayer = null;

    //using events allows for greater flexibility
    public UnityEvent interactionEvent;

    //Key for the interaction
    public KeyCode interactionKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TopDownCharacterController player = collision.GetComponent<TopDownCharacterController>();
        if (player != null)
        {
            nearbyPlayer = player;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (nearbyPlayer != null)
        {
            TopDownCharacterController player = collision.GetComponent<TopDownCharacterController>();
            if (player != null)
            {
                nearbyPlayer = null;
                interactionIcon.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (nearbyPlayer)
        {
            if (nearbyPlayer.canMove)
            {
                if (!interactionIcon.activeInHierarchy)
                    interactionIcon.SetActive(true);
                if (Input.GetKeyDown(interactionKey))
                    Interact();
            }
            else if (interactionIcon.activeInHierarchy)
                interactionIcon.SetActive(false);
        }
    }

    protected void Interact()
    {
        interactionIcon.SetActive(false);
        nearbyPlayer.ToggleMovement(false);
        interactionEvent.Invoke();
    }
}
