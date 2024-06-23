using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] Type birdType;
        [HideInInspector] public Room.Controller location;

        [Space, SerializeField] float initialSleepTime = 10;

        [Space, SerializeField] float minMoveDelay = 45;
        [SerializeField] float maxMoveDelay = 60;

        [Space, SerializeField] float minNoiseEventDelay = 2;
        [SerializeField] float maxNoiseEventDelay = 8;

        Dictionary<string, float> weightedNoiseEvents = new()
        {
            {"Common", 70},
            {"Unique", 20},
            {"Global", 10},
            {"Speak", 0},
            {"Murmur", 0}
        };

        [Space, SerializeField] float motionDetectionEvasionChance = 0.35f;
        [SerializeField] public float motionDetectionEvasionLimit = 3;

        [Header("Unique Noise Events")]
        [SerializeField] AudioClip[] uniqueNoise;
        [SerializeField] AudioClip[] speakNoise;
        [SerializeField] AudioClip[] murmurNoise;

        void Start()
        {
            Invoke(WAKE_UP, initialSleepTime);
        }

        const string WAKE_UP = "WakeUp";
        void WakeUp()
        {
            Debug.Log($"{gameObject.name} has woken up!");

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
                    WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(GetNoiseEventClip(noiseEvent.Key), location);
                    break;
                }
                else randomizedWeight -= noiseEvent.Value;
            }

            // Reinvoke
            Invoke(TRIGGER_NOISE_EVENT, minNoiseEventDelay + Random.Range(0, maxNoiseEventDelay - minNoiseEventDelay));
        }

        AudioClip GetNoiseEventClip(string type)
        {
            switch (type)
            {
                case "Unique":
                    return uniqueNoise[Random.Range(0, uniqueNoise.Length)];

                case "Global":
                    MotionDetectionStation.Instance.OnMotionDetectionTrigger.Invoke(this);
                    return Manager.Instance.globalNoise[Random.Range(0, Manager.Instance.globalNoise.Length)];

                case "Speak":
                    return speakNoise[Random.Range(0, speakNoise.Length)];

                case "Murmur":
                    return murmurNoise[Random.Range(0, murmurNoise.Length)];

                default:
                    return Manager.Instance.commonNoise[Random.Range(0, Manager.Instance.commonNoise.Length)];
            }
        }

        const string TRIGGER_MOVE = "TriggerMove";
        void TriggerMove()
        {
            // If right before the office, attempt to attack
            if (location.tags.Contains(Room.Tag.End))
            {
                Attack();
                return;
            }

            // Move to the next room
            GoNextRoom();
            MotionDetectionStation.Instance.OnMotionDetectionTrigger.Invoke(this);

            // Reinvoke
            Invoke(TRIGGER_MOVE, minMoveDelay + Random.Range(0, maxMoveDelay - minMoveDelay));
        }

        public bool TryToEvadeDetection()
            => motionDetectionEvasionLimit > 0 && Random.Range(0f, 1f) < motionDetectionEvasionChance;

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
