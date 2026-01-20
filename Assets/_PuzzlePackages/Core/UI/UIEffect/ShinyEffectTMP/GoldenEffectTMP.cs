using UnityEngine;

public class GoldenEffectTMP : ShinyEffectTMP
{
    protected override Color32 CalculateColor32(float value)
    {
        Color32 color32 = Color.white;
        color32.r = defaultColor.r;
        color32.g = (byte)Mathf.Max(defaultColor.g, value * (shinyColor.g - defaultColor.g) + defaultColor.g);
        color32.b = (byte)Mathf.Max(defaultColor.b, value * shinyColor.b);
        return color32;
    }
}
