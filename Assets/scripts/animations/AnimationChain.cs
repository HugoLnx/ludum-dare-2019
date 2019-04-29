using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnimationChain : PremadeAnimation
{
    private MonoBehaviour obj;
    private List<PremadeAnimation> chain;
    private Action after;

    public bool HasFinished => chain.Last().HasFinished;
    public float Progress => chain.Average((a) => a.Progress);

    public AnimationChain(MonoBehaviour obj, List<PremadeAnimation> chain, System.Action after = null)
    {
        this.obj = obj;
        this.chain = chain;
        this.after = after;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        throw new System.NotImplementedException();
    }

    public PremadeAnimation Start()
    {
        obj.StartCoroutine(StartChain());
        return this;
    }

    private IEnumerator StartChain()
    {
        foreach (var anim in this.chain)
        {
            anim.Start();
            yield return new WaitUntil(() => anim.HasFinished);
        }
        if (this.after != null) this.after();
    }
}
