using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] BirdType birdType;
        [SerializeField, Tooltip("Current room that this bird is in")] Room.Controller location;

        public void SetLocation(Room.Controller newLocation)
        {
            if (location != null) location.RemoveOccupant(this);
            location = newLocation;
            location.AddOccupant(this);

            // Move this object to the location (debugging)
            gameObject.transform.position = location.gameObject.transform.position;
        }

        public void GoNextRoom()
        {
            SetLocation(location.GetNextRoom());
        }

        public void ScareAway()
        {
            Destroy(gameObject);
        }
    }
}
