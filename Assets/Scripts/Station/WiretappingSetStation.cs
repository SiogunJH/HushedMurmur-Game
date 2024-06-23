using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WiretappingSetStation : MonoBehaviour
{
    #region Singleton

    public static WiretappingSetStation Instance;
    void Awake()
    {
        // Destroy self, if object of this class already exists
        if (Instance != null) Destroy(gameObject);

        //One time setup of a Singleton
        else Instance = this;
    }

    #endregion

    public UnityEvent<AudioClip, Room.Controller> OnAudioRequestTrigger;

    [SerializeField] Room.Controller roomCurrentlySpiedOn;
    [SerializeField] bool isActive = false;

    public void ProcessAudioRequest(AudioClip audioClip, Room.Controller sourceRoom)
    {
        if (sourceRoom != roomCurrentlySpiedOn) return;
        if (!isActive) return;

        PlayAudio(audioClip);
    }

    [SerializeField] Transform audioSpawnPosition;
    void PlayAudio(AudioClip audioClip)
    {
        var obj = new GameObject("One-Time Audio Player");
        obj.transform.position = audioSpawnPosition.position;

        var audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 0.0f;
        audioSource.maxDistance = 2.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        audioSource.Play();

        StartCoroutine(DestroyAfterPlaying(obj, audioSource));
    }

    IEnumerator DestroyAfterPlaying(GameObject obj, AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying && isActive);
        Destroy(obj);
    }
}
