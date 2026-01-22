using UnityEngine;
using System.Collections;
using System;

public class EmissiveLamp : MonoBehaviour
{
    public string lampId = Guid.NewGuid().ToString(); // uniek ID voor save/load
    public float intensity = 10f;
    Material materialInstance;
    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        materialInstance = rend.material;

        materialInstance.EnableKeyword("_EMISSION");
    }

    public void SetColor(Color color)
    {
        materialInstance.SetColor("_EmissionColor", color);
        DynamicGI.SetEmissive(rend, color);
    }

    public IEnumerator FadeToColor(Color targetColor, float duration)
    {
        Color startColor = materialInstance.GetColor("_EmissionColor");
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            Color current = Color.Lerp(startColor, targetColor * intensity, time / duration);

            materialInstance.SetColor("_EmissionColor", current);
            DynamicGI.SetEmissive(rend, current);

            yield return null;
        }

        materialInstance.SetColor("_EmissionColor", targetColor * intensity);
        DynamicGI.SetEmissive(rend, targetColor * intensity);
    }

    public void ApplyEmission(Color color, float intensity)
    {
        Color emissive = color * intensity;
        materialInstance.SetColor("_EmissionColor", emissive);
        DynamicGI.SetEmissive(rend, emissive);
    }


    public void TurnOff()
    {
        SetColor(Color.black);
    }
}