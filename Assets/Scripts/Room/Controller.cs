using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                if (roomName.Split(' ').Length != 2) throw new System.ArgumentException($"Room Code can be generated only from a Room Name consisting of two words. Current Room Name '{roomName}' does not match this requirement.");
                return $"{roomName.TrimStart()[0]}{roomName.Split(' ')[1].Length}";
            }
        }

        #endregion

        #region Occupants

        [SerializeField] List<BirdBase> roomOccupants = new();

        public void AddOccupant(BirdBase newOccupant) { roomOccupants.Add(newOccupant); }
        public void RemoveOccupant(BirdBase occupant) { roomOccupants.Remove(occupant); }

        public List<BirdBase> GetOccupants() => roomOccupants;

        #endregion

        #region Properties

        [SerializeField] List<Controller> roomConnectionsForward = new();
        public List<Tag> tags;

        public Controller GetNextRoom()
            => roomConnectionsForward[Random.Range(0, roomConnectionsForward.Count)];

        #endregion
    }
}