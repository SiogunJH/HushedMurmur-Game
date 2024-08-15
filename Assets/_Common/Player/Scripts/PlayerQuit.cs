using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerQuit : MonoBehaviour
{
    float quittingTimeInSeconds = 1;
    Coroutine quittingCoroutine;

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            quittingCoroutine = StartCoroutine(TryToQuit());
        }
        else if (context.canceled)
        {
            StopCoroutine(quittingCoroutine);
        }
    }

    IEnumerator TryToQuit()
    {
        yield return new WaitForSeconds(quittingTimeInSeconds);
        Gameplay.Manager.Instance.Load(Gameplay.Manager.MAIN_MENU_SCENE_NAME);
    }
}
