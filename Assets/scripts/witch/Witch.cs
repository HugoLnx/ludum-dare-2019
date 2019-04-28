using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public static string TAG = "witch";
    private CharMovement movement;
    private bool carryingChild;
    public int Score { get; private set; }

    void Awake()
    {
        this.movement = GetComponent<CharMovement>();
        this.movement.WalkSpeed = 2.5f;
    }

    public void Dead()
    {
        Debug.Log("dead");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        CheckCollision(collision);    
    }

    private void CheckCollision(Collider2D collision)
    {
        if (collision.CompareTag(Child.TAG) && !carryingChild)
        {
            this.carryingChild = true;
            GameObject.Destroy(collision.gameObject);
        } else if (collision.CompareTag(Cauldron.TAG) && carryingChild)
        {
            this.carryingChild = false;
            this.Score += 1000;
        }
    }
}
