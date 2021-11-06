using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{

    public AudioSource[] sources = new AudioSource[2];

    public AudioClip[] clips;

    public float fadeDuration;
    private int activeSource = 0;
    private float targetVolume;

    void Start()
    {
        targetVolume = sources[0].volume;
        sources[0].DOFade(targetVolume, fadeDuration).From(0);
        sources[0].Play();
        activeSource = 0;
    }

    public void CrossFadeToSong(int songId)
    {
        if (clips[songId] != sources[activeSource].clip)
        {
            if (activeSource == 0)
            {
                sources[0].DOFade(0, fadeDuration);
                sources[1].time = sources[0].time;
                sources[1].clip = clips[songId];
                sources[1].Play();
                sources[1].DOFade(targetVolume, fadeDuration);
                activeSource = 1;
            }
            else
            {
                sources[1].DOFade(0, fadeDuration);
                sources[0].time = sources[1].time;
                sources[0].clip = clips[songId];
                sources[0].Play();
                sources[0].DOFade(targetVolume, fadeDuration);
                activeSource = 0;
            }
        }
    }
}
