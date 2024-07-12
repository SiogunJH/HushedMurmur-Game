using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument uiDocument;

    [Space]
    const string PLAY_BUTTON_NAME = "play-button";
    public UnityEvent onPlayButtonClick;

    const string LEVEL_SELECT_BUTTON_NAME = "level-selection-button";
    public UnityEvent onLevelSelectButtonClick;

    const string SETTINGS_BUTTON_NAME = "options-button";
    public UnityEvent onSettingsButtonClick;

    const string QUIT_BUTTON_NAME = "quit-button";
    public UnityEvent onQuitButtonClick;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        root.Q<Button>(PLAY_BUTTON_NAME).clicked += () => { onPlayButtonClick?.Invoke(); };

        root.Q<Button>(LEVEL_SELECT_BUTTON_NAME).clicked += () => { onLevelSelectButtonClick?.Invoke(); };

        root.Q<Button>(SETTINGS_BUTTON_NAME).clicked += () => { onSettingsButtonClick?.Invoke(); };

        root.Q<Button>(QUIT_BUTTON_NAME).clicked += () => { onQuitButtonClick?.Invoke(); };
    }

    public void Play()
    {
        //
    }

    public void Quit()
    {
        Application.Quit();
    }
}
