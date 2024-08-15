using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MiddleScreenMarker : MonoBehaviour
{
    [Space]
    public UIDocument uiDocument;
    public string markerElementID;
    [Space]
    public Sprite markerUnfocused;
    public Sprite markerFocused;

    VisualElement markerVisualElement;
    bool isMarkerFocused = false;

    #region Singleton

    public static MiddleScreenMarker Instance { get; private set; }

    void Awake()
    {
        // Destroy self, if object of this class already exists
        if (Instance != null) Destroy(gameObject);

        //One time setup of a Singleton
        else Instance = this;
    }

    #endregion

    void Start()
    {
        markerVisualElement = uiDocument.rootVisualElement.Q<VisualElement>(markerElementID);

        if (markerVisualElement == null)
            Debug.LogError($"Marker Visual Element is invalid!\nWas expecting Visual Element of ID '{markerElementID}' but recieved null.");
    }

    void FocusMarker()
    {
        if (!isMarkerFocused)
        {
            isMarkerFocused = true;
            markerVisualElement.style.backgroundImage = new StyleBackground(markerFocused);
        }
    }

    void UnfocusMarker()
    {
        if (isMarkerFocused)
        {
            isMarkerFocused = false;
            markerVisualElement.style.backgroundImage = new StyleBackground(markerUnfocused);
        }
    }

    public void UpdateMarker()
    {
        var objectLookedAt = PlayerLook.Instance.GetObjectLookedAt();
        if (objectLookedAt == null)
        {
            UnfocusMarker();
            return;
        }

        var interactable = objectLookedAt.GetComponent<Interactable>();

        if (interactable == null) UnfocusMarker();
        else FocusMarker();
    }
}
