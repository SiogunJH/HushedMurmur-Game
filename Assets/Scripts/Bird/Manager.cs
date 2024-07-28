using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Bird
{
    public class Manager : MonoBehaviour
    {
        #region Singleton

        public static Manager Instance { get; private set; }

        void Awake()
        {
            // Destroy self, if object of this class already exists
            if (Instance != null) Destroy(gameObject);

            //One time setup of a Singleton
            else Instance = this;
        }

        #endregion

        #region Birds

        [Space, Header("Bird Prefabs")]
        [SerializeField] GameObject[] birdPrefabs;
        [SerializeField, Tooltip("A Game Object, in which new Birds will spawn")] GameObject birdCage;
        HashSet<GameObject> activeBirds = new();

        public GameObject SpawnBird()
        {
            Room.Controller[] startingRooms = Room.Manager.Instance
                .GetRoomsByTag(Room.Tag.Start)
                .Where(room => room.GetOccupants().Count() == 0)
                .ToArray();
            if (startingRooms.Length == 0)
            {
                Debug.LogError("Could not spawn new Bird - no empty starting rooms available.");
                return null;
            }

            GameObject[] birdPool = birdPrefabs.Where(bird => !activeBirds.Select(x => x.GetComponent<Controller>().birdType).Contains(bird.GetComponent<Controller>().birdType)).ToArray();
            if (birdPool.Length == 0)
            {
                Debug.LogError("Could not spawn new Bird - unique bird prefabs left.");
                return null;
            }

            GameObject spawnedBird = Instantiate(birdPool[Random.Range(0, birdPool.Length)]);
            spawnedBird.transform.SetParent(birdCage.transform);
            spawnedBird.GetComponent<Controller>().SetLocation(startingRooms[Random.Range(0, startingRooms.Length)]);
            activeBirds.Add(spawnedBird);

            return spawnedBird;
        }

        public void RemoveBird(GameObject bird)
        {
            activeBirds.Remove(bird);

            if (activeBirds.Count == 0) Gameplay.Manager.Instance.OnVictory?.Invoke();
        }

        #endregion

        #region Noise Events

        [Space, Header("Noise Event Arrays")]
        public AudioClip[] commonNoise;
        public AudioClip[] globalNoise;
        public AudioClip[] clumsyTraitNoise;
        public AudioClip[] heavyTraitNoise;
        public AudioClip[] droolingTraitNoise;
        public AudioClip[] brutalTraitNoise;
        public AudioClip[] speakingTraitNoise;

        #endregion
    }
}