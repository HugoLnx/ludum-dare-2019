﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterActions : MonoBehaviour
{
    public HunterShot prefabShot;
    private SpriteRenderer srenderer;
    private CharMovement movement;
    private WalkingGrid grid;

    void Awake()
    {
        this.srenderer = GetComponentInChildren<SpriteRenderer>();
        this.grid = GetComponentInParent<WalkingGrid>();
        this.movement = GetComponent<CharMovement>();
    }

    public void Shoot() {
        var shot = GameObject.Instantiate(prefabShot);
        shot.Setup(this.grid, this.movement.Cell, this.movement.HeadedDirection);
    }
}
