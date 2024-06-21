using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bird
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] BirdType birdType;
        [SerializeField, Tooltip("Current room that this bird is in")] Room.Controller location;

        public void GoNextRoom()
        {
            location.RemoveOccupant(this);
            location = location.GetNextRoom();
            location.AddOccupant(this);
        }

        public void ScareAway()
        {
            Destroy(gameObject);
        }
    }
}
