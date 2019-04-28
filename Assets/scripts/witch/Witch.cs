using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public static string TAG = "witch";
    private CharMovement movement;
    void Awake()
    {
        this.movement = GetComponent<CharMovement>();
        this.movement.WalkSpeed = 2.5f;
    }
    
    void Update()
    {
        
    }

    public void Dead()
    {
        Debug.Log("dead");
    }
}
