using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    public const string TAG = "child";
    private AnimationControllerRandomizer randomizer;
    public int ChildTypeInx { get { return randomizer.inx; } }
    
    void Awake()
    {
        this.randomizer = GetComponentInChildren<AnimationControllerRandomizer>();
    }
}
