using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WiretappingSetStation : MonoBehaviourSingleton<WiretappingSetStation>, ISingleton, IToggleable
{
    [HideInInspector] public UnityEvent<bool, string> OnStationStatusChange;

    [Header("Room IDs Settings")]
    [SerializeField] char[] lettersAvailable;

    [Space, Header("Children References")]
    [SerializeField] TextMeshProUGUI letterDisplay;
    [SerializeField] TextMeshProUGUI numberDisplay;
    [Space, SerializeField] GameObject audioOutput;

    void Start()
    {
        ScrambleDisplay();
    }

    #region IToggleable

    public bool IsTurnedOn { get; private set; } = true;

    [Space, Header("Events"), SerializeField] UnityEvent OnTurnOn;
    [SerializeField] UnityEvent OnTurnOff;

    public void ToggleOnOff()
    {
        IsTurnedOn = !IsTurnedOn;

        if (IsTurnedOn) OnTurnOn?.Invoke();
        else OnTurnOff?.Invoke();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    #endregion

    #region Displays

    public string WiretappedRoomCode { get => letterDisplay.text + numberDisplay.text; }

    public void ChangeLetter(bool forward)
    {
        string letters = new string(lettersAvailable.ToArray());
        letterDisplay.text = letters[(letters.IndexOf(letterDisplay.text) + (forward ? 1 : -1 + letters.Length)) % letters.Length].ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + Room.Manager.roomCodeAmountOfNumbers)) % Room.Manager.roomCodeAmountOfNumbers).ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    void ScrambleDisplay()
    {
        string letters = new string(lettersAvailable.ToArray());

        letterDisplay.text = letters[Random.Range(0, letters.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    #endregion

    #region Audio Handling

    public (AudioSource audioSource, Coroutine coroutine) PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var oneTimeAudio = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, audioOutput, name: sourceRoom.roomCode);

        oneTimeAudio.audioSource.spatialBlend = 1.0f;
        oneTimeAudio.audioSource.minDistance = 5.0f;
        oneTimeAudio.audioSource.maxDistance = 10.0f;
        oneTimeAudio.audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        if (!IsTurnedOn || sourceRoom.roomCode != WiretappedRoomCode) oneTimeAudio.audioSource.mute = true;

        UnityAction<bool, string> listener = null;
        listener = (isTurnedOn, wiretappedRoom) =>
        {
            if (oneTimeAudio.audioSource == null) OnStationStatusChange.RemoveListener(listener);
            else if (!isTurnedOn || oneTimeAudio.audioSource.gameObject.name != wiretappedRoom) oneTimeAudio.audioSource.mute = true;
            else oneTimeAudio.audioSource.mute = false;
        };

        OnStationStatusChange.AddListener(listener);

        return oneTimeAudio;
    }

    #endregion
}