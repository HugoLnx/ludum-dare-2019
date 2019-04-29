using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public static string TAG = "witch";
    private CharMovement movement;
    private bool carryingChild;
    private int score;
    public int Score { get { return score; } private set { this.score = value < 0 ? 0 : value; } }
    public PlayerHUD hud;
    public ObjectScent Scent { get; private set; }

    void Awake()
    {
        this.movement = GetComponent<CharMovement>();
        this.movement.WalkSpeed = 4f;
        this.Scent = GetComponent<ObjectScent>();
    }

    public void Dead()
    {
        this.Score -= 731;
        this.hud.SetScore(this.Score);
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
            this.hud.SetScore(this.Score);
        }
    }
}
