using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateSetter", menuName = "Scriptable Objects/StateSetter")]
public class StateSetter : ScriptableObject
{
    [SerializeField] List<AnimationStateSO> stateSOList = new List<AnimationStateSO>();

    [ContextMenu("Set States")]
    public void SetStates()
    {
        foreach(var _state in stateSOList)
        {
            _state.SetValues();
        }
    }
}
