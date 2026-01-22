using UnityEngine;
using System.Collections;

public class LightShowController : MonoBehaviour
{
    public EmissiveLamp[] lamps;

    public LightPattern currentPattern = LightPattern.Static;

    [Header("Settings")]
    public Color baseColor = Color.cyan;
    public float intensity = 5f;
    public float speed = 1f;
    public float waveOffset = 0.2f;

    Coroutine runningShow;

    void Start()
    {
        StartShow(currentPattern);
    }

    void Update()
    {
        // Debug keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartShow(LightPattern.FadeAll);
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartShow(LightPattern.Pulse);
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartShow(LightPattern.Wave);
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartShow(LightPattern.Chase);
        if (Input.GetKeyDown(KeyCode.Alpha5)) StartShow(LightPattern.Rainbow);
    }

    public void StartShow(LightPattern pattern)
    {
        if (runningShow != null)
            StopCoroutine(runningShow);

        currentPattern = pattern;

        switch (pattern)
        {
            case LightPattern.FadeAll:
                runningShow = StartCoroutine(FadeAll());
                break;
            case LightPattern.Pulse:
                runningShow = StartCoroutine(Pulse());
                break;
            case LightPattern.Wave:
                runningShow = StartCoroutine(Wave());
                break;
            case LightPattern.Chase:
                runningShow = StartCoroutine(Chase());
                break;
            case LightPattern.Rainbow:
                runningShow = StartCoroutine(Rainbow());
                break;
        }
    }

    // ================= PATTERNS =================

    IEnumerator FadeAll()
    {
        while (true)
        {
            yield return FadeIntensityAll(0f, 1f / speed);
            yield return FadeIntensityAll(intensity, 1f / speed);
        }
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            foreach (var lamp in lamps)
                lamp.SetColor(baseColor * Mathf.Lerp(0f, intensity, t));
            yield return null;
        }
    }

    IEnumerator Wave()
    {
        while (true)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                float t = Mathf.Sin(Time.time * speed - i * waveOffset);
                float iVal = Mathf.Clamp01((t + 1f) / 2f);
                lamps[i].ApplyEmission(baseColor, iVal * intensity);
            }
            yield return null;
        }
    }

    IEnumerator Chase()
    {
        int index = 0;
        while (true)
        {
            for (int i = 0; i < lamps.Length; i++)
                lamps[i].ApplyEmission(baseColor, i == index ? intensity : 0f);

            index = (index + 1) % lamps.Length;
            yield return new WaitForSeconds(0.1f / speed);
        }
    }

    IEnumerator Rainbow()
    {
        while (true)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                float hue = Mathf.Repeat(Time.time * speed + i * 0.1f, 1f);
                Color c = Color.HSVToRGB(hue, 1f, 1f);
                lamps[i].ApplyEmission(c, intensity);
            }
            yield return null;
        }
    }

    // ================= HELPERS =================

    IEnumerator FadeIntensityAll(float target, float duration)
    {
        float[] start = new float[lamps.Length];
        for (int i = 0; i < lamps.Length; i++)
            start[i] = lamps[i].intensity;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            for (int i = 0; i < lamps.Length; i++)
            {
                float iVal = Mathf.Lerp(start[i], target, t / duration);
                lamps[i].ApplyEmission(baseColor, iVal);
            }
            yield return null;
        }
    }
}
