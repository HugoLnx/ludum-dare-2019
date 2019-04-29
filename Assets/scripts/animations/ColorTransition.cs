using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTransition
{
    private const float MAX_PROGRESS = 1f;
    public const int KEEP_RED = 0x01;
    public const int KEEP_GREEN = 0x02;
    public const int KEEP_BLUE = 0x04;
    public const int KEEP_ALPHA = 0x08;
    public const int KEEP_VISIBLE_COLORS = KEEP_RED | KEEP_GREEN | KEEP_BLUE;
    public Color OriginColor { get; private set; }
    public Color FinalColor { get; private set; }

    private int keepColors;

    public ColorTransition(Color originColor, Color finalColor, int keepColors = 0x00)
    {
        this.OriginColor = originColor;
        this.FinalColor = finalColor;
        this.keepColors = keepColors;
    }

    public Color GetColor(Color currentColor, float progress)
    {
        var localOriginColor = KeepNeededColorsOf(OriginColor, keepReference: currentColor);
        var localFinalColor = KeepNeededColorsOf(FinalColor, keepReference: currentColor);
        
        return Color.Lerp(localOriginColor, localFinalColor, progress);
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
