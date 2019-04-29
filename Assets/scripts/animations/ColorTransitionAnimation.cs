using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ColorTransitionAnimation : PremadeAnimation
{
    private float duration;
    private ColorTransition transition;
    private MonoBehaviour obj;
    private float FRAME_DELAY = 1f / 30f; // 12 fps
    public float Progress { get; private set; }
    public bool HasFinished { get; private set; }
    private TMP_Text[] texts;
    private Action after;

    public ColorTransitionAnimation(MonoBehaviour obj, ColorTransition transition, float duration = 1f, bool noChildren = false, Action after = null)
    {
        this.texts = noChildren ? new TMP_Text[] { obj.GetComponentInChildren<TMP_Text>() } : obj.GetComponentsInChildren<TMP_Text>();
        this.Progress = 0f;
        this.HasFinished = false;
        this.duration = duration;
        this.transition = transition;
        this.obj = obj;
        this.after = after;
    }

    public PremadeAnimation Start()
    {
        this.obj.StartCoroutine(StartAnimation(this.after));
        return this;
    }

    public PremadeAnimation SetDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

    public ColorTransitionAnimation(MonoBehaviour obj, Color from, Color to, float duration=1f, bool noChildren = false, Action after = null)
        : this(obj, new ColorTransition(from, to), duration, noChildren, after) {}

    public static ColorTransitionAnimation buildFadeOut(MonoBehaviour obj, float duration = 1f, bool noChildren = false, Action after = null)
    {
        var fadeOutTransition = new ColorTransition(Color.white, new Color(1f, 1f, 1f, 0f), BlinkAnimation.KEEP_VISIBLE_COLORS);
        return new ColorTransitionAnimation(obj, fadeOutTransition, duration, noChildren, after);
    }

    public static ColorTransitionAnimation buildFadeIn(MonoBehaviour obj, float duration = 1f, bool noChildren = false, Action after = null)
    {
        var fadeOutTransition = new ColorTransition(new Color(1f, 1f, 1f, 0f), Color.white, BlinkAnimation.KEEP_VISIBLE_COLORS);
        return new ColorTransitionAnimation(obj, fadeOutTransition, duration, noChildren, after);
    }

    private IEnumerator StartAnimation(Action after)
    {
        foreach (var text in this.texts) text.color = transition.OriginColor;
        while (Progress < 1f)
        {
            yield return new WaitForSeconds(FRAME_DELAY);
            this.Progress = Mathf.Min(1f, this.Progress + FRAME_DELAY / duration);
            foreach (var text in this.texts) text.color = transition.GetColor(text.color, this.Progress);
        }
        this.HasFinished = true;
        if (after != null) after();
    }
}
