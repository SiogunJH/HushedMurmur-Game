using System.Collections;
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

    public static AudioSource PlayOneTimeAudio(this MonoBehaviour executor, AudioClip audioClip, Vector3 position)
    {
        var obj = new GameObject("One-Time Audio Player");
        obj.transform.position = position;

        var audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        executor.StartCoroutine(DestroyAfterPlaying(obj, audioSource));

        return audioSource;
    }

    static IEnumerator DestroyAfterPlaying(GameObject obj, AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        GameObject.Destroy(obj);
    }
}