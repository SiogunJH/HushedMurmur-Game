using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bird
{
    [System.Serializable]
    public struct NoiseEntry
    {
        public Sounds Sound;
        public bool Enabled;
        public float Weight;

        public NoiseEntry(Sounds sound = Sounds.LoudBreath, bool enabled = true, float weight = 0)
        {
            Sound = sound;
            Enabled = enabled;
            Weight = weight;
        }
    }

    [CustomPropertyDrawer(typeof(NoiseEntry))]
    public class EnabledWeightDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin the property
            EditorGUI.BeginProperty(position, label, property);

            // Remove the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate the positions for each field
            float enumWidth = 150f;
            float labelWidth = 60f;
            float checkboxWidth = 20f;
            float mariginWidth = 5f;
            float offset = 0f;

            var soundRect = new Rect(position.x, position.y, enumWidth, position.height);
            offset += enumWidth + mariginWidth;

            var enabledLabelRect = new Rect(position.x + offset, position.y, labelWidth, position.height);
            offset += labelWidth + mariginWidth;
            var enabledRect = new Rect(position.x + offset, position.y, checkboxWidth, position.height);
            offset += checkboxWidth + mariginWidth;

            var weightLabelRect = new Rect(position.x + offset, position.y, labelWidth, position.height);
            offset += labelWidth + mariginWidth;
            var weightRect = new Rect(position.x + offset, position.y, position.width - offset, position.height);

            // Get the properties
            var soundProp = property.FindPropertyRelative("Sound");
            var enabledProp = property.FindPropertyRelative("Enabled");
            var weightProp = property.FindPropertyRelative("Weight");

            // Draw the 'Sound' enum
            EditorGUI.PropertyField(soundRect, soundProp, GUIContent.none);

            // Draw the 'Enabled' label and checkbox
            EditorGUI.LabelField(enabledLabelRect, "Enabled:");
            EditorGUI.PropertyField(enabledRect, enabledProp, GUIContent.none);

            // Draw the 'Weight' label and field
            EditorGUI.LabelField(weightLabelRect, "Weight:");

            // Disable the 'Weight' field if 'Enabled' is not checked
            GUI.enabled = enabledProp.boolValue;
            EditorGUI.PropertyField(weightRect, weightProp, GUIContent.none);
            GUI.enabled = true; // Re-enable GUI for subsequent controls

            // End the property
            EditorGUI.EndProperty();
        }
    }
}