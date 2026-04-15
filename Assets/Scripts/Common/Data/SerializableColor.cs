using Newtonsoft.Json;
using UnityEngine;

namespace DefqonEngine.Common.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct SerializableColor
    {
        [JsonProperty] public float r, g, b, a;

        public SerializableColor(Color c)
        {
            r = c.r;
            g = c.g;
            b = c.b;
            a = c.a;
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }

        public static implicit operator SerializableColor(Color c) => new SerializableColor(c);
        public static implicit operator Color(SerializableColor c) => c.ToColor();
    }
}