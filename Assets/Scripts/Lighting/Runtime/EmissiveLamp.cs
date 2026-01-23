using System;
using System.Collections;
using UnityEngine;
namespace DefqonEngine.Lighting.Runtime
{
[RequireComponent(typeof(Renderer))]
    public class EmissiveLamp : MonoBehaviour
    {
        public int lampId = 0;
        public float intensity = 10f;
        Material materialInstance;
        Renderer rend;

        void Awake()
        {
            rend = GetComponent<Renderer>();
            materialInstance = rend.material;

            materialInstance.EnableKeyword("_EMISSION");
        }

        public IEnumerator FadeToColor(Color targetColor, float duration)
        {
            Color startColor = materialInstance.GetColor("_EmissionColor");
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                Color current = Color.Lerp(startColor, targetColor * intensity, time / duration);

                ApplyColorEmissive(current, intensity);
                yield return null;
            }
            ApplyColorEmissive(targetColor, intensity);
        }

        public void ApplyColorEmissive(Color color, float intensity)
        {
            Color emissive = color * intensity;
            materialInstance.SetColor("_EmissionColor", emissive);
            DynamicGI.SetEmissive(rend, emissive);
        }

        public void SetColor(Color color)
        {
            ApplyColorEmissive(color, intensity);
        }
    }
}