using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnimationGroup : PremadeAnimation
{
    private List<PremadeAnimation> animations;
    public bool HasFinished => animations.All((a) => a.HasFinished);
    public float Progress => animations.Average((a) => a.Progress);

    public AnimationGroup(List<PremadeAnimation> animations)
    {
        this.animations = animations;
    }
    public AnimationGroup(List<PremadeAnimation> animations, MonoBehaviour obj, Action after)
    {
        this.animations = animations;
        obj.StartCoroutine(WaitAnimationsFinish(after));
    }

    public PremadeAnimation Start()
    {
        foreach (var a in this.animations) a.Start();
        return this;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        foreach (var a in this.animations) a.SetDuration(duration);
        return this;
    }

    private IEnumerator WaitAnimationsFinish(Action after)
    {
        yield return new WaitUntil(() => this.HasFinished);
        after();
    }
}
