using DefqonEngine.UI.Timeline.Waveform;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WaveformTextureRenderer))]
public class WaveformTileFactory : MonoBehaviour
{
    [SerializeField]
    private WaveformTextureRenderer texRenderer;

    public WaveformTile CreateTile(
        Transform parent,
        WaveformPoint[] data,
        int startSample,
        int endSample)
    {
        int height =
            Mathf.RoundToInt(
                ((RectTransform)parent)
                .rect.height);

        Texture2D tex =
            texRenderer.GenerateTexture(
                data,
                height);

        GameObject tileObject =
            new GameObject(
                "WaveTile",
                typeof(RawImage));

        tileObject.transform.SetParent(parent, false);

        RawImage image =
            tileObject.GetComponent<RawImage>();

        image.texture = tex;

        RectTransform rt =
            image.rectTransform;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;

        return new WaveformTile
        {
            texture = tex,
            data = data,
            image = image,
            startSample = startSample,
            endSample = endSample
        };
    }
}