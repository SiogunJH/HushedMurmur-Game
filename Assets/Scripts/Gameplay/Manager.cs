using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
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

        public void Easy()
        {
            var birdOne = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdOne.initialSleepTime = 30;

            var birdTwo = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdTwo.initialSleepTime = 120;
        }

        public void Lose()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
