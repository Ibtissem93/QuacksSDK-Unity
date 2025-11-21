using UnityEngine;

namespace CustomTypes
{
    /// <summary>
    /// Custom type example: Bundled reward package
    /// Demonstrates SDK support for complex custom types
    /// </summary>
    [System.Serializable]
    public class DuckReward
    {
        public int cornAmount;
        public ColorData duckColor;
        public string message;
        public float volumeBoost;
    }

    /// <summary>
    /// Serializable color data structure
    /// Used instead of Unity.Color for JSON compatibility
    /// </summary>
    [System.Serializable]
    public class ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorData() { }

        public ColorData(float r, float g, float b, float a = 1.0f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static ColorData FromColor(Color color)
        {
            return new ColorData(color.r, color.g, color.b, color.a);
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }
}