using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;

    const string PLAY_BUTTON_NAME = "play-button";
    const string LEVEL_SELECT_BUTTON_NAME = "level-selection-button";
    const string TUTORIAL_BUTTON_NAME = "tutorial-button";
    const string SETTINGS_BUTTON_NAME = "options-button";
    const string QUIT_BUTTON_NAME = "quit-button";

    void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        root.Q<Button>(PLAY_BUTTON_NAME).clicked += Play;
        root.Q<Button>(TUTORIAL_BUTTON_NAME).clicked += Tutorial;
        root.Q<Button>(SETTINGS_BUTTON_NAME).clicked += Settings;
        root.Q<Button>(QUIT_BUTTON_NAME).clicked += Quit;
    }

    public void Play()
    {
        Gameplay.Manager.Instance.Load(Gameplay.Manager.LEVEL_2_SCENE_NAME);
    }

    public void Tutorial()
    {
        Gameplay.Manager.Instance.Load(Gameplay.Manager.LEVEL_0_SCENE_NAME);
    }

    public void Settings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
