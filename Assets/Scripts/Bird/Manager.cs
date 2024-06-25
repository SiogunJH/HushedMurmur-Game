using System.Linq;
using UnityEngine;

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

            PreStart();
        }

        #endregion

        #region Birds

        [Space, Header("Bird Prefabs")]
        [SerializeField] GameObject[] birdPrefabs;

        private Controller[] allBirds;
        private const string BIRD_OBJECT_TAG = "Bird";

        void PreStart()
        {
            allBirds = GameObject.FindGameObjectsWithTag(BIRD_OBJECT_TAG)
                .Select(obj => obj.GetComponent<Controller>())
                .Where(component => component != null)
                .ToArray();
        }

        #endregion

        void Start()
        {
            Room.Controller[] startingRooms = Room.Manager.Instance.GetRoomsByTag(Room.Tag.Start);

            // Assign birds to rooms
            foreach (var bird in allBirds)
            {
                bird.SetLocation(startingRooms[Random.Range(0, startingRooms.Length)]);
            }
        }

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