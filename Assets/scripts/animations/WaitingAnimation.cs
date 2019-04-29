using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingAnimation : PremadeAnimation
{
    private float duration;
    private MonoBehaviour obj;

    public bool HasFinished { get; private set; }
    public float Progress { get; private set; }

    public WaitingAnimation(MonoBehaviour obj, float duration)
    {
        this.duration = duration;
        this.obj = obj;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

    public PremadeAnimation Start()
    {
        obj.StartCoroutine(StartAnimation());
        return this;
    }

    private IEnumerator StartAnimation()
    {
        var timepast = 0f;
        while (this.Progress < 1f)
        {
            yield return new WaitForSeconds(0.5f);
            timepast += 0.5f;
            this.Progress = Mathf.Min(1f, this.duration / timepast);
        }
        HasFinished = true;
    }
}
