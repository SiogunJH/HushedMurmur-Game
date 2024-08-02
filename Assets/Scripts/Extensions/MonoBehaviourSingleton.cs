using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour, ISingleton
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        AssignSingleton();
    }

    public void AssignSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}