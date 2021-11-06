using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffects : MonoBehaviour
{

    public PlayRandomSound clickSounds;
    public PlayRandomSound hoverSounds;
    public PlayRandomSound failSounds;

    public Button myButton;


    public void Hover()
    {
        if (myButton.interactable)
        {
            hoverSounds.PlaySound();
            if(!DOTween.IsTweening(transform))
                transform.DOShakeRotation(0.1f, 10);
        }
        else
        {
            failSounds.PlaySound();
            if (!DOTween.IsTweening(transform))
                transform.DOShakePosition(0.1f, 0.3f);
        }
    }

    public void Click()
    {
        if (myButton.interactable)
        {
            clickSounds.PlaySound();
            if (!DOTween.IsTweening(transform))
                transform.DOShakeScale(0.1f, 0.5f, 2, 0);
        }
    }
}
