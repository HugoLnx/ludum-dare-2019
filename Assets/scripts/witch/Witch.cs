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

    private WitchSprite witchSprite;
    private bool recovering = false;

    void Awake()
    {
        this.movement = GetComponent<CharMovement>();
        this.movement.WalkSpeed = 4f;
        this.Scent = GetComponent<ObjectScent>();
        this.witchSprite = GetComponentInChildren<WitchSprite>();
    }

    public bool BeingHit()
    {
        if (recovering) return false;
        this.Score -= 731;
        this.hud.SetScore(this.Score);
        this.recovering = true;
        StartCoroutine(StartAnimation());
        return true;
    }

    private IEnumerator StartAnimation()
    {
        var invisible = new Color(1f, 1f, 1f, 0.5f);
        var fadeOut = new ColorTransition(Color.white, invisible, ColorTransition.KEEP_VISIBLE_COLORS);
        var fadeIn = new ColorTransition(invisible, Color.white, ColorTransition.KEEP_VISIBLE_COLORS);
        var blinkDuration = 0.2f;
        var blinkAnimations = new List<PremadeAnimation>();
        for (var i = 0; i < 5; i++)
        {
            blinkAnimations.Add(new ColorTransitionAnimation(witchSprite, fadeOut, duration: blinkDuration));
            blinkAnimations.Add(new ColorTransitionAnimation(witchSprite, fadeIn, duration: blinkDuration));
        }
        var animation = new AnimationChain(witchSprite, blinkAnimations).Start();
        yield return new WaitUntil(() => animation.HasFinished);
        this.recovering = false;
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
