using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public UnityEvent OnVictory;
        public UnityEvent OnDefeat;


        [SerializeField] TextMeshProUGUI winMessage;
        [SerializeField] TextMeshProUGUI loseMessage;

        public void ChangeSensitivity(float value)
        {
            PlayerLook.mouseXSensitivity = Mathf.Clamp(PlayerLook.mouseXSensitivity + value, 0.1f, 100);
            PlayerLook.mouseYSensitivity = Mathf.Clamp(PlayerLook.mouseYSensitivity + value, 0.1f, 100);
        }

        public void Easy()
        {
            var birdOne = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdOne.initialSleepTime = 30;

            var birdTwo = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdTwo.initialSleepTime = 120;
        }

        public void Lose()
        {
            loseMessage.gameObject.SetActive(true);
            Invoke("Reload", 10);
        }

        public void Win()
        {
            winMessage.gameObject.SetActive(true);
            Invoke("Reload", 10);
        }

        public static void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
