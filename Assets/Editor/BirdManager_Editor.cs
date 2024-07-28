using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Bird.Manager))]
public class BirdManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector, preserving existing fields and their values
        DrawDefaultInspector();

        // Get a reference to the Manager script
        Bird.Manager manager = (Bird.Manager)target;

        // Add a button to the inspector
        if (GUILayout.Button("Spawn Bird"))
        {
            if (Application.isPlaying)
            {
                manager.SpawnBird();
            }
            else
            {
                Debug.LogWarning("Can't spawn new Bird object when the editor is not playing.");
            }
        }
    }
}
