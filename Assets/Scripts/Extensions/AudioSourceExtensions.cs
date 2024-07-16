using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioSourceExtensions
{
    /// <summary>
    /// Sets the <i>clip</i> property of <i>AudioSource</i> to an <i>AudioClip</i> randomly chosen from the provided pool of clips
    /// </summary>
    /// <param name="audioSource"><i>AudioSource</i> component, to which a clip will be assigned.</param>
    /// <param name="poolOfClips">A collection of <i>AudioClip</i> variables, from which a random clip will be chosen.</param>
    /// <returns>Randomly chosen clip, that was assigned to <i>AudioSource</i> component.</returns>
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

    /// <summary>
    /// Creates an empty <i>GameObject</i> with an <i>AudioSource</i> component in a specified position. 
    /// Automatically plays the provided <i>AudioClip</i> via created component, and then destroys itself.
    /// </summary>
    /// <param name="executor"><i>MonoBehaviour</i> component, what will execute the coroutine responsible for removing <i>GameObject</i> upon finishing assigned <i>AudioClip</i>.</param>
    /// <param name="audioClip"><i>AudioClip</i> that will be played.</param>
    /// <param name="position">Position, in which a <i>GameObject</i> will be created.</param>
    /// <param name="name">Name of a created <i>GameObject</i></param>
    /// <param name="spatialBlend">[0,1] How much audio is treated as 3D source.</param>
    /// <returns><i>AudioSource</i> component of a created <i>GameObject</i></returns>
    public static AudioSource PlayOneTimeAudio(this MonoBehaviour executor, AudioClip audioClip, Vector3 position, string name = "One-Time Audio Player", GameObject parent = null)
    {
        // create new game object
        var obj = new GameObject(name);
        obj.transform.position = position;

        // attach an audio source component to created game object and play provided clip
        var audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        // attach game object to a parent, if provided with one
        if (parent != null) obj.transform.SetParent(parent.transform, true);

        // start coroutine
        executor.StartCoroutine(DestroyAfterPlaying(audioSource));

        // return audio source component
        return audioSource;
    }

    /// <summary>
    /// A coroutine that will remove a game object, upon finishin playing.
    /// </summary>
    /// <param name="audioSource"><i>AudioSource</i> component that will trigger <i>GameObject</i>'s removal, upon finishing.</param>
    static IEnumerator DestroyAfterPlaying(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        GameObject.Destroy(audioSource.gameObject);
    }

    public static AudioSourceSettings SavePreset(this AudioSource audioSource)
    {
        AudioSourceSettings ass;

        ass.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        ass.mute = audioSource.mute;
        ass.bypassEffects = audioSource.bypassEffects;
        ass.bypassListenerEffects = audioSource.bypassListenerEffects;
        ass.bypassReverbZones = audioSource.bypassReverbZones;
        ass.playOnAwake = audioSource.playOnAwake;
        ass.loop = audioSource.loop;

        ass.priority = audioSource.priority;
        ass.volume = audioSource.volume;
        ass.pitch = audioSource.pitch;
        ass.panStereo = audioSource.panStereo;
        ass.spatialBlend = audioSource.spatialBlend;
        ass.reverbZoneMix = audioSource.reverbZoneMix;

        ass.dopplerLevel = audioSource.dopplerLevel;
        ass.spread = audioSource.spread;
        ass.rolloffMode = audioSource.rolloffMode;
        ass.minDistance = audioSource.minDistance;
        ass.maxDistance = audioSource.maxDistance;

        return ass;
    }

    public static void LoadPreset(this AudioSource audioSource, AudioSourceSettings ass)
    {
        audioSource.outputAudioMixerGroup = ass.outputAudioMixerGroup;
        audioSource.mute = ass.mute;
        audioSource.bypassEffects = ass.bypassEffects;
        audioSource.bypassListenerEffects = ass.bypassListenerEffects;
        audioSource.bypassReverbZones = ass.bypassReverbZones;
        audioSource.playOnAwake = ass.playOnAwake;
        audioSource.loop = ass.loop;

        audioSource.priority = ass.priority;
        audioSource.volume = ass.volume;
        audioSource.pitch = ass.pitch;
        audioSource.panStereo = ass.panStereo;
        audioSource.spatialBlend = ass.spatialBlend;
        audioSource.reverbZoneMix = ass.reverbZoneMix;

        audioSource.dopplerLevel = ass.dopplerLevel;
        audioSource.spread = ass.spread;
        audioSource.rolloffMode = ass.rolloffMode;
        audioSource.minDistance = ass.minDistance;
        audioSource.maxDistance = ass.maxDistance;
    }
}

public struct AudioSourceSettings
{
    public AudioMixerGroup outputAudioMixerGroup;
    public bool mute;
    public bool bypassEffects;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;
    public bool playOnAwake;
    public bool loop;

    public int priority;
    public float volume;
    public float pitch;
    public float panStereo;
    public float spatialBlend;
    public float reverbZoneMix;

    public float dopplerLevel;
    public float spread;
    public AudioRolloffMode rolloffMode;
    public float minDistance;
    public float maxDistance;
}