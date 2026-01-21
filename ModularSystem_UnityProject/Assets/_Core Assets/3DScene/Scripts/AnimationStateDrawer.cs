using UnityEngine;
using UnityEditor;
using UnityEngine;
using static AnimationSystem;

//Boilerplate code for having names in property lists for visibility. 
[CustomPropertyDrawer(typeof(AnimState))]
public class AnimationStateSettingsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var nameProp = property.FindPropertyRelative("stateName");
        label.text = string.IsNullOrEmpty(nameProp.stringValue)
            ? "Animation State"
            : nameProp.stringValue;

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
[CustomPropertyDrawer(typeof(AnimatedProperty))]
public class PropertySettingsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var nameProp = property.FindPropertyRelative("propertyName");
        label.text = string.IsNullOrEmpty(nameProp.stringValue)
            ? "Animation State"
            : nameProp.stringValue;

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
