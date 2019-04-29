using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TranslateAnimation : PremadeAnimation
{
    private const float FRAME_DELAY = 1f / 30f;
    private float duration;
    private MonoBehaviour obj;
    private Vector3 origin;
    private Vector3 final;

    public bool HasFinished { get; private set; }
    public float Progress { get; private set; }

    public TranslateAnimation(MonoBehaviour obj, Vector3 to, Nullable<Vector3> from = null, float duration=1f)
    {
        this.origin = from.HasValue ? from.Value : Vector3.zero;
        this.final = to;
        this.duration = duration;
        this.obj = obj;
    }

    public PremadeAnimation Start()
    {
        this.obj.StartCoroutine(StartAnimation());
        return this;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

    private IEnumerator StartAnimation()
    {
        obj.transform.position += this.origin;
        while (Progress < 1f)
        {
            yield return new WaitForSeconds(FRAME_DELAY);
            obj.transform.position += GetFrameAmountFor(this.final - this.origin);
            this.Progress = Mathf.Min(1f, this.Progress + FRAME_DELAY / duration);
        }
        this.HasFinished = true;
    }

    private Vector3 GetFrameAmountFor(Vector3 v) {
        return v / Speed;
    }
    private float Speed {
        get { return this.duration / FRAME_DELAY; }
    }
}
