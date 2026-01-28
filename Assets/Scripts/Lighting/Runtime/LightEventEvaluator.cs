using DefqonEngine.Lighting.Data;
using UnityEngine;

public static class LightEventEvaluator
{
    public static bool Evaluate(LightEvent e, float globalTime, Color lowerColor, out Color result)
    {
        result = Color.black;

        float start = e.time;
        float end = e.time + e.duration;

        // check of event actief is
        if (globalTime < start || globalTime > end)
            return false;

        // normaliseer tijd over duration
        float t = (globalTime - start) / e.duration;
        t = Mathf.Clamp01(t);

        switch (e.lightEffectType)
        {
            case LightEffectType.Static:
                result = e.color;
                break;

            case LightEffectType.Fade:
                {
                    float curveT = e.fadeCurve.Evaluate(t);
                    result = Color.LerpUnclamped(lowerColor, e.color, curveT);
                    break;
                }

            case LightEffectType.Chase:
                result = e.color; // implementatie later
                break;
            case LightEffectType.Converge:
                result = e.color; // implementatie later
                break;
        }

        return true;
    }
}
