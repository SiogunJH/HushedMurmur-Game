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
    [SerializeField] AudioSource audioSourceTemplate;

    [Space, SerializeField] AudioClip whiteNoise;

    void Start()
    {
        ScrambleDisplay();
        SpawnWhiteNoise();
        ToggleOnOff(); // Turn off
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
        if (!IsTurnedOn) return;

        var characterAvailable = Room.Manager.Instance.codeCharactersAvailable_Slot1.ToList();
        letterDisplay.text = characterAvailable[(characterAvailable.IndexOf(letterDisplay.text[0]) + (forward ? 1 : -1 + characterAvailable.Count)) % characterAvailable.Count].ToString();

        OnStationStatusChange?.Invoke(IsTurnedOn, WiretappedRoomCode);
    }

    public void ChangeSlot2(bool forward)
    {
        if (!IsTurnedOn) return;

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

    public (AudioSource audioSource, Coroutine coroutine) SpawnAudio(AudioClip audioClip, Room.Controller sourceRoom, float maxPitchOffset = 0.15f)
    {
        var independentAudio = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, audioOutput, name: sourceRoom.roomCode);
        independentAudio.audioSource.LoadPreset(audioSourceTemplate.SavePreset());
        independentAudio.audioSource.pitch += Random.Range(-maxPitchOffset, maxPitchOffset);

        if (!IsTurnedOn || sourceRoom.roomCode != WiretappedRoomCode) independentAudio.audioSource.mute = true;

        UnityAction<bool, string> listener = null;
        listener = (isTurnedOn, wiretappedRoom) =>
        {
            if (independentAudio.audioSource == null) OnStationStatusChange.RemoveListener(listener);
            else if (!isTurnedOn || independentAudio.audioSource.gameObject.name != wiretappedRoom) independentAudio.audioSource.mute = true;
            else independentAudio.audioSource.mute = false;
        };
        OnStationStatusChange.AddListener(listener);

        return independentAudio;
    }

    void SpawnWhiteNoise()
    {
        var independentAudio = AudioSourceExtensions.PlayOneTimeAudio(this, whiteNoise, audioOutput, name: "Independent Audio (White Noise)");
        independentAudio.audioSource.LoadPreset(audioSourceTemplate.SavePreset());
        independentAudio.audioSource.loop = true;

        if (!IsTurnedOn) independentAudio.audioSource.mute = true;
        UnityAction<bool, string> listener = null;
        listener = (isTurnedOn, wiretappedRoom) =>
        {
            if (independentAudio.audioSource == null) OnStationStatusChange.RemoveListener(listener);
            else if (!isTurnedOn || Room.Manager.Instance.DoesRoomExist(wiretappedRoom)) independentAudio.audioSource.mute = true;
            else independentAudio.audioSource.mute = false;
        };
        OnStationStatusChange.AddListener(listener);
    }

    #endregion
}