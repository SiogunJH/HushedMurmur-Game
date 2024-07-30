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

    [HideInInspector] public UnityEvent<bool, string> OnStationStatusChange;

    public bool IsActive { get; private set; } = false;
    public string WiretappedRoomCode { get => letterDisplay.text + numberDisplay.text; }

    const float HEADSET_ANIMATION_LENGTH = 0.35f;

    [Space, Header("Sound"), SerializeField] AudioClip[] enableSound;
    [SerializeField] AudioClip[] disableSound;

    [Space, Header("Children References"), SerializeField] TextMeshProUGUI letterDisplay;
    [SerializeField] TextMeshProUGUI numberDisplay;
    [SerializeField] GameObject headsetObject;
    [SerializeField] GameObject headsetTransformMarker;
    [SerializeField] GameObject headsetTableParent;

    // Coroutines
    Coroutine headsetCoroutine;

    void Start()
    {
        ScrambleDisplay();
    }

    public void ToggleOnOff()
    {
        IsActive = !IsActive;

        if (IsActive) TurnOn();
        else TurnOff();

        OnStationStatusChange?.Invoke(IsActive, WiretappedRoomCode);
    }

    void TurnOn()
    {
        AudioSourceExtensions.PlayOneTimeAudio(this, enableSound[Random.Range(0, enableSound.Length)], headsetObject, "Pick up Headset");
        headsetObject.transform.SetParent(PlayerLook.Instance.gameObject.transform);

        if (headsetCoroutine != null) StopCoroutine(headsetCoroutine);
        headsetCoroutine = StartCoroutine(MoveHeadset(PlayerLook.Instance.gameObject.transform, HEADSET_ANIMATION_LENGTH));
    }

    void TurnOff()
    {
        headsetObject.transform.SetParent(headsetTableParent.transform);
        AudioSourceExtensions.PlayOneTimeAudio(this, disableSound[Random.Range(0, disableSound.Length)], headsetObject, "Put away Headset");

        if (headsetCoroutine != null) StopCoroutine(headsetCoroutine);
        headsetCoroutine = StartCoroutine(MoveHeadset(headsetTransformMarker.transform, HEADSET_ANIMATION_LENGTH));
    }

    IEnumerator MoveHeadset(Transform targetTransform, float time)
    {
        Vector3 initialPosition = headsetObject.transform.position;
        Quaternion initialRotation = headsetObject.transform.rotation;
        float totalTime = time;
        float progress;

        while (time != 0)
        {
            time = Mathf.Clamp(time - Time.deltaTime, 0, totalTime);
            progress = 1 - (time / totalTime);

            var newPosition = Vector3.Slerp(initialPosition, targetTransform.position, progress);
            var newRotation = Quaternion.Slerp(initialRotation, targetTransform.rotation, progress);

            headsetObject.transform.SetPositionAndRotation(newPosition, newRotation);

            yield return null;
        }
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

    public (AudioSource audioSource, Coroutine coroutine) PlayAudio(AudioClip audioClip, Room.Controller sourceRoom)
    {
        var oneTimeAudio = AudioSourceExtensions.PlayOneTimeAudio(this, audioClip, headsetObject, name: sourceRoom.roomCode);

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
