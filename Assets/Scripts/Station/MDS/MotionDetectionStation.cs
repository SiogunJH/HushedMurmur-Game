using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MotionDetectionStation : MonoBehaviour
{
    #region Singleton

    public static MotionDetectionStation Instance;
    void Awake()
    {
        // Destroy self, if object of this class already exists
        if (Instance != null) Destroy(gameObject);

        //One time setup of a Singleton
        else Instance = this;
    }

    #endregion

    [SerializeField] public UnityEvent<Bird.Controller> OnMotionDetectionTrigger;
    float failureChance = 0f;
    float failureLimit = 3;

    [SerializeField] AudioSource audioSource;

    [SerializeField] TextMeshProUGUI tmpText;
    [SerializeField] int maxAmountOfLines = 8;

    void Start()
    {
        tmpText.text = "Admin> run MDS";
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
        PushMessage($"MDS> Motion in {roomID}");
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
