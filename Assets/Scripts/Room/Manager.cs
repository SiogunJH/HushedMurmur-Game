using System.Linq;
using UnityEngine;

namespace Room
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

        private Controller[] allRooms;
        private const string ROOM_OBJECT_TAG = "Room";

        void PreStart()
        {
            allRooms = GameObject.FindGameObjectsWithTag(ROOM_OBJECT_TAG)
                .Select(obj => obj.GetComponent<Controller>())
                .Where(component => component != null)
                .ToArray();
        }

        public Controller[] GetRoomsByTag(Tag tag)
            => allRooms.Where(room => room.tags.Contains(tag)).ToArray();

        public Controller[] GetUnoccupiedRooms()
            => allRooms.Where(room => room.GetOccupants().Count == 0).ToArray();
    }
}