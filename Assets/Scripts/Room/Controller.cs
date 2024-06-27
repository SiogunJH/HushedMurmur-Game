using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Room
{
    public class Controller : MonoBehaviour
    {
        #region Naming

        [SerializeField] TextMeshProUGUI codeDisplay;
        [HideInInspector] public string roomCode = "X0";

        void RequestRoomCode()
        {
            Manager.Instance.GenerateRoomCode(this);
            UpdateCodeDisplay();
        }

        void UpdateCodeDisplay()
        {
            codeDisplay.text = roomCode;
        }

        #endregion

        #region Occupants

        [SerializeField] List<Bird.Controller> roomOccupants = new();

        public void AddOccupant(Bird.Controller newOccupant) { roomOccupants.Add(newOccupant); }
        public void RemoveOccupant(Bird.Controller occupant) { roomOccupants.Remove(occupant); }

        public List<Bird.Controller> GetOccupants() => roomOccupants;

        #endregion

        #region Properties

        [SerializeField] List<Controller> roomConnectionsForward = new();
        public List<Tag> tags;
        public Type type;

        public Controller GetNextRoom(Type preferredRoomType)
        {
            if (roomConnectionsForward.Select(room => room.type).Contains(preferredRoomType))
                return roomConnectionsForward.Where(room => room.type == preferredRoomType).ToArray()[0];

            return roomConnectionsForward[Random.Range(0, roomConnectionsForward.Count)];
        }

        #endregion

        void Start()
        {
            RequestRoomCode();
        }
    }
}