using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VentilationSystem : MonoBehaviour
{
    #region Singleton

    public static VentilationSystem Instance;
    void Awake()
    {
        // Destroy self, if object of this class already exists
        if (Instance != null) Destroy(gameObject);

        //One time setup of a Singleton
        else Instance = this;
    }

    #endregion

    [SerializeField] public UnityEvent<AudioClip> OnNewAudioRequest;

    public void PlayAudio(AudioClip audioClip)
    {
        var audioSource = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, transform.position);

        audioSource.spatialBlend = 1.0f;
        audioSource.volume = 0.35f;
        audioSource.minDistance = 2.0f;
        audioSource.maxDistance = 20.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
    }
}
