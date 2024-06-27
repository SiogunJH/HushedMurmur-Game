using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        string letters = new string(EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray());
        letterDisplay.text = letters[(letters.IndexOf(letterDisplay.text) + (forward ? 1 : -1 + letters.Length)) % letters.Length].ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + Room.Manager.roomCodeAmountOfNumbers)) % Room.Manager.roomCodeAmountOfNumbers).ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    void ScrambleDisplay()
    {
        char[] letters = EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray();

        letterDisplay.text = letters[Random.Range(0, letters.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();

        OnStationStatusChange?.Invoke(isActive, GetWiretappedRoomCode());
    }

    string GetWiretappedRoomCode() => letterDisplay.text + numberDisplay.text;

    public void PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var audioSource = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, audioSpawnPosition.position, sourceRoom.roomCode);

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 5.0f;
        audioSource.maxDistance = 10.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        if (!isActive || sourceRoom.roomCode != GetWiretappedRoomCode()) audioSource.mute = true;

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
