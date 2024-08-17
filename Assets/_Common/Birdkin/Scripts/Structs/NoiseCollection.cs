using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bird
{
    [System.Serializable]
    public struct NoiseCollection
    {
        public Bird.Sounds Sound;
        public AudioClip[] audioClips;
    }
}
