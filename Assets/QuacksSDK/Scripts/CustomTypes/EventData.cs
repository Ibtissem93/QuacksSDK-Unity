using UnityEngine;

namespace CustomTypes
{
    /// <summary>
    /// Custom type example: Special game event
    /// Demonstrates multiple property types in one custom type
    /// </summary>
    [System.Serializable]
    public class EventData
    {
        public string eventName;
        public int rewardCorn;
        public ColorData specialColor;
        public float durationSeconds;
        public bool showParticles;
    }
}