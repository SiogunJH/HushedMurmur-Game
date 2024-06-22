using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] BirdType birdType;
        Room.Controller location;

        [Space, SerializeField] float initialSleepTime = 10;

        [Space, SerializeField] float minMoveDelay = 45;
        [SerializeField] float maxMoveDelay = 60;

        [Space, SerializeField] float minNoiseEventDelay = 2;
        [SerializeField] float maxNoiseEventDelay = 8;

        Dictionary<string, float> weightedNoiseEvents = new()
        {
            {"Common", 70},
            {"Unique", 20},
            {"Global", 10}
        };

        [Space, SerializeField] float motionDetectionEvasionChance = 0.2f;

        void Start()
        {
            Invoke(WAKE_UP, initialSleepTime);
        }

        const string WAKE_UP = "WakeUp";
        void WakeUp()
        {
            Debug.Log($"{gameObject.name} is now active!");

            Invoke(TRIGGER_NOISE_EVENT, minNoiseEventDelay + Random.Range(0, maxNoiseEventDelay - minNoiseEventDelay));
            Invoke(TRIGGER_MOVE, minMoveDelay + Random.Range(0, maxMoveDelay - minMoveDelay));
        }

        const string TRIGGER_NOISE_EVENT = "TriggerNoiseEvent";
        void TriggerNoiseEvent()
        {
            float totalWeight = 0;
            foreach (var noiseEvent in weightedNoiseEvents) totalWeight += noiseEvent.Value;

            float randomizedWeight = Random.Range(0, totalWeight);
            foreach (var noiseEvent in weightedNoiseEvents)
            {
                if (randomizedWeight < noiseEvent.Value)
                {
                    Debug.Log($"{gameObject.name} triggered {noiseEvent.Key} Noise Event!");
                    break;
                }
                else randomizedWeight -= noiseEvent.Value;
            }

            // Reinvoke
            Invoke(TRIGGER_NOISE_EVENT, minNoiseEventDelay + Random.Range(0, maxNoiseEventDelay - minNoiseEventDelay));
        }


        const string TRIGGER_MOVE = "TriggerMove";
        void TriggerMove()
        {
            // If right before the office, attempt to attack
            if (location.tags.Contains(Room.Tag.End))
            {
                Attack();
                Debug.Log($"{gameObject.name} has attacked!");
                return;
            }

            // Move to the next room
            GoNextRoom();
            Debug.Log($"{gameObject.name} has moved!");

            // Reinvoke
            Invoke(TRIGGER_MOVE, minMoveDelay + Random.Range(0, maxMoveDelay - minMoveDelay));
        }

        public void SetLocation(Room.Controller newLocation)
        {
            if (location != null) location.RemoveOccupant(this);
            location = newLocation;
            location.AddOccupant(this);

            // Move this object to the location (debugging)
            gameObject.transform.position = location.gameObject.transform.position;
        }

        public void GoNextRoom()
        {
            if (location == null) return;
            SetLocation(location.GetNextRoom());
        }

        public void ScareAway()
        {
            Debug.Log($"{gameObject.name} was scared away!");
            Destroy(gameObject);
        }

        void Attack()
        {
            Debug.Log($"{gameObject.name} is attacking!");
        }
    }
}
