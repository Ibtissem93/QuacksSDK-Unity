using System;

namespace SDK
{
    /// <summary>
    /// Represents a command message from the server
    /// Contains command name and parameters
    /// </summary>
    [Serializable]
    public class ServerCommand
    {
        public string command;
        public object parameters;

        public ServerCommand() { }

        public ServerCommand(string command, object parameters)
        {
            this.command = command;
            this.parameters = parameters;
        }
    }
}