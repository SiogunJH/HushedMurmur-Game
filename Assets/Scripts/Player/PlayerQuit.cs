using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerQuit : MonoBehaviour
{
    float quittingTime = 1;

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(TryToQuit());
        }
        else if (context.canceled)
        {
            StopCoroutine(TryToQuit());
        }
    }

    IEnumerator TryToQuit()
    {
        yield return new WaitForSeconds(quittingTime);
        Gameplay.Manager.Instance.Load(Gameplay.Manager.MAIN_MENU_SCENE_NAME);
    }
}
