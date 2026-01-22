using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

public class AnimationSystem : MonoBehaviour
{
    [System.Serializable]
    //this is the easing settings for each property to tween
    public class AnimationSettings
    {
        public Ease ease = Ease.InOutSine;
        public float strength = 0.3f;
        public float extra = 0f;
    }


    [System.Serializable]
    //This is the property to animate and it holds the property name, settings and value
    public class AnimatedProperty
    {

        [HideInInspector] public string propertyName;
        [HideInInspector] public float value;
        public AnimationSettings settings;
        //Store material value state
        [NonSerialized] public Dictionary<string, float> propertyDict;
    }


    [System.Serializable]
    //this is the animation state. 
    //holds the SO data, and the properties to animate
    public class AnimState
    {
        public bool shouldReset = false;
        public string stateName;
        public AnimationStateSO animSO;
        public List<AnimatedProperty> properties;


    }


    //static dotween settings for each property, that cannot dynamically be set, should be global.
    [System.Serializable]
    public class GlobalPropertyTweenSettings
    {
        [HideInInspector] public string propertyName;
        public float duration = 0.2f;
    }

    //Use property name as key to find tween and driver.
    Dictionary<string, TweenDriver> propertyTweenDict;

    //Tween drivers carry the running tweens and their settings.
    public class TweenDriver
    {
        //Tween settings
        internal float duration = 0.2f;
        internal float driver;
        internal Tweener tween;
        internal AnimationSettings settings;
        //data to change when running tween.
        internal float start;
        internal float delta;
        internal string propertyName;
        internal bool reset;

    }

    [Header("Animated Properties")] //In inspector

    //The first animated state, and current active.
    AnimState currentAnimState;
    //Previous animation for reset loops.
    AnimState prevAnimState;


    //Animated states created in inspector
    [SerializeField] List<AnimState> states = new List<AnimState>();

    //Global tween duration.
    public float tweensDuration;

    //The material on the image or object that will be animated
    private Material realMat;

    //the base material reference
    [SerializeField] Material _baseMat;


    [ContextMenu("SetProperties")]
    public void SetProperties()
    {


        //Go through each state in the inspector, set their properties at start or in inspector.
        for (int i = 0; i < states.Count; i++)
        {

            for (int j = 0; j < states[i].animSO.propertyNames.Count; j++)
            {
                AnimState _states = states[i];
                List<AnimatedProperty> _properties = _states.properties;


                if (_states.animSO != null)
                {
                    //If there are missing properties, create one and set it.
                    if (_properties.Count < _states.animSO.propertyNames.Count)
                    {
                        AnimatedProperty addedProp = new AnimatedProperty();

                        _properties.Add(addedProp);
                    }

                    if (_properties[j].propertyName != _states.animSO.propertyNames[j])
                    {
                        AnimatedProperty newProp = new AnimatedProperty();

                        newProp.propertyName = states[i].animSO.propertyNames[j];
                        newProp.settings = new AnimationSettings();

                        _properties[j] = newProp;

                    }

                }


                // Initialize runtime dictionaries from the SO. These dictionaries carry the float values of the property.
                _properties[j].propertyDict = new Dictionary<string, float>();

                string name = _properties[j].propertyName;
                float val = _states.animSO.state.GetFloat(name);

                _properties[j].propertyDict.Add(name, val);



            }
        }
    }


    /*
     The dotween tween is set up at the start. Each tween is contained in a tween driver object.
     The tween lerps its driver value from 0-1. This is multiplied with a delta difference to drive property changes.
    */
    private TweenDriver SetupTween(TweenDriver tweenDriver, float duration)
    {
        tweenDriver.driver = 0f;
        tweenDriver.settings = new AnimationSettings();
        tweenDriver.duration = duration;
        tweenDriver.tween = DOTween
            .To(() => tweenDriver.driver, x => tweenDriver.driver = x, 1f, duration)
            .SetAutoKill(false)
            .Pause()
            .OnUpdate(() =>
            {
                //gets the difference of current material state, and incrementally add it to current value, while lerping the addition. 
                float val = tweenDriver.start + tweenDriver.delta * tweenDriver.driver;
                realMat.SetFloat(tweenDriver.propertyName, val);
            })
            .OnComplete(() =>
            {
                //Make sure tween set to end value;
                realMat.SetFloat(
                  tweenDriver.propertyName,
                 tweenDriver.start + tweenDriver.delta
             );
                //Reset driver, trigger previous state if set to.
                tweenDriver.driver = 0f;
                if (tweenDriver.reset)
                {
                    TriggerAnimate(prevAnimState.animSO);
                }
            });


        return tweenDriver;
    }

    private void Start()
    {
        currentAnimState = states[0];

        //Create a copy of the current material 
        //(If using MPB would change code here not to create instance. This Demo does not use MBP, would use in production to avoid instances.)
        realMat = Instantiate(states[0].animSO.state);

        if (this.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
        {
            renderer.material = realMat;
        }
        if (this.TryGetComponent<Image>(out Image image))
        {
            image.material = realMat;
        }

        SetProperties();

        //Create the dictionary that holds the tween driver references.
        propertyTweenDict = new Dictionary<string, TweenDriver>();

        //Create the tweens for each property that is stored to be animated
        for (int i = 0; i < currentAnimState.animSO.propertyNames.Count; i++)
        {
            TweenDriver newTweener = new TweenDriver();
            propertyTweenDict.Add(currentAnimState.animSO.propertyNames[i], SetupTween(newTweener, tweensDuration));
        }



    }


    public void TriggerAnimate(AnimationStateSO SO)
    {


        for (int i = 0; i < states.Count; i++)
        {
            //Check SO is actually in the list of states to animate.
            //Set previous state, to reset animation later if needed.
            if (SO != states[i].animSO) continue;
            if (currentAnimState != states[i])
            {
                prevAnimState = currentAnimState;
                currentAnimState = states[i];
            }


        }

        for (int j = 0; j < currentAnimState.properties.Count; j++)
        {

            var prop = currentAnimState.properties[j];
            //Send in the animated property object needed for animation. Check if material is different, to not send when not needed. 

            if (realMat.GetFloat(prop.propertyName) != prop.propertyDict[prop.propertyName])
            {
               // Debug.Log($"Animate prop {prop.propertyName}");
                AnimateValue(prop);
            }

        }


    }



    //Get the property settings and value to animate to, and then restart the tween driver's tween.
    private void AnimateValue(AnimatedProperty prop)
    {

        //Get tween driver to animate

        TweenDriver td = propertyTweenDict[prop.propertyName];

        //Start value is current material
        td.start = realMat.GetFloat(prop.propertyName);
        //Delta is difference between start and new, to add to current.
        td.delta = prop.propertyDict[prop.propertyName] - td.start;
        //Other data to update.
        td.propertyName = prop.propertyName;
        td.reset = currentAnimState.shouldReset;



        //Ease settings that new property has from current state.
        td.tween.SetEase(prop.settings.ease, prop.settings.strength, prop.settings.extra);
        if (!td.tween.IsPlaying() && !td.tween.IsComplete())
        {
            // First play of the tween.
            td.tween.Play();
        }

        // Playback when complete.
        if (td.tween.IsComplete())
        {
            //send in current driver value and restart. 
            td.tween.ChangeStartValue(td.driver, 0);
            td.tween.Restart();
        }


    }




}

