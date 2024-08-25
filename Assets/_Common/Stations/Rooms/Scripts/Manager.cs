using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Room
{
    public class Manager : MonoBehaviourSingleton<Manager>, ISingleton
    {
        protected override void Awake()
        {
            base.Awake();

            GenerateValidStep();
            currentID = Random.Range(0, MaxRooms);
        }

        #region Room ID

        int MaxRooms { get => codeCharactersAvailable_Slot1.Length * codeCharactersAvailable_Slot2.Length; }
        HashSet<Controller> allRooms = new();
        int currentID;
        int step;

        public char[] codeCharactersAvailable_Slot1;
        public char[] codeCharactersAvailable_Slot2;

        public void AddToRoomPool(Controller room) => allRooms.Add(room);

        void GenerateValidStep()
        {
            HashSet<int> validSteps = new();

            for (int i = 0; i < MaxRooms; i++)
                if (MathfExtensions.GCD(i, MaxRooms) == 1)
                    validSteps.Add(i);

            if (!validSteps.Any())
            {
                Debug.LogError($"LCG error! Failed to generate a valid step for {MaxRooms}");
                return;
            }

            step = validSteps.ElementAt(Random.Range(0, validSteps.Count));
        }

        public string GenerateUniqueRoomCode()
        {
            if (allRooms.Count >= MaxRooms)
            {
                Debug.LogError($"Room code error! Room pool is full, and new unique room code cannot be generated. This may be due to the amount of room objects exceeding the amount of unique room codes possible");
                return null;
            }

            string roomCode = $"{codeCharactersAvailable_Slot1[currentID / codeCharactersAvailable_Slot1.Length]}{codeCharactersAvailable_Slot2[currentID % codeCharactersAvailable_Slot2.Length]}";
            currentID = (currentID + step) % MaxRooms;
            return roomCode;
        }

        public bool DoesRoomExist(string roomCode) => allRooms.Where(room => room.roomCode == roomCode).Any();

        #endregion

        public Controller[] GetRoomsByTag(Tag tag)
            => allRooms.Where(room => room.tags.Contains(tag)).ToArray();

        public Controller[] GetUnoccupiedRooms()
            => allRooms.Where(room => room.GetOccupants().Count == 0).ToArray();

        public static int roomCodeAmountOfNumbers = 10;
    }
}