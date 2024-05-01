using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSourceComponent;
    [SerializeField]
    AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        if (audioSourceComponent == null) Debug.LogWarning($"Missing [Audio Source Component] reference in {gameObject.name}");
        if (audioClips == null || audioClips.Length == 0) Debug.LogWarning($"Missing [Audio Clips] reference(s) in {gameObject.name}");
    }

    public void Play()
    {
        // Abort if no clips are available
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError($"Missing [Audio Clips] reference(s) in {gameObject.name}");
            return;
        }

        // Play random clip
        audioSourceComponent.clip = audioClips[Random.Range(0, audioClips.Length - 1)];
        audioSourceComponent.Play();
    }
}
