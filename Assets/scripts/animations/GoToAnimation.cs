using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToAnimation : PremadeAnimation
{
    private const float FRAME_DELAY = 1f / 30f;
    private float duration;
    private MonoBehaviour obj;
    private Vector3 target;

    public bool HasFinished { get; private set; }
    public float Progress { get; private set; }

    public GoToAnimation(MonoBehaviour obj, Vector3 target, float duration = 1f)
    {
        this.target = target;
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
        var delta = target - obj.transform.position;
        while (Progress < 1f)
        {
            yield return new WaitForSeconds(FRAME_DELAY);
            obj.transform.position += GetFrameAmountFor(delta);
            this.Progress = Mathf.Min(1f, this.Progress + FRAME_DELAY / duration);
        }
        this.HasFinished = true;
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
