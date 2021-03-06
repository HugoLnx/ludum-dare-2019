﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public const string TAG = "cauldron";
    public ObjectScent Scent { get; private set; }

    void Awake()
    {
        this.Scent = GetComponent<ObjectScent>();
    }
}
