using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerDoAction : MonoBehaviour
{
    public string interactTag;
    public UnityEvent onEnterEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(interactTag))
        {
            onEnterEvent.Invoke();
        }
    }
}
