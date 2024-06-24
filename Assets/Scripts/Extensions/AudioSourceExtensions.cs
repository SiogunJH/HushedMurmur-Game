using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static AudioClip SetClipFromPool(this AudioSource audioSource, IEnumerable<AudioClip> poolOfClips)
    {
        if (poolOfClips == null || poolOfClips.Count() == 0)
        {
            Debug.LogError("Poll of clips must be non-empty and must not be null.");
            return null;
        }

        var randomClip = poolOfClips.ToArray()[Random.Range(0, poolOfClips.Count() - 1)];
        audioSource.clip = randomClip;
        return randomClip;
    }
}