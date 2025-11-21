using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Wrapper classes for primitive and Unity types
    /// Enables JSON serialization of scalar values
    /// </summary>

    [System.Serializable]
    public class IntParameter
    {
        public int value;
    }

    [System.Serializable]
    public class FloatParameter
    {
        public float value;
    }

    [System.Serializable]
    public class StringParameter
    {
        public string value;
    }

    [System.Serializable]
    public class Vector3Parameter
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class ColorParameter
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }
}