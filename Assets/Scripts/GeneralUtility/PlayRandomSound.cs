using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] randomFx;

    public AudioSource source;

    public void PlaySound()
    {
        source.PlayOneShot(randomFx[Random.Range(0, randomFx.Length)]);
    }
    
    public void PlaySound(AudioClip[] audioClips)
    {
        source.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
