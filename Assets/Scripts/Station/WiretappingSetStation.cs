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

    public bool IsActive { get; private set; } = false;

    [Space, SerializeField] AudioClip[] enableSound;
    [Space, SerializeField] AudioClip[] disableSound;

    [Space, SerializeField] TextMeshProUGUI letterDisplay;
    [SerializeField] TextMeshProUGUI numberDisplay;

    [SerializeField] GameObject headsetObject;
    Vector3 headsetPositionOnTable;
    Quaternion headsetRotationOnTable;

    public string WiretappedRoomCode { get => letterDisplay.text + numberDisplay.text; }

    void Start()
    {
        ScrambleDisplay();

        headsetPositionOnTable = headsetObject.transform.position;
        headsetRotationOnTable = headsetObject.transform.rotation;
    }

    public void ToggleOnOff()
    {
        IsActive = !IsActive;

        if (IsActive)
        {
            AudioSourceExtensions.PlayOneTimeAudio(this, enableSound[0], headsetObject.transform.position, "Pick up Headset", headsetObject);

            headsetObject.transform.position = PlayerLook.Instance.gameObject.transform.position;
            headsetObject.transform.rotation = PlayerLook.Instance.gameObject.transform.rotation;
        }
        else
        {
            headsetObject.transform.position = headsetPositionOnTable;
            headsetObject.transform.rotation = headsetRotationOnTable;

            AudioSourceExtensions.PlayOneTimeAudio(this, disableSound[0], headsetObject.transform.position, "Put away Headset", headsetObject);
        }

        OnStationStatusChange?.Invoke(IsActive, WiretappedRoomCode);
    }
    public void ChangeLetter(bool forward)
    {
        string letters = new string(EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray());
        letterDisplay.text = letters[(letters.IndexOf(letterDisplay.text) + (forward ? 1 : -1 + letters.Length)) % letters.Length].ToString();

        OnStationStatusChange?.Invoke(IsActive, WiretappedRoomCode);
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + Room.Manager.roomCodeAmountOfNumbers)) % Room.Manager.roomCodeAmountOfNumbers).ToString();

        OnStationStatusChange?.Invoke(IsActive, WiretappedRoomCode);
    }

    void ScrambleDisplay()
    {
        char[] letters = EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray();

        letterDisplay.text = letters[Random.Range(0, letters.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();

        OnStationStatusChange?.Invoke(IsActive, WiretappedRoomCode);
    }

    public void PlayAudioWrapper(AudioClip audioClip, Room.Controller sourceRoom) => PlayAudio(audioClip, sourceRoom); // This bullshit will be removed as soon as I can; it's here because UnityEvent accepts only void methods
    public (AudioSource audioSource, Coroutine coroutine) PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var oneTimeAudio = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, headsetObject.transform.position, name: sourceRoom.roomCode, parent: headsetObject);

        oneTimeAudio.audioSource.spatialBlend = 1.0f;
        oneTimeAudio.audioSource.minDistance = 5.0f;
        oneTimeAudio.audioSource.maxDistance = 10.0f;
        oneTimeAudio.audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        if (!IsActive || sourceRoom.roomCode != WiretappedRoomCode) oneTimeAudio.audioSource.mute = true;

        UnityAction<bool, string> listener = null;
        listener = (IsActive, wiretappedRoom) =>
        {
            if (oneTimeAudio.audioSource == null) OnStationStatusChange.RemoveListener(listener);
            else if (!IsActive || oneTimeAudio.audioSource.gameObject.name != wiretappedRoom) oneTimeAudio.audioSource.mute = true;
            else oneTimeAudio.audioSource.mute = false;
        };

        OnStationStatusChange.AddListener(listener);

        return oneTimeAudio;
    }
}
