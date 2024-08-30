using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour, IBirdkinController
    {
        [Header("Name")]
        [SerializeField] public Type birdType;

        [Header("Traits, Noises and Preferences")]
        [SerializeField] List<NoiseEntry> noiseEventsPool;
        [SerializeField] public Room.Trait[] avoidRoomTraits;
        [SerializeField] public Room.Trait[] favorRoomTraits;

        [Space, SerializeField] public Color repellantColor;

        [Header("Statistics")]
        [Space, SerializeField] public float initialSleepTime = 10;

        [Space, SerializeField] float moveDelay = 60;
        [SerializeField] float moveDelayOffset = 15;

        [Space, SerializeField] public float noiseDelay = 5;
        [SerializeField] public float noiseDelayOffset = 3;

        [HideInInspector] public Room.Controller location;

        [Header("Other")]
        [SerializeField] bool sendDebugInfo;

        public Coroutine MoveCoroutine { get; private set; }
        public Coroutine NoiseCoroutine { get; private set; }

        void Start()
        {
            StartCoroutine(WakeUp());
            if (sendDebugInfo) Debug.Log($"{birdType} has spawned");
        }

        public IEnumerator WakeUp()
        {
            yield return new WaitForSeconds(initialSleepTime);
            if (sendDebugInfo) Debug.Log($"{birdType} has woken up");

            MotionDetectionStation.Instance.OnMotionDetectionTrigger?.Invoke(this);

            NoiseCoroutine = StartCoroutine(NoiseLogic());
            MoveCoroutine = StartCoroutine(MoveLogic());
        }

        #region Noise

        public void StopNoiseLogic() => StopCoroutine(NoiseCoroutine);
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

        public void TriggerNoiseEvent()
        {
            var noiseEventsAvailable = noiseEventsPool.Where(noise => noise.Enabled);
            if (noiseEventsAvailable == null || !noiseEventsAvailable.Any()) return;

            float totalWeight = noiseEventsAvailable.Sum(noise => noise.Weight);
            float randWeight = Random.Range(0f, totalWeight);
            AudioClip noise = null;

            foreach (var noiseEvent in noiseEventsAvailable)
            {
                if (randWeight <= noiseEvent.Weight)
                {
                    if (sendDebugInfo) Debug.Log($"{birdType} is playing {noiseEvent.Sound}");
                    noise = Manager.Instance.GetRandomNoise(noiseEvent.Sound);
                    break;
                }
                randWeight -= noiseEvent.Weight;
            }

            WiretappingSetStation.Instance.SpawnAudio(noise, location);
        }

        #endregion

        #region Movement

        public void StopMoveLogic() => StopCoroutine(MoveCoroutine);
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

        public void SetLocation(Room.Controller newLocation)
        {
            if (location != null) location.RemoveOccupant(this);
            location = newLocation;
            location.AddOccupant(this);
        }

        public void GoNextRoom()
        {
            if (location == null) return;
            SetLocation(location.GetNextRoom(avoidRoomTraits, favorRoomTraits));
            MotionDetectionStation.Instance.OnMotionDetectionTrigger?.Invoke(this);
        }

        #endregion

        #region Other

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

        #endregion
    }
}
