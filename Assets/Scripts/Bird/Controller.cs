using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] public Type birdType;

        [Space, SerializeField] Trait mainTrait;
        [SerializeField] Trait secondaryTrait;
        [SerializeField] Room.Type favoredRoom;

        int commonNoiseWeight = 12;
        int globalNoiseWeight = 2;
        int mainTraitWeight = 2;
        int secondaryTraitWeight = 1;

        [Space, SerializeField] float initialSleepTime = 10;

        [Space, SerializeField] float minMoveDelay = 45;
        [SerializeField] float maxMoveDelay = 60;

        [Space, SerializeField] float minNoiseEventDelay = 2;
        [SerializeField] float maxNoiseEventDelay = 8;

        [Space, SerializeField] float motionDetectionEvasionChance = 0.35f;
        [SerializeField] public float motionDetectionEvasionLimit = 3;

        [HideInInspector] public Room.Controller location;

        void Start()
        {
            Invoke(WAKE_UP, initialSleepTime);
        }

        const string WAKE_UP = "WakeUp";
        void WakeUp()
        {
            Debug.Log($"{gameObject.name} has woken up!");

            MotionDetectionStation.Instance.OnMotionDetectionTrigger.Invoke(this);

            Invoke(TRIGGER_NOISE_EVENT, minNoiseEventDelay + Random.Range(0, maxNoiseEventDelay - minNoiseEventDelay));
            Invoke(TRIGGER_MOVE, minMoveDelay + Random.Range(0, maxMoveDelay - minMoveDelay));
        }

        const string TRIGGER_NOISE_EVENT = "TriggerNoiseEvent";
        void TriggerNoiseEvent()
        {
            int totalWeight = mainTraitWeight + secondaryTraitWeight + commonNoiseWeight + globalNoiseWeight;

            float randWeight = Random.Range(0, totalWeight);

            // COMMON NOISE
            if (randWeight < commonNoiseWeight)
            {
                var noise = Manager.Instance.commonNoise[Random.Range(0, Manager.Instance.commonNoise.Length)];
                WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(noise, location);
            }
            // GLOBAL NOISE
            else if (randWeight - commonNoiseWeight < globalNoiseWeight)
            {
                var noise = Manager.Instance.globalNoise[Random.Range(0, Manager.Instance.globalNoise.Length)];
                VentilationSystem.Instance.OnNewAudioRequest.Invoke(noise);
            }
            // MAIN TRAIT
            else if (randWeight - commonNoiseWeight - globalNoiseWeight < mainTraitWeight)
                HandleTraitNoiseEvent(mainTrait);

            // SECONDARY TRAIT
            else HandleTraitNoiseEvent(secondaryTrait);


            // Reinvoke
            Invoke(TRIGGER_NOISE_EVENT, minNoiseEventDelay + Random.Range(0, maxNoiseEventDelay - minNoiseEventDelay));
        }

        void HandleTraitNoiseEvent(Trait trait)
        {
            AudioClip noise;
            switch (trait)
            {
                case Trait.Clumsy:
                    noise = Manager.Instance.clumsyTraitNoise[Random.Range(0, Manager.Instance.clumsyTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(noise, location);
                    break;

                case Trait.Heavy:
                    noise = Manager.Instance.heavyTraitNoise[Random.Range(0, Manager.Instance.heavyTraitNoise.Length)];
                    VentilationSystem.Instance.OnNewAudioRequest.Invoke(noise);
                    break;

                case Trait.Drooling:
                    noise = Manager.Instance.droolingTraitNoise[Random.Range(0, Manager.Instance.droolingTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(noise, location);
                    break;

                case Trait.Brutal:
                    noise = Manager.Instance.brutalTraitNoise[Random.Range(0, Manager.Instance.brutalTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(noise, location);
                    break;

                case Trait.Speaking:
                    noise = Manager.Instance.speakingTraitNoise[Random.Range(0, Manager.Instance.speakingTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnAudioRequestTrigger.Invoke(noise, location);
                    break;

                default:
                    Debug.LogWarning($"Unhandled trait: {trait}");
                    break;

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
