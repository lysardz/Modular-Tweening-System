using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationState", menuName = "Scriptable Objects/AnimationState")]
public class AnimationStateSO : ScriptableObject
{
    public Material state;
    
    public Dictionary<string,float> stateFloatDict = new Dictionary<string,float>();
    public List<string> propertyNames = new List<string>();


    //Set data from material to update into SO state object
    [ContextMenu("SetValues")]
    public void SetValues()
    {
        stateFloatDict.Clear();
        propertyNames.Clear();
        //Go through properties of floats in shader material.
        for (int i = 0; i < state.GetPropertyNames(MaterialPropertyType.Float).Count(); i++)
        {


            string prop = state.GetPropertyNames(MaterialPropertyType.Float)[i];
            //parameters need to start with PR to be included, make sure to name parameters this way. 
            //This is so it doesn't add extra non-custom parameters.
            if (!prop.Contains("PR"))
                continue;

            string normalized = prop.Trim();

            propertyNames.Add(normalized);
            stateFloatDict.Add(normalized, state.GetFloat(prop));

        }
    }    
}
