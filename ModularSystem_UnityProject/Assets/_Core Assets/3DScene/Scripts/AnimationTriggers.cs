using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

//Basic trigger script for animation demo testing
public class AnimationTriggers : MonoBehaviour
{
    public AnimationSystem animationScript;
    public AnimationStateSO action1;
    public AnimationStateSO action2;
    public AnimationStateSO action3;


    public void Action1()
    {
        
        animationScript.TriggerAnimate(action1);
    }

    public void Action2()
    {
        animationScript.TriggerAnimate(action2);
    }
    public void Action3()
    {
        animationScript.TriggerAnimate(action3);
    }

}
