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
        internal float currentValue;

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

  
    //Material property blocks
    private Renderer targetRenderer;
    private MaterialPropertyBlock mpb;

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
                        AnimatedProperty _addedProp = new AnimatedProperty();

                        _properties.Add(_addedProp);
                    }

                    if (_properties[j].propertyName != _states.animSO.propertyNames[j])
                    {
                        AnimatedProperty _newProp = new AnimatedProperty();

                        _newProp.propertyName = states[i].animSO.propertyNames[j];
                        _newProp.settings = new AnimationSettings();

                        _properties[j] = _newProp;

                    }

                }


                // Initialize runtime dictionaries from the SO. These dictionaries carry the float values of the property.
                _properties[j].propertyDict = new Dictionary<string, float>();

                string _name = _properties[j].propertyName;
                float _val = _states.animSO.state.GetFloat(_name);

                _properties[j].propertyDict.Add(_name, _val);



            }
        }
    }


    /*
     The dotween tween is set up at the start. Each tween is contained in a tween driver object.
     The tween lerps its driver value from 0-1. This is multiplied with a delta difference to drive property changes.
    */
    private TweenDriver SetupTween(TweenDriver _tDriver, float _duration, float _currentValue)
    {
        _tDriver.driver = 0f;
        _tDriver.currentValue = _currentValue;
        _tDriver.settings = new AnimationSettings();
        _tDriver.duration = _duration;
        _tDriver.tween = DOTween
            .To(() => _tDriver.driver, x => _tDriver.driver = x, 1f, _duration)
            .SetAutoKill(false)
            .Pause()
            .OnUpdate(() =>
            {
                //gets the difference of current material state, and incrementally add it to current value, while lerping the addition. 
                float _val = _tDriver.start + _tDriver.delta * _tDriver.driver;
                _tDriver.currentValue = _val;
                mpb.SetFloat(_tDriver.propertyName, _val);
                targetRenderer.SetPropertyBlock(mpb);
            })
            .OnComplete(() =>
            {
                //Make sure tween set to end value;
                mpb.SetFloat(_tDriver.propertyName,_tDriver.start + _tDriver.delta);
                targetRenderer.SetPropertyBlock(mpb);
                _tDriver.currentValue = _tDriver.start + _tDriver.delta;
                //Reset driver, trigger previous state if set to.
                _tDriver.driver = 0f;
                if (_tDriver.reset)
                {
                    TriggerAnimate(prevAnimState.animSO);
                }
            });


        return _tDriver;
    }

    private void Start()
    {
        currentAnimState = states[0];

       //Create MBP to animate and set the renderer. To allow shared materials with seperate animation.
        targetRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();


        SetProperties();

        //Create the dictionary that holds the tween driver references.
        propertyTweenDict = new Dictionary<string, TweenDriver>();

        //Create the tweens for each property that is stored to be animated
        for (int i = 0; i < currentAnimState.animSO.propertyNames.Count; i++)
        {
            TweenDriver newDriver = new TweenDriver();
            string _propName = currentAnimState.animSO.propertyNames[i];
            float _propValue = currentAnimState.properties[i].propertyDict[_propName];
            propertyTweenDict.Add(_propName, SetupTween(newDriver, tweensDuration, _propValue));
        }



    }
    private float GetFloat(string propertyName)
    {
        targetRenderer.GetPropertyBlock(mpb);
        return mpb.GetFloat(propertyName);
    }

    private void SetFloat(string propertyName, float value)
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(propertyName, value);
        targetRenderer.SetPropertyBlock(mpb);
    }

    public void TriggerAnimate(AnimationStateSO _SO)
    {


        for (int i = 0; i < states.Count; i++)
        {
            //Check _SO is actually in the list of states to animate.
            //Set previous state, to reset animation later if needed.
            if (_SO != states[i].animSO) continue;
            if (currentAnimState != states[i])
            {
                prevAnimState = currentAnimState;
                currentAnimState = states[i];
            }


        }

        for (int j = 0; j < currentAnimState.properties.Count; j++)
        {

            var _prop = currentAnimState.properties[j];
            //Send in the animated property object needed for animation. Check if material is different, to not send when not needed. 

            if (GetFloat(_prop.propertyName) != _prop.propertyDict[_prop.propertyName])
            {
               // Debug.Log($"Animate prop {prop.propertyName}");
                AnimateValue(_prop);
            }

        }


    }



    //Get the property settings and value to animate to, and then restart the tween driver's tween.
    private void AnimateValue(AnimatedProperty _prop)
    {

        //Get tween driver to animate

        TweenDriver _tDriver = propertyTweenDict[_prop.propertyName];

        //Start value is current material
        _tDriver.start = _tDriver.currentValue;
        //Delta is difference between start and new, to add to current.
        _tDriver.delta = _prop.propertyDict[_prop.propertyName] - _tDriver.start;
        //Other data to update.
        _tDriver.propertyName = _prop.propertyName;
        _tDriver.reset = currentAnimState.shouldReset;



        //Ease settings that new property has from current state.
        _tDriver.tween.SetEase(_prop.settings.ease, _prop.settings.strength, _prop.settings.extra);
        if (!_tDriver.tween.IsPlaying() && !_tDriver.tween.IsComplete())
        {
            // First play of the tween.
            _tDriver.tween.Play();
        }

        // Playback when complete.
        if (_tDriver.tween.IsComplete())
        {
            //send in current driver value and restart. 
            _tDriver.tween.ChangeStartValue(_tDriver.driver, 0);
            _tDriver.tween.Restart();
        }


    }




}

