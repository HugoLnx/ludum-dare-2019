using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerRandomizer : MonoBehaviour
{
    public RuntimeAnimatorController[] controllers;

    void Awake()
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = controllers[Random.Range(0, controllers.Length)];
    }
}
