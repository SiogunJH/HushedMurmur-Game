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
            StartCoroutine(TRY_TO_QUIT);
        }
        else if (context.canceled)
        {
            StopCoroutine(TRY_TO_QUIT);
        }
    }

    const string TRY_TO_QUIT = "TryToQuit";
    IEnumerator TryToQuit()
    {
        yield return new WaitForSeconds(quittingTime);
        Gameplay.Manager.Reload();
    }
}
