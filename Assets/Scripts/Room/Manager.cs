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

        HashSet<Controller> allRooms = new();

        public Controller[] GetRoomsByTag(Tag tag)
            => allRooms.Where(room => room.tags.Contains(tag)).ToArray();

        public Controller[] GetUnoccupiedRooms()
            => allRooms.Where(room => room.GetOccupants().Count == 0).ToArray();

        public static int roomCodeAmountOfNumbers = 10;

        public void GenerateRoomCode(Controller room)
        {
            string newCode;
            char letter = (char)room.type;

            do newCode = $"{letter}{Random.Range(0, roomCodeAmountOfNumbers)}";
            while (allRooms.Select(room => room.roomCode).Contains(newCode));

            room.roomCode = newCode;
            allRooms.Add(room);
        }


    }
}