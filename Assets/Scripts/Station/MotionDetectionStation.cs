using System.Collections;
using System.Collections.Generic;
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
    float failureChance = 0.2f;
    float failureLimit = 3;

    public void ProcessMovementAlert(Bird.Controller bird)
    {
        // ON EVASION
        if (bird.TryToEvadeDetection())
        {
            bird.motionDetectionEvasionLimit--;
            //Debug.Log("Motion Detection Evaded");
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
        Debug.Log($"*BEEP*\nMotion detected in {roomID}!");
    }
}
