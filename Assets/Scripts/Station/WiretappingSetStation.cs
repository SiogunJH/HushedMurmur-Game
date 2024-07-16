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

    bool isActive = false;

    [Space, SerializeField] AudioClip[] enableSound;
    [Space, SerializeField] AudioClip[] disableSound;

    [Space, SerializeField] TextMeshProUGUI letterDisplay;
    [SerializeField] TextMeshProUGUI numberDisplay;

    [SerializeField] GameObject headsetObject;
    Vector3 headsetPositionOnTable;
    Quaternion headsetRotationOnTable;

    string WiretappedRoomCode { get => letterDisplay.text + numberDisplay.text; }

    void Start()
    {
        ScrambleDisplay();

        headsetPositionOnTable = headsetObject.transform.position;
        headsetRotationOnTable = headsetObject.transform.rotation;
    }

    public void ToggleOnOff()
    {
        isActive = !isActive;

        if (isActive)
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

        OnStationStatusChange?.Invoke(isActive, WiretappedRoomCode);
    }
    public void ChangeLetter(bool forward)
    {
        string letters = new string(EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray());
        letterDisplay.text = letters[(letters.IndexOf(letterDisplay.text) + (forward ? 1 : -1 + letters.Length)) % letters.Length].ToString();

        OnStationStatusChange?.Invoke(isActive, WiretappedRoomCode);
    }

    public void ChangeNumber(bool forward)
    {
        numberDisplay.text = ((int.Parse(numberDisplay.text) + (forward ? 1 : -1 + Room.Manager.roomCodeAmountOfNumbers)) % Room.Manager.roomCodeAmountOfNumbers).ToString();

        OnStationStatusChange?.Invoke(isActive, WiretappedRoomCode);
    }

    void ScrambleDisplay()
    {
        char[] letters = EnumExtensions.GetEnumValues<Room.Type>().Select(i => (char)i).ToArray();

        letterDisplay.text = letters[Random.Range(0, letters.Length)].ToString();
        numberDisplay.text = Random.Range(0, 10).ToString();

        OnStationStatusChange?.Invoke(isActive, WiretappedRoomCode);
    }

    public void PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var audioSource = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, headsetObject.transform.position, name: sourceRoom.roomCode, parent: headsetObject);

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 5.0f;
        audioSource.maxDistance = 10.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        if (!isActive || sourceRoom.roomCode != WiretappedRoomCode) audioSource.mute = true;

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
