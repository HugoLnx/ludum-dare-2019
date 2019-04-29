using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimation
{
    private const float DEFAULT_BLINK_DURATION_IN_SECS = 1f;
    private const float MAX_PROGRESS = 1f;
    private const float HALF_PROGRESS = MAX_PROGRESS / 2f;
    public const int KEEP_RED = 0x01;
    public const int KEEP_GREEN = 0x02;
    public const int KEEP_BLUE = 0x04;
    public const int KEEP_ALPHA = 0x08;
    public const int KEEP_VISIBLE_COLORS = KEEP_RED | KEEP_GREEN | KEEP_BLUE;
    public Color OriginColor { get; private set;  }
    public Color BlinkColor { get; private set; }

    private int keepColors;
    private float progress;
    private float blinkDuration;
    private bool once;

    public BlinkAnimation(Color originColor, Color blinkColor, int keepColors = 0x00, float blinkDuration = DEFAULT_BLINK_DURATION_IN_SECS, bool once = false)
    {
        this.OriginColor = originColor;
        this.BlinkColor = blinkColor;
        this.keepColors = keepColors;
        this.progress = 0f;
        this.blinkDuration = blinkDuration;
        this.once = once;
    }

    public static BlinkAnimation BuildHitBlink()
    {
        return new BlinkAnimation(Color.white, new Color(1f, 1f, 1f, 0.5f), BlinkAnimation.KEEP_VISIBLE_COLORS, blinkDuration: 0.25f, once: true);
    }

    public Color NextColor(Color currentColor, float deltaTime)
    {
        Color nextColor;
        var localOriginColor = KeepNeededColorsOf(OriginColor, keepReference: currentColor);
        var localBlinkColor = KeepNeededColorsOf(BlinkColor, keepReference: currentColor);

        if (progress < HALF_PROGRESS)
        {
            nextColor = Color.Lerp(localOriginColor, localBlinkColor, progress/HALF_PROGRESS);
        }
        else
        {
            nextColor = Color.Lerp(localBlinkColor, localOriginColor, (progress - HALF_PROGRESS)/HALF_PROGRESS);
        }

        if (this.once)
        {
            this.progress = Mathf.Min(this.progress + deltaTime / this.blinkDuration, MAX_PROGRESS);
        } else
        {
            this.progress = (this.progress + deltaTime / this.blinkDuration) % MAX_PROGRESS;
        }
        return nextColor;
    }

    public bool HasFinished()
    {
        if (!this.once) throw new Exception("Only blink an once animation finishes.");
        return this.progress == MAX_PROGRESS;
    }

    private Color KeepNeededColorsOf(Color color, Color keepReference)
    {
        var r = HasKeepColor(this.keepColors, KEEP_RED) ? keepReference.r : color.r;
        var g = HasKeepColor(this.keepColors, KEEP_GREEN) ? keepReference.g : color.g;
        var b = HasKeepColor(this.keepColors, KEEP_BLUE) ? keepReference.b : color.b;
        var a = HasKeepColor(this.keepColors, KEEP_ALPHA) ? keepReference.a : color.a;
        return new Color(r, g, b, a);
    }

    private bool HasKeepColor(int keepColorsCode, int keepCode)
    {
        return (keepColorsCode & keepCode) != 0;
    }
}
