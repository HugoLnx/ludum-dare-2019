using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharController : MonoBehaviour
{
    private CharMovement movement;
    private const float AXIS_OFFSET = 0.05f;

    void Awake()
    {
        this.movement = GetComponent<CharMovement>();
    }
    
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) <= AXIS_OFFSET && Mathf.Abs(vertical) <= AXIS_OFFSET)
        {
            movement.Stop();
        }
        else
        {
            Nullable<CharMovement.Direction> direction = null;
            if (horizontal >= AXIS_OFFSET) direction = CharMovement.Direction.RIGHT;
            if (horizontal <= -AXIS_OFFSET) direction = CharMovement.Direction.LEFT;
            if (vertical >= AXIS_OFFSET) direction = CharMovement.Direction.UP;
            if (vertical <= -AXIS_OFFSET) direction = CharMovement.Direction.DOWN;
            if (direction.HasValue) movement.Move(direction.Value);
        }

    }
}
