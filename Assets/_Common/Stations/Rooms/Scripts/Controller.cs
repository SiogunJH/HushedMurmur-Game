using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Room
{
    public class Controller : MonoBehaviour
    {
        void Start()
        {
            Initialize();
        }

        #region Naming

        [SerializeField] TextMeshProUGUI codeDisplay;
        [HideInInspector] public string roomCode = "X0";

        void Initialize()
        {
            roomCode = Manager.Instance.GenerateUniqueRoomCode(this);
            Manager.Instance.AddToRoomPool(this);
            UpdateCodeDisplay();
        }

        void UpdateCodeDisplay()
        {
            codeDisplay.text = roomCode;
        }

        #endregion

        #region Occupants

        List<Bird.Controller> roomOccupants = new();

        public void AddOccupant(Bird.Controller newOccupant) { roomOccupants.Add(newOccupant); }
        public void RemoveOccupant(Bird.Controller occupant) { roomOccupants.Remove(occupant); }

        public List<Bird.Controller> GetOccupants() => roomOccupants;

        #endregion

        #region Properties

        [SerializeField] List<Controller> roomConnectionsForward = new();
        public List<Tag> tags;
        public List<Trait> traits;

        public Controller GetNextRoom(Trait[] traitsToAvoid = null, Trait[] traitsToFavor = null)
        {
            List<Controller> roomPool = roomConnectionsForward;

            // Remove rooms with traits to avoid
            if (traitsToAvoid != null)
                foreach (var trait in traitsToAvoid)
                    roomPool = roomPool.Where(room => !room.traits.Contains(trait)).ToList();

            // Focus on rooms that have traits to favor, if any exist
            List<Controller> favoredRooms = new();
            if (traitsToFavor != null)
                foreach (Controller roomController in roomPool)
                    foreach (Trait traitToFavor in traitsToFavor)
                        if (!favoredRooms.Contains(roomController) && roomController.traits.Contains(traitToFavor))
                        {
                            favoredRooms.Add(roomController);
                            break;
                        }

            if (favoredRooms.Any()) roomPool = favoredRooms;

            // Null/Empty safety
            if (roomPool == null || roomPool.Count == 0 || !roomPool.Any())
            {
                Debug.LogError($"[Dead End] error in room {roomCode} with traits [{System.String.Join(", ", traits)}] - no connections forward that allow to avoid {System.String.Join(", ", traitsToAvoid)} and favor {System.String.Join(", ", traitsToFavor)}!");
                return null;
            }

            // Pick a room from pool
            return roomPool[Random.Range(0, roomPool.Count)];
        }

        #endregion
    }
}