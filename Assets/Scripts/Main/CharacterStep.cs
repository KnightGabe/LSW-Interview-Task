using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStep : MonoBehaviour
{
    public int currentTerrain;

    public PlayRandomSound stoneStep;
    public PlayRandomSound grassStep;

    public void CallSound()
    {
        if (currentTerrain == 1)
            stoneStep.PlaySound();
        else if (currentTerrain == 0)
            grassStep.PlaySound();
    }
}
