using UnityEngine;

namespace Demo
{
    /// <summary>
    /// Duck's current state data
    /// Represents all changeable properties
    /// </summary>
    [System.Serializable]
    public class DuckStats
    {
        [Header("Resources")]
        public int corn = 0;

        [Header("Appearance")]
        public Color currentColor = Color.yellow;

        [Header("Audio")]
        [Range(0f, 1f)]
        public float quackVolume = 0.5f;

        [Header("Location")]
        public string currentPond = "Home";
        public Vector3 position = Vector3.zero;

        [Header("Event Info")]
        public string activeEvent = "None";
        public bool inEvent = false;
    }
}