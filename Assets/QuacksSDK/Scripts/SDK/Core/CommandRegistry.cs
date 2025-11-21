using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Registry for storing and retrieving registered commands
    /// Uses Dictionary for O(1) lookup performance
    /// </summary>
    public class CommandRegistry
    {
        private Dictionary<string, Delegate> commands = new Dictionary<string, Delegate>();

        /// <summary>
        /// Register a command with its callback
        /// </summary>
        public void RegisterCommand(string commandName, Delegate callback)
        {
            if (commands.ContainsKey(commandName))
            {
                Debug.LogWarning($"[Registry] Overwriting command: '{commandName}'");
            }

            commands[commandName] = callback;
            Debug.Log($"[Registry] -- Registered: '{commandName}'");
        }

        /// <summary>
        /// Try to retrieve a registered command
        /// </summary>
        public bool TryGetCommand(string commandName, out Delegate callback)
        {
            return commands.TryGetValue(commandName, out callback);
        }

        /// <summary>
        /// Check if a command exists
        /// </summary>
        public bool HasCommand(string commandName)
        {
            return commands.ContainsKey(commandName);
        }

        /// <summary>
        /// Get total number of registered commands
        /// </summary>
        public int GetCommandCount()
        {
            return commands.Count;
        }

        /// <summary>
        /// Get all registered command names
        /// </summary>
        public List<string> GetCommandNames()
        {
            return new List<string>(commands.Keys);
        }
    }
}