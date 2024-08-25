using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Room
{
    public class Controller : MonoBehaviour
    {
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            roomCode = Manager.Instance.GenerateUniqueRoomCode();
            Manager.Instance.AddToRoomPool(this);
            UpdateCodeDisplay();
            PlayAmbient();
        }

        #region Naming

        [SerializeField] TextMeshProUGUI codeDisplay;
        [HideInInspector] public string roomCode = "X0";

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

        #region Room Traits & Connections

        [Space, SerializeField] List<Controller> roomConnectionsForward = new();
        [SerializeField] public List<Tag> tags;
        [SerializeField] public List<Trait> traits;

        public Controller GetNextRoom(Trait[] traitsToAvoid, Trait[] traitsToFavor)
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

        #region Ambient

        [Space, SerializeField] AudioClip roomAmbient;
        void PlayAmbient(bool loop = true)
        {
            if (roomAmbient == null) return;
            var independentAudio = WiretappingSetStation.Instance.SpawnAudio(roomAmbient, this);
            independentAudio.audioSource.loop = loop;
        }

        #endregion
    }
}