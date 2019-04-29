using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedAnimation : PremadeAnimation
{
    public bool HasFinished { get { return true; } }
    public float Progress { get { return 1f; } }

    private static readonly FinishedAnimation instance = new FinishedAnimation();
    public static FinishedAnimation Instance { get { return instance; } }

    public PremadeAnimation Start()
    {
        return this;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        return this;
    }
}
