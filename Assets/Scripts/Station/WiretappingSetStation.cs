using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [SerializeField] bool isActive = false;
    string lettersAvailable = "BGKLOPSWX";

    [SerializeField] TextMeshProUGUI letterDisplay;
    [SerializeField] TextMeshProUGUI numberDisplay;

    [SerializeField] Transform audioSpawnPosition;
    [SerializeField] GameObject headsetObject;

    void Start()
    {
        ScrambleDisplay();
    }

    public void ToggleOnOff()
    {
        isActive = !isActive;
        headsetObject.SetActive(!isActive);
    }
    public void ChangeLetter(bool forward)
    {
        int i = lettersAvailable.IndexOf(letterDisplay.text);

        letterDisplay.text = lettersAvailable[(i + (forward ? 1 : -1 + lettersAvailable.Length)) % lettersAvailable.Length].ToString();
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + 10)) % 10).ToString();
    }

    void ScrambleDisplay()
    {
        letterDisplay.text = lettersAvailable[Random.Range(0, lettersAvailable.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();
    }

    public void ProcessAudioRequest(AudioClip audioClip, Room.Controller sourceRoom)
    {
        if (!isActive) return;
        if (sourceRoom.roomCode != letterDisplay.text + numberDisplay.text) return;

        PlayAudio(audioClip);
    }

    void PlayAudio(AudioClip audioClip)
    {
        var obj = new GameObject("One-Time Audio Player");
        obj.transform.position = audioSpawnPosition.position;

        var audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
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
