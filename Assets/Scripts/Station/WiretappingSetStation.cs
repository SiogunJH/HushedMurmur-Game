using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public UnityEvent<AudioClip, Room.Controller> OnNewAudioRequest;
    public UnityEvent<bool, string> OnStationStatusChange;

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

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }
    public void ChangeLetter(bool forward)
    {
        letterDisplay.text = lettersAvailable[(lettersAvailable.IndexOf(letterDisplay.text) + (forward ? 1 : -1 + lettersAvailable.Length)) % lettersAvailable.Length].ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + 10)) % 10).ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    void ScrambleDisplay()
    {
        letterDisplay.text = lettersAvailable[Random.Range(0, lettersAvailable.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    string GetWiretappedRoomCode() => letterDisplay.text + numberDisplay.text;

    public void PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var audioSource = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, audioSpawnPosition.position, GetWiretappedRoomCode());

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 2.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        if (!isActive || sourceRoom.roomCode != audioSource.name) audioSource.mute = true;

        UnityAction<bool, string> listener = null;
        listener = (isActive, wiretappedRoom) =>
        {
            if (audioSource == null) OnStationStatusChange.RemoveListener(listener);
            else if (!isActive || audioSource.gameObject.name != wiretappedRoom) audioSource.mute = true;
            else audioSource.mute = false;
        };

        OnStationStatusChange.AddListener(listener);
    }
}
