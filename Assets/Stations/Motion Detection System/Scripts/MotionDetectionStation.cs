using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class MotionDetectionStation : MonoBehaviourSingleton<MotionDetectionStation>, ISingleton
{
    [SerializeField] public UnityEvent<Bird.Controller> OnMotionDetectionTrigger;
    float failureChance = 0f;
    float failureLimit = 3;

    AudioSource audioSource;

    [SerializeField] TextMeshProUGUI tmpText;
    [SerializeField] int maxAmountOfLines = 7;

    const string START_MESSAGE = "Admin> run MDS";
    const string MOTION_ALERT_MESSAGE = "MDS> Motion in";

    void Start()
    {
        tmpText.text = START_MESSAGE;
        audioSource = GetComponent<AudioSource>();
    }

    public void ProcessMovementAlert(Bird.Controller bird)
    {
        // ON EVASION
        if (bird.TryToEvadeDetection())
        {
            bird.motionDetectionEvasionLimit--;
            return;
        }

        // ON SYSTEM FAILUIRE
        if (failureLimit > 0 && Random.Range(0f, 1f) < failureChance)
        {
            var unoccupiedRooms = Room.Manager.Instance.GetUnoccupiedRooms();
            SendMovementAlert(unoccupiedRooms[Random.Range(0, unoccupiedRooms.Length)].roomCode);
            failureLimit--;
            return;
        }

        // ON SUCCESS
        SendMovementAlert(bird.location.roomCode);
    }

    void SendMovementAlert(string roomID)
    {
        audioSource.Play();
        PushMessage($"{MOTION_ALERT_MESSAGE} {roomID}");
    }

    void PushMessage(string message)
    {
        if (tmpText.GetAmountOfLineBreaks() >= maxAmountOfLines)
        {
            string[] lines = tmpText.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            tmpText.text = string.Join(System.Environment.NewLine, lines, 1, lines.Length - 1);
        }

        tmpText.text += $"\n{message}";
    }
}