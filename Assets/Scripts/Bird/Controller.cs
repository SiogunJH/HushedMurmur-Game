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

        public (int Common, int Global, int MainTrait, int SecondaryTrait) NoiseEventWeight { get; set; } = (120, 0, 15, 10);

        [Header("Statistics")]
        [Space, SerializeField] public float initialSleepTime = 10;

        [Space, SerializeField] float moveDelay = 60;
        [SerializeField] float moveDelayOffset = 15;

        [Space, SerializeField] public float noiseDelay = 5;
        [SerializeField] public float noiseDelayOffset = 3;

        [Space, SerializeField] float motionDetectionEvasionChance = 0f;
        [SerializeField] public float motionDetectionEvasionLimit = 3;

        [HideInInspector] public Room.Controller location;

        // Coroutines
        public Coroutine MoveCoroutine { get; private set; }
        public Coroutine NoiseCoroutine { get; private set; }

        void Start()
        {
            Invoke(WAKE_UP, initialSleepTime);
        }

        public const string WAKE_UP = "WakeUp";
        public void WakeUp()
        {
            MotionDetectionStation.Instance.OnMotionDetectionTrigger?.Invoke(this);

            NoiseCoroutine = StartCoroutine(NoiseLogic());
            MoveCoroutine = StartCoroutine(MoveLogic());
        }

        IEnumerator NoiseLogic()
        {
            while (true)
            {
                // Wait for the next noise delay
                var delay = Random.Range(noiseDelay - noiseDelayOffset, noiseDelay + noiseDelayOffset);
                yield return new WaitForSeconds(delay);

                // Trigger noise
                TriggerNoiseEvent();
            }
        }

        public void StopNoiseLogic() => StopCoroutine(NoiseCoroutine);

        public void TriggerNoiseEvent()
        {
            int totalWeight = NoiseEventWeight.MainTrait + NoiseEventWeight.SecondaryTrait + NoiseEventWeight.Common + NoiseEventWeight.Global;

            float randWeight = Random.Range(0f, totalWeight);
            AudioClip noise;

            // COMMON NOISE
            if (randWeight < NoiseEventWeight.Common)
            {
                noise = Manager.Instance.commonNoise[Random.Range(0, Manager.Instance.commonNoise.Length)];
                WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
            }

            // GLOBAL NOISE
            else if (randWeight - NoiseEventWeight.Common < NoiseEventWeight.Global)
            {
                noise = Manager.Instance.globalNoise[Random.Range(0, Manager.Instance.globalNoise.Length)];
                WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
                // VentilationSystem.Instance.OnNewAudioRequest.Invoke(noise);
            }

            // MAIN TRAIT
            else if (randWeight - NoiseEventWeight.Common - NoiseEventWeight.Global < NoiseEventWeight.MainTrait)
                HandleTraitNoiseEvent(mainTrait);

            // SECONDARY TRAIT
            else
            {
                HandleTraitNoiseEvent(secondaryTrait);
            }
        }

        AudioClip HandleTraitNoiseEvent(Trait trait)
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
                    WiretappingSetStation.Instance.OnNewAudioRequest.Invoke(noise, location);
                    // VentilationSystem.Instance.OnNewAudioRequest.Invoke(noise);
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
                    noise = null;
                    Debug.LogWarning($"Unhandled trait: {trait}");
                    break;

            }
            return noise;
        }

        IEnumerator MoveLogic()
        {
            while (true)
            {
                // Wait for the next move delay
                var delay = Random.Range(moveDelay - moveDelayOffset, moveDelay + moveDelayOffset);
                yield return new WaitForSeconds(delay);

                // If right before the office, attempt to attack
                if (location.tags.Contains(Room.Tag.End))
                {
                    Attack();
                    yield break;
                }

                // Move to the next room
                GoNextRoom();
            }
        }

        public void StopMoveLogic() => StopCoroutine(MoveCoroutine);

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
            MotionDetectionStation.Instance.OnMotionDetectionTrigger.Invoke(this);
        }

        public void ScareAway()
        {
            if (location != null) location.RemoveOccupant(this);
            Manager.Instance.RemoveBird(gameObject);
            Destroy(gameObject);
        }

        void Attack()
        {
            Gameplay.Manager.Instance.OnDefeat?.Invoke();
        }
    }
}
