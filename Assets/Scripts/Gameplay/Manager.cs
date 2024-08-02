using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class Manager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Message message;

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

        #region OnEnable & OnDisable

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        #endregion

        #region Scene Management

        public const string MAIN_MENU_SCENE_NAME = "Main Menu";
        public const string LEVEL_0_SCENE_NAME = "Level 0 - Lab Grounds";
        public const string LEVEL_2_SCENE_NAME = "Level 2 - Main Office";

        public void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case MAIN_MENU_SCENE_NAME:

                    break;

                case LEVEL_0_SCENE_NAME:
                    StartCoroutine(TutorialSequence());
                    break;

                case LEVEL_2_SCENE_NAME:
                    StartCoroutine(Level2Sequence());
                    break;

                default:
                    Debug.LogError($"Unhandled OnSceneLoaded scene '{scene.name}'. Verify scene name and saved string, you dumb fuck.");
                    break;
            }
        }

        public void Load(string sceneName) => StartCoroutine(LoadSceneCoroutine(sceneName));

        IEnumerator LoadSceneCoroutine(string sceneName)
        {
            yield return BlackScreen.Instance.FadeOut();
            SceneManager.LoadScene(sceneName);
        }

        #endregion

        #region Victory

        public UnityEvent OnVictory;
        [Space, SerializeField] TextMeshProUGUI winMessage;

        public void Win()
        {
            winMessage.gameObject.SetActive(true);
            Invoke("Reload", 10);
        }

        #endregion

        #region Defeat

        public UnityEvent OnDefeat;
        [Space, SerializeField] TextMeshProUGUI loseMessage;

        public void Lose()
        {
            loseMessage.gameObject.SetActive(true);
            Invoke("Reload", 10);
        }

        #endregion

        #region Conditionals

        Func<bool> IsPlayerLookingAt(string objectsName, float minRotationDifferenceAllowed = 0, float maxRotationDifferenceAllowed = 360)
        {
            return () =>
            {
                var objectLookedAt = PlayerLook.Instance.GetObjectLookedAt();
                if (objectLookedAt == null) return false;

                if (objectLookedAt.name != objectsName) return false;

                float playerToObjectRotationDifference = Mathf.Abs(PlayerLook.Instance.yRotation - objectLookedAt.transform.eulerAngles.y);
                return playerToObjectRotationDifference > minRotationDifferenceAllowed && playerToObjectRotationDifference < maxRotationDifferenceAllowed;
            };
        }

        Func<bool> IsWiretappingSetTunedTo(string expectedRoomCode)
        {
            return () => WiretappingSetStation.Instance.WiretappedRoomCode == expectedRoomCode;
        }

        Func<bool> IsWiretappingSetTurnedOn()
        {
            return () => WiretappingSetStation.Instance.IsActive;
        }

        #endregion

        #region Gamemodes

        IEnumerator TutorialSequence()
        {
            /*/ 
                --===========--
                --== SETUP ==-- 
                --===========--
            /*/

            // Consts
            const string MAP = "Poster";

            const string ALCHEMY_TABLE = "PBR_Table";

            const string POSTER_RAVEN = "Poster (Raven)";
            const string POSTER_CROW = "Poster (Crow)";
            const string POSTER_OWL = "Poster (Owl)";

            const string SYRINGE_RAVEN = "Syringe (Raven)";

            const string MONITOR = "Old Monitor";
            const float MONITOR_ROTATION_DIFFERENCE_MIN = 150;
            const float MONITOR_ROTATION_DIFFERENCE_MAX = 210;

            const float TUTORIAL_STEP_BUFFER_SHORT = 0.8f;
            const float TUTORIAL_STEP_BUFFER_AVERAGE = 1.6f;
            const float TUTORIAL_STEP_BUFFER_LONG = 2.4f;

            const int AMOUNT_OF_COMMON_NOISE_EVENTS_IN_SET = 5;
            const int AMOUNT_OF_UNIQUE_NOISE_EVENTS_IN_SET = 2;

            // Vars
            List<AudioClip> noiseSet = new();

            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            /*/ 
                --=================--
                --== LOOK AROUND ==-- 
                --=================--
            /*/

            yield return message.Display("Use [Mouse] to look around", MessageSettings.DoNotRequireEndInput);

            // Get initial rotation info
            float xRotationInitial = PlayerLook.Instance.xRotation;
            float yRotationInitial = PlayerLook.Instance.yRotation;
            const float MOUSE_BOX_SIZE = 40;

            // Wait until player moves mouse out of the "box"
            yield return new WaitUntil(() =>
            {
                float xRotation = PlayerLook.Instance.xRotation;
                float yRotation = PlayerLook.Instance.yRotation;
                return !
                (
                    xRotation < xRotationInitial + MOUSE_BOX_SIZE / 2 &&
                    xRotation > xRotationInitial - MOUSE_BOX_SIZE / 2 &&
                    yRotation < yRotationInitial + MOUSE_BOX_SIZE / 2 &&
                    yRotation > yRotationInitial - MOUSE_BOX_SIZE / 2
                );
            });

            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            /*/ 
                --=================--
                --== WALK AROUND ==-- 
                --=================--
            /*/

            yield return message.Display("Use [WSAD] or [Arrows] to walk around", MessageSettings.DoNotRequireEndInput);

            // Get initial position info
            float xPositionInitial = PlayerMovement.Instance.gameObject.transform.position.x;
            float zPositionInitial = PlayerMovement.Instance.gameObject.transform.position.z;
            const float WALK_BOX_SIZE = 2;

            // Wait until player moves out of the "box"
            yield return new WaitUntil(() =>
            {
                float xPosition = PlayerMovement.Instance.gameObject.transform.position.x;
                float zPosition = PlayerMovement.Instance.gameObject.transform.position.z;
                return !
                (
                    xPosition < xPositionInitial + WALK_BOX_SIZE / 2 &&
                    xPosition > xPositionInitial - WALK_BOX_SIZE / 2 &&
                    zPosition < zPositionInitial + WALK_BOX_SIZE / 2 &&
                    zPosition > zPositionInitial - WALK_BOX_SIZE / 2
                );
            });

            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            /*/ 
                --====================--
                --== CHECK COMPUTER ==--
                --    Bird appears    -- 
                --====================--
            /*/

            Bird.Controller tutorialBird = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            tutorialBird.initialSleepTime = 0;
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);
            tutorialBird.StopMoveLogic(); // Move logic doesn't start until one frame after spawning the bird, that's why it's being stopped after a while, and not right away
            tutorialBird.StopNoiseLogic(); // Same as above

            yield return message.Display("The Motion Detection System picked up some movement");
            yield return message.Display("Walk up to the computer and check the motion alert", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MONITOR, MONITOR_ROTATION_DIFFERENCE_MIN, MONITOR_ROTATION_DIFFERENCE_MAX));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            /*/ 
                --===============--
                --== CHECK MAP ==-- 
                --===============--
            /*/

            yield return message.Display($"There was a motion in room {tutorialBird.location.roomCode}");
            yield return message.Display($"Check the Map for the precise location of the room {tutorialBird.location.roomCode}", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MAP));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            /*/ 
                --==================--
                --== WIRETAP ROOM ==--
                --    Main Trait    --
                --==================--
            /*/

            yield return message.Display($"The position of the Bird is now known - you can use a Wiretapping Set to eavesdrop on it");
            yield return message.Display($"Walk up to the Wiretapping Set and select room {tutorialBird.location.roomCode}, by interacting with buttons with [E]", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsWiretappingSetTunedTo(tutorialBird.location.roomCode));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return message.Display($"Pick up the Headset with [E], to begin eavesdropping", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsWiretappingSetTurnedOn());
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return message.Display($"When eavesdropping, listen for unique sounds that could indicate certain characteristics, like clumsiness or aggravated aggression", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            // Create a set of noises and play all of them
            for (int i = 0; i < AMOUNT_OF_COMMON_NOISE_EVENTS_IN_SET; i++) noiseSet.Add(Bird.Manager.Instance.commonNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.commonNoise.Length)]);
            for (int i = 0; i < AMOUNT_OF_UNIQUE_NOISE_EVENTS_IN_SET; i++) noiseSet.Add(Bird.Manager.Instance.clumsyTraitNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.clumsyTraitNoise.Length)]);
            while (noiseSet.Count != 0)
            {
                int index = UnityEngine.Random.Range(0, noiseSet.Count);
                yield return WiretappingSetStation.Instance.PlayAudio(noiseSet[index], tutorialBird.location).coroutine;
                noiseSet.RemoveAt(index);

                float delay = UnityEngine.Random.Range(tutorialBird.noiseDelay - tutorialBird.noiseDelayOffset, tutorialBird.noiseDelay + tutorialBird.noiseDelayOffset);
                yield return new WaitForSeconds(delay);
            }
            // Always finish with unique
            yield return WiretappingSetStation.Instance.PlayAudio(Bird.Manager.Instance.clumsyTraitNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.clumsyTraitNoise.Length)], tutorialBird.location).coroutine;

            yield return message.Display($"Notice the sound of breaking glass - this could indicate the clumsiness of the Bird in the room {tutorialBird.location.roomCode}");

            /*/ 
                --====================--
                --== CHECK COMPUTER ==--
                --     Bird moves     --
                --====================--
            /*/

            tutorialBird.GoNextRoom();
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            yield return message.Display("The Motion Detection System just sent a new alert");
            yield return message.Display("This could mean, that the Bird you're spying at have moved");
            yield return message.Display("Put down the headset by interacting with a stool with [E], and walk up to the computer to check the motion alert", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MONITOR, MONITOR_ROTATION_DIFFERENCE_MIN, MONITOR_ROTATION_DIFFERENCE_MAX));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            /*/ 
                --===============--
                --== CHECK MAP ==-- 
                --===============--
            /*/

            yield return message.Display($"The motion was detected in {tutorialBird.location.roomCode}");
            yield return message.Display($"Check the map for the precise location of the room {tutorialBird.location.roomCode}", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MAP));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            /*/ 
                --==================--
                --== WIRETAP ROOM ==--
                -- Secondary Trait  --
                --==================--
            /*/

            yield return message.Display($"This room is much closer to the office you're in - if the Bird ever reaches your room, they will kill you");
            yield return message.Display($"This Bird seems to have skipped a room while searching for you - you can use this knowledge to your advantage");
            yield return message.Display($"Each Bird have a favored room, which they will never omit, and all rooms are indentifiable by the letter in their code");
            yield return message.Display($"Letter '{tutorialBird.location.roomCode[0]}' in room code {tutorialBird.location.roomCode} means that the room is a Greenhouse");
            yield return message.Display($"Same goes for other room codes - 'K' means Kitchen, 'W' means Workshop, and so on");
            yield return message.Display($"Return to the Wiretapping Set and select room {tutorialBird.location.roomCode} to continue spying", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsWiretappingSetTunedTo(tutorialBird.location.roomCode));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return message.Display($"Pick up the Headset, to resume eavesdropping", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsWiretappingSetTurnedOn());
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return message.Display($"Listen for unique sounds", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            // Create a set of noises and play all of them
            for (int i = 0; i < AMOUNT_OF_COMMON_NOISE_EVENTS_IN_SET; i++) noiseSet.Add(Bird.Manager.Instance.commonNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.commonNoise.Length)]);
            for (int i = 0; i < AMOUNT_OF_UNIQUE_NOISE_EVENTS_IN_SET; i++) noiseSet.Add(Bird.Manager.Instance.speakingTraitNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.speakingTraitNoise.Length)]);
            while (noiseSet.Count != 0)
            {
                int index = UnityEngine.Random.Range(0, noiseSet.Count);
                yield return WiretappingSetStation.Instance.PlayAudio(noiseSet[index], tutorialBird.location).coroutine;
                noiseSet.RemoveAt(index);

                float delay = UnityEngine.Random.Range(tutorialBird.noiseDelay - tutorialBird.noiseDelayOffset, tutorialBird.noiseDelay + tutorialBird.noiseDelayOffset);
                yield return new WaitForSeconds(delay);
            }
            // Always finish with unique
            yield return WiretappingSetStation.Instance.PlayAudio(Bird.Manager.Instance.speakingTraitNoise[UnityEngine.Random.Range(0, Bird.Manager.Instance.speakingTraitNoise.Length)], tutorialBird.location).coroutine;

            yield return message.Display($"The threat attempted to speak, although the words were indistinguishable - this could indicate the chatterbox-like tendencies of the Bird in the room");

            /*/ 
                --====================--
                --== CHECK COMPUTER ==--
                --   Bird is close    --
                --====================--
            /*/

            tutorialBird.GoNextRoom();
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            yield return message.Display("Another motion alert was sent - check the computer", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MONITOR, MONITOR_ROTATION_DIFFERENCE_MIN, MONITOR_ROTATION_DIFFERENCE_MAX));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            /*/ 
                --===============--
                --== CHECK MAP ==-- 
                --===============--
            /*/

            yield return message.Display($"The motion was detected in {tutorialBird.location.roomCode}");
            yield return message.Display($"Check the map for the precise location of the room {tutorialBird.location.roomCode}", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(MAP));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            /*/ 
                --===================--
                --== CHECK POSTERS ==-- 
                --===================--
            /*/

            yield return message.Display($"Room {tutorialBird.location.roomCode} is adjacent to the office you're in");
            yield return message.Display($"This makes the Bird vulnerable, but in order to attack it, you need to know what type of a Bird you're facing");
            yield return message.Display($"Walk up to the right-most poster, that's pinned on a wall behind Motion Detection System, and analyze it", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(POSTER_OWL));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            yield return message.Display($"Top-most line, is the name of a Bird. An Owl type should be Clumsy and Brutal, with a favored room being a Workshop");
            yield return message.Display($"Although the Bird you spied on was Clumsy, they didn't appear to show any aggravated aggresion signs, nor visited any Workshops");
            yield return message.Display($"In conclusion, the Owl type doesn't match the description of a Bird in the building");
            yield return message.Display($"Walk up to the middle poster, and analyze it", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(POSTER_CROW));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            yield return message.Display($"A Crow type should be Clumsy and Speaking, with a favored room being a Pantry");
            yield return message.Display($"The Bird you spied on was both Clumsy and Speaking, but they didn't visit any Pantries");
            yield return message.Display($"This means - just like with an Owl type - the Bird in the building isn't a Crow type");
            yield return message.Display($"Walk up to the left-most poster, and analyze it", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(POSTER_RAVEN));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_LONG);

            yield return message.Display($"A Raven type - just like Crow - should be Clumsy and Speaking, but with a favored room being a Greenhouse");
            yield return message.Display($"The Bird you spied on visited two rooms which codes started with a letter 'G', indicating that those were Greenhouses - Raven's favored rooms");
            yield return message.Display($"This means that the Bird in the building must be of a Raven type");
            yield return message.Display($"Now that you've established Bird type, it's time to attack it");
            yield return message.Display($"Notice the small colored text below Bird's name on each of the posters");
            yield return message.Display($"This color hints on the elixir that should be used to defeat a Bird - the Raven's elixir should be green");

            /*/ 
                --=====================--
                --== INJECT YOURSELF ==-- 
                --=====================--
            /*/

            yield return message.Display($"Walk up to the Alchemy Table, next to the door", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(IsPlayerLookingAt(ALCHEMY_TABLE));
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return message.Display($"Each Syringe is one-use only, so you need to use them sparingly, and only when you're certain of a Bird type");
            yield return message.Display($"Pick up a Syringe with green elixir, which will repel Birds of a Raven type, by interacting with it with [E]", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            GameObject syringeGameObject = GameObject.Find(SYRINGE_RAVEN);
            syringeGameObject.GetComponent<Interactable>().disabled = false;

            Syringe syringe = syringeGameObject.GetComponent<Syringe>();
            yield return new WaitUntil(() => !syringe.IsFull);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            /*/ 
                --=============--
                --== SCREECH ==-- 
                --=============--
            /*/

            yield return message.Display($"While facing the dark corridor leading to your office, hold down [Space] to take a deep breath, and then release the button to create a loud shriek, that will repel the Raven", MessageSettings.DoNotRequireEndInput);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_SHORT);

            yield return new WaitUntil(() => tutorialBird == null);
            yield return new WaitForSeconds(TUTORIAL_STEP_BUFFER_AVERAGE);

            /*/ 
                --===============--
                --== COMPLETED ==--
                --===============--
            /*/

            yield return message.Display($"You survive, by scaring away all Birds present in the building");
            yield return message.Display($"This is the end of a tutorial sequence - since now, you're on your own");
            yield return message.Display($"You can exit to Main Menu by holding down [Esc] for a second. Good luck", MessageSettings.DoNotRequireEndInput);
        }

        IEnumerator Level2Sequence()
        {
            // Setup
            const float INITIAL_IDLE_TIME = 10;
            const float BIRD_ONE_SLEEP_TIME = 20;
            const float BIRD_TWO_SLEEP_TIME = 110;

            yield return new WaitForSeconds(INITIAL_IDLE_TIME);

            // Bird One

            var birdOne = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdOne.initialSleepTime = BIRD_ONE_SLEEP_TIME;

            // Bird Two

            var birdTwo = Bird.Manager.Instance.SpawnBird().GetComponent<Bird.Controller>();
            birdTwo.initialSleepTime = BIRD_TWO_SLEEP_TIME;
        }

        #endregion

        public void ChangeSensitivity(float value)
        {
            PlayerLook.mouseXSensitivity = Mathf.Clamp(PlayerLook.mouseXSensitivity + value, 0.1f, 100);
            PlayerLook.mouseYSensitivity = Mathf.Clamp(PlayerLook.mouseYSensitivity + value, 0.1f, 100);
        }
    }
}
