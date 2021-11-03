using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public GameObject pauseMenu;

    public UnityEvent pauseEvent;
    public UnityEvent resumeEvent;


    public KeyCode menuKey;

    public bool gamePaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            PauseGame(!gamePaused);
        }
    }

    public void PauseGame(bool value)
    {
        gamePaused = value;
        TopDownCharacterController.Instance.ToggleMovement(!value);
        if (value)
            pauseEvent.Invoke();
        else
            resumeEvent.Invoke();
    }
}
