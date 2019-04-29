using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimation : PremadeAnimation
{
    private const float FRAME_DELAY = 1f / 30f;
    private float duration;
    private MonoBehaviour obj;
    private float height;
    private float maxScale;
    private Vector3 origin;
    private Vector3 final;

    public bool HasFinished { get; private set; }
    public float Progress { get; private set; }

    public JumpAnimation(MonoBehaviour obj, Vector3 to, Vector3? from = null, float duration = 1f, float height = 6f)
    {
        this.origin = from.HasValue ? from.Value : Vector3.zero;
        this.final = to;
        this.duration = duration;
        this.obj = obj;
        this.height = height;
        this.maxScale = 2f;
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
        var deltaHeight = 0f;
        while (this.Progress < 1f)
        {
            yield return new WaitForSeconds(FRAME_DELAY);
            var frameHeight = GetHeightFrame();
            deltaHeight += frameHeight;
            obj.transform.position += GetFrameAmountFor(this.final - this.origin) + (Vector3.up * frameHeight);
            
            this.Progress = Mathf.Min(1f, this.Progress + FRAME_DELAY / duration);
        }
        obj.transform.position -= Vector3.up * deltaHeight;
        this.HasFinished = true;
    }

    private float GetHeightFrame()
    {
        var totalFrames = Mathf.RoundToInt(this.duration / FRAME_DELAY);
        var val = this.height / totalFrames;
        return this.Progress <= 0.5f ? val : -val;
    }

    private float GetScaleFrame()
    {
        var totalFrames = Mathf.RoundToInt(this.duration / FRAME_DELAY);
        var val = this.maxScale / totalFrames;
        return this.Progress <= 0.5f ? val : -val;
    }

    private Vector3 GetFrameAmountFor(Vector3 v)
    {
        return v / Speed;
    }
    private float Speed
    {
        get { return this.duration / FRAME_DELAY; }
    }
}
