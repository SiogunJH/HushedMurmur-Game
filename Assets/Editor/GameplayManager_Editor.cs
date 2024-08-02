// using UnityEngine;
// using UnityEditor;

// //[CustomEditor(typeof(Gameplay.Manager))]
// public class GameplayManager_Editor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // Draw the default inspector, preserving existing fields and their values
//         DrawDefaultInspector();

//         // Get a reference to the Manager script
//         Gameplay.Manager manager = (Gameplay.Manager)target;

//         // Add a button to the inspector
//         if (GUILayout.Button("Easy"))
//         {
//             if (Application.isPlaying)
//             {
//                 manager.Easy();
//             }
//             else
//             {
//                 Debug.LogWarning("Can't start when the editor is not playing.");
//             }
//         }
//     }
// }
