using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerRandomizer : MonoBehaviour
{
    public RuntimeAnimatorController[] controllers;
    public int inx;

    void Awake()
    {
        var animator = GetComponent<Animator>();
        this.inx = Random.Range(0, controllers.Length);
        animator.runtimeAnimatorController = controllers[inx];
    }
}
