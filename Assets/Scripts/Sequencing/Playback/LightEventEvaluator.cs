using DefqonEngine.Sequencing.Data.Events;
using UnityEngine;

namespace DefqonEngine.Sequencing.Playback
{

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
            if (e.inverted)
                t = 1f - t;

            switch (e.lightEffectType)
            {
                case LightEffectType.Static:
                    result = e.color;
                    break;

                case LightEffectType.Fade:
                    {
                        result = CalculateFade(e, lowerColor, t);
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


        static Color CalculateFade(LightEvent e, Color lowerColor, float t)
        {
            switch (e.fadeCurve)
            {
                case CurveType.EaseIn:
                    {
                        float curveT = t * t;
                        return Color.LerpUnclamped(lowerColor, e.color, curveT);
                    }

                case CurveType.EaseOut:
                    {
                        float curveT = t * (2 - t);
                        return Color.LerpUnclamped(lowerColor, e.color, curveT);
                    }

                case CurveType.EaseInOut:
                    {
                        float curveT = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
                        return Color.LerpUnclamped(lowerColor, e.color, curveT);
                    }
                default:
                    return Color.LerpUnclamped(lowerColor, e.color, t);
            }
        }
    }

}
