using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BlackScreen : MonoBehaviourSingleton<BlackScreen>, ISingleton
{
    [SerializeField] UIDocument uiDocument;

    const string BLACK_SCREEN_VISUAL_ELEMENT_NAME = "black-screen";
    VisualElement blackScreen;

    public const float NON_TRANSPARENT_OPACITY = 1;
    public const float TRANSPARENT_OPACITY = 0;

    const float FADE_DURATION = .6f;
    const float FADE_DELAY_BEFORE = .25f;
    const float FADE_DELAY_AFTER = .25f;

    protected override void Awake()
    {
        base.Awake();

        var root = uiDocument.rootVisualElement;
        blackScreen = root.Q<VisualElement>(BLACK_SCREEN_VISUAL_ELEMENT_NAME);

        SetOpacity(NON_TRANSPARENT_OPACITY);
        FadeIn();

    }

    void SetOpacity(float opacity)
    {
        Color initialColor = blackScreen.style.backgroundColor.value;
        blackScreen.style.backgroundColor = new StyleColor(new Color(initialColor.r, initialColor.g, initialColor.b, opacity));
    }

    IEnumerator Fade(float targetOpacity, float duration, float delayBefore, float delayAfter)
    {
        yield return new WaitForSeconds(delayBefore);

        Color initialColor = blackScreen.style.backgroundColor.value;
        float initialOpacity = initialColor.a;

        float totalDuration = duration;
        float progress = 0;

        while (progress != 1)
        {
            duration -= Time.deltaTime;
            progress = Mathf.Clamp01(1 - duration / totalDuration);

            float newOpacity = Mathf.Lerp(initialOpacity, targetOpacity, progress);
            blackScreen.style.backgroundColor = new StyleColor(new Color(initialColor.r, initialColor.g, initialColor.b, newOpacity));

            yield return null;
        }

        blackScreen.style.backgroundColor = new StyleColor(new Color(initialColor.r, initialColor.g, initialColor.b, targetOpacity));

        yield return new WaitForSeconds(delayAfter);
    }

    public Coroutine FadeIn() => StartCoroutine(Fade(TRANSPARENT_OPACITY, FADE_DURATION, FADE_DELAY_BEFORE, 0));
    public Coroutine FadeOut() => StartCoroutine(Fade(NON_TRANSPARENT_OPACITY, FADE_DURATION, 0, FADE_DELAY_AFTER));
}
