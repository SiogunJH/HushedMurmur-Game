using System.Collections;
using System.Collections.Generic;
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
        }

        #endregion

        private Controller[] allRooms;

        void Start()
        {
            allRooms = GetComponents<Controller>();
        }

        public IEnumerable GetRoomsByTag(Tag tag)
            => allRooms.Where(room => room.tags.Contains(tag));

    }
}