using UnityEngine;

public class LampController : MonoBehaviour
{
    [SerializeField] EmissiveLamp[] lamps;
    public float fadeDuration = 1.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeAll(Color.magenta);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            FadeAll(Color.black); // uit
        }
    }

    public void FadeAll(Color targetColor)
    {
        foreach (var lamp in lamps)
        {
            StartCoroutine(lamp.FadeToColor(targetColor, fadeDuration));
        }
    }
}
