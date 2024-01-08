using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Manager
{
    public static void playSound(ref AudioSource source, AudioClip clip, float pitchVariance)
    {
        // Play sound.
        float pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
        source.pitch = pitch;

        source.PlayOneShot(clip); // One shot so sounds can overlap.
    }
    
    public static void playRandomSound(ref AudioSource source, AudioClip[] clips, float pitchVariance)
    {
        // Play random sound.
        float pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
        source.pitch = pitch;
        
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(clip);
    }
}
