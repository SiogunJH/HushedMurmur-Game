using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {

        [SerializeField] public Type birdType;

        [Header("Traits and Preferences")]
        [Space, SerializeField] public Trait mainTrait;
        [SerializeField] public Trait secondaryTrait;
        [SerializeField] public Room.Type favoredRoom;
        [SerializeField] public Color repellantColor;

        int commonNoiseWeight = 120;
        int globalNoiseWeight = 20;
        int mainTraitWeight = 20;
        int secondaryTraitWeight = 10;

        [Header("Statistics")]
        [Space, SerializeField] public float initialSleepTime = 10;

        [Space, SerializeField] float moveDelay = 60;
        [SerializeField] float moveDelayOffset = 15;

        [Space, SerializeField] float noiseEventDelay = 5;
        [SerializeField] float noiseEventDelayOffset = 3;

        [Space, SerializeField] float motionDetectionEvasionChance = 0.25f;
        [SerializeField] public float motionDetectionEvasionLimit = 3;

        [HideInInspector] public Room.Controller location;

        void Start()
        {
            Invoke(WAKE_UP, initialSleepTime);
        }

        const string WAKE_UP = "WakeUp";
        void WakeUp()
        {
            MotionDetectionStation.Instance.OnMotionDetectionTrigger.Invoke(this);

            Invoke(TRIGGER_NOISE_EVENT, Random.Range(noiseEventDelay - noiseEventDelayOffset, noiseEventDelay + noiseEventDelayOffset));
            Invoke(TRIGGER_MOVE, Random.Range(moveDelay - moveDelayOffset, moveDelay + moveDelayOffset));
        }

        const string TRIGGER_NOISE_EVENT = "TriggerNoiseEvent";
        void TriggerNoiseEvent()
        {
            int totalWeight = mainTraitWeight + secondaryTraitWeight + commonNoiseWeight + globalNoiseWeight;

            float randWeight = Random.Range(0f, totalWeight);

            // COMMON NOISE
            if (randWeight < commonNoiseWeight)
            {
                var noise = Manager.Instance.commonNoise[Random.Range(0, Manager.Instance.commonNoise.Length)];
                WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
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
            else
            {
                //Debug.Log("Secondary Trait Triggered!");
                HandleTraitNoiseEvent(secondaryTrait);
            }


            // Reinvoke
            Invoke(TRIGGER_NOISE_EVENT, Random.Range(noiseEventDelay - noiseEventDelayOffset, noiseEventDelay + noiseEventDelayOffset));
        }

        void HandleTraitNoiseEvent(Trait trait)
        {
            AudioClip noise;
            switch (trait)
            {
                case Trait.Clumsy:
                    noise = Manager.Instance.clumsyTraitNoise[Random.Range(0, Manager.Instance.clumsyTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
                    break;

                case Trait.Heavy:
                    noise = Manager.Instance.heavyTraitNoise[Random.Range(0, Manager.Instance.heavyTraitNoise.Length)];
                    VentilationSystem.Instance.OnNewAudioRequest.Invoke(noise);
                    break;

                case Trait.Drooling:
                    noise = Manager.Instance.droolingTraitNoise[Random.Range(0, Manager.Instance.droolingTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
                    break;

                case Trait.Brutal:
                    noise = Manager.Instance.brutalTraitNoise[Random.Range(0, Manager.Instance.brutalTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
                    break;

                case Trait.Speaking:
                    noise = Manager.Instance.speakingTraitNoise[Random.Range(0, Manager.Instance.speakingTraitNoise.Length)];
                    WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
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
            Invoke(TRIGGER_MOVE, Random.Range(moveDelay - moveDelayOffset, moveDelay + moveDelayOffset));
        }

        public bool TryToEvadeDetection()
            => motionDetectionEvasionLimit > 0 && Random.Range(0f, 1f) < motionDetectionEvasionChance;

        public void SetLocation(Room.Controller newLocation)
        {
            if (location != null) location.RemoveOccupant(this);
            location = newLocation;
            location.AddOccupant(this);
        }

        public void GoNextRoom()
        {
            if (location == null) return;
            SetLocation(location.GetNextRoom(favoredRoom));
        }

        public void ScareAway()
        {
            Destroy(gameObject);
        }

        void Attack()
        {
            Gameplay.Manager.Instance.Lose();
        }
    }
}
