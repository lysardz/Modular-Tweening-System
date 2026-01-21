using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AnimationTriggers))]
public class Buttons : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AnimationTriggers triggers = (AnimationTriggers)target;

       
        AnimationSystem animationSystem = triggers.GetComponent<AnimationSystem>();

        if (GUILayout.Button("Trigger Action 1"))
            triggers.Action1();
        if (GUILayout.Button("Trigger Action 2"))
            triggers.Action2();
        if (GUILayout.Button("Trigger Action 3"))
            triggers.Action3();
        if (animationSystem != null && GUILayout.Button("Set State Properties"))
            animationSystem.SetProperties();
    }
}