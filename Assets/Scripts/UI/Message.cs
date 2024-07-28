using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Message : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;

    [Space, SerializeField] InputAction endMessage;

    const string MESSAGE_LABEL = "message-text";

    const string SKIP_LABEL = "skip-text";
    const string SKIP_TEXT = "Press [Q]";

    Label messageLabel;
    Label skipLabel;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        messageLabel = root.Q<Label>(MESSAGE_LABEL);
        skipLabel = root.Q<Label>(SKIP_LABEL);

        Flush();
    }

    public Coroutine Display(string message, params MessageSettings[] additionalSettings) => StartCoroutine(DisplayMessageLetterByLetter(message, additionalSettings));

    IEnumerator DisplayMessageLetterByLetter(string message, params MessageSettings[] additionalSettings)
    {
        if (message == null) yield break;
        Flush();

        const float TIME_BETWEEN_CHARACTERS = 0.02f;
        const float PAUSE_BEFORE_SKIP_PROMPT = 0.5f;

        for (int i = 0; i < message.Length; i++)
        {
            messageLabel.text += message[i];
            yield return new WaitForSeconds(TIME_BETWEEN_CHARACTERS);
        }

        if (!additionalSettings.Contains(MessageSettings.DoNotRequireEndInput))
        {
            yield return new WaitForSeconds(PAUSE_BEFORE_SKIP_PROMPT);
            skipLabel.text = SKIP_TEXT;

            endMessage.Enable();
            yield return new WaitUntil(() => endMessage.triggered);
            endMessage.Disable();
            Flush();
        }
    }

    public void Flush()
    {
        messageLabel.text = "";
        skipLabel.text = "";
    }
}

public enum MessageSettings
{
    DoNotRequireEndInput
}
