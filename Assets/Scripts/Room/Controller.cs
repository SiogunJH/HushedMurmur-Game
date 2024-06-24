using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class Controller : MonoBehaviour
    {
        #region Naming

        [SerializeField] string roomName = "Not Set";
        public string roomCode
        {
            get
            {
                if (roomName.Split(' ').Length != 2)
                {
                    Debug.LogWarning($"Room Code can be generated only from a Room Name consisting of two words. Current Room Name '{roomName}' does not match this requirement.");
                    return "Room ID Failure";
                }
                return $"{roomName.TrimStart()[0]}{roomName.Split(' ')[1].Length}";
            }
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

        public Controller GetNextRoom()
            => roomConnectionsForward[Random.Range(0, roomConnectionsForward.Count)];

        #endregion
    }
}