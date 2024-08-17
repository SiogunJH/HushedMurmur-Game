using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WiretappingSetStation : MonoBehaviourSingleton<WiretappingSetStation>, ISingleton, IToggleable
{
    [HideInInspector] public UnityEvent<bool, string> OnStationStatusChange;

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

    public void ChangeSlot1(bool forward)
    {
        var characterAvailable = Room.Manager.Instance.codeCharactersAvailable_Slot1.ToList();
        letterDisplay.text = characterAvailable[(characterAvailable.IndexOf(letterDisplay.text[0]) + (forward ? 1 : -1 + characterAvailable.Count)) % characterAvailable.Count].ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    public void ChangeSlot2(bool forward)
    {
        var characterAvailable = Room.Manager.Instance.codeCharactersAvailable_Slot2.ToList();
        numberDisplay.text = characterAvailable[(characterAvailable.IndexOf(numberDisplay.text[0]) + (forward ? 1 : -1 + characterAvailable.Count)) % characterAvailable.Count].ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    void ScrambleDisplay()
    {
        letterDisplay.text = Room.Manager.Instance.codeCharactersAvailable_Slot1[Random.Range(0, Room.Manager.Instance.codeCharactersAvailable_Slot1.Length)].ToString();
        numberDisplay.text = Room.Manager.Instance.codeCharactersAvailable_Slot2[Random.Range(0, Room.Manager.Instance.codeCharactersAvailable_Slot2.Length)].ToString();

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