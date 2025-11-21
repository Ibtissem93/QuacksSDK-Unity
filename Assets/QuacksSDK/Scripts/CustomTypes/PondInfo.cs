using UnityEngine;

namespace CustomTypes
{
    [System.Serializable]
    public class PondInfo
    {
        public Vector3Data position;  
        public string pondName;
        public int cornBonus;
    }

    // Add this wrapper
    [System.Serializable]
    public class Vector3Data
    {
        public float x;
        public float y;
        public float z;

        public Vector3Data() { }

        public Vector3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static Vector3Data FromVector3(Vector3 v)
        {
            return new Vector3Data(v.x, v.y, v.z);
        }
    }
}