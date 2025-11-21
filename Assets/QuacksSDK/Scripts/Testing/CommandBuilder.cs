using SDK;
using CustomTypes;
using UnityEngine;

namespace Testing
{
    /// <summary>
    /// Helper class to easily build ServerCommand objects
    /// Makes it simple to create commands without writing JSON
    /// </summary>
    public static class CommandBuilder
    {
        // INT commands
        public static ServerCommand FeedDuck(int amount)
        {
            return new ServerCommand("FeedDuck", new { value = amount });
        }

        // FLOAT commands
        public static ServerCommand SetVolume(float volume)
        {
            return new ServerCommand("SetQuackVolume", new { value = volume });
        }

        // COLOR commands
        public static ServerCommand ChangeColor(Color color)
        {
            return new ServerCommand("ChangeDuckColor", new
            {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a
            });
        }

        // VECTOR3 commands
        public static ServerCommand MoveToPond(Vector3 position)
        {
            return new ServerCommand("MoveToPond", new
            {
                x = position.x,
                y = position.y,
                z = position.z
            });
        }

        // CUSTOM TYPE: DuckReward
        public static ServerCommand GiveReward(DuckReward reward)
        {
            return new ServerCommand("GiveReward", reward);
        }

        // CUSTOM TYPE: PondInfo
        public static ServerCommand TeleportToPond(PondInfo pondInfo)
        {
            return new ServerCommand("TeleportToPond", pondInfo);
        }

        // CUSTOM TYPE: EventData
        public static ServerCommand StartEvent(EventData eventData)
        {
            return new ServerCommand("StartEvent", eventData);
        }
    }
}