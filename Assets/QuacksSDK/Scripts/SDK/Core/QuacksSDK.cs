using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SDK
{
    public class QuacksSDK : MonoBehaviour
    {
        public static QuacksSDK Instance { get; private set; }

        private CommandRegistry registry;
        private TypeConverter converter;

        // Error callback for production monitoring
        public event Action<string, string> OnCommandError;

        #region Initialization

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSDK();
        }

        private void InitializeSDK()
        {
            registry = new CommandRegistry();
            converter = new TypeConverter();

            Debug.Log("[Quacks SDK] Initialized");
        }

        #endregion

        #region Public API

        public void RegisterCommand<T>(string commandName, Action<T> callback)
        {
            if (!ValidateRegistration(commandName, callback))
                return;

            registry.RegisterCommand(commandName, callback);
        }

        public void ProcessServerMessage(string jsonMessage)
        {
            // Validate input
            if (string.IsNullOrEmpty(jsonMessage))
            {
                LogError("Received empty message from server");
                OnCommandError?.Invoke("unknown", "Empty message");
                return;
            }

            try
            {
                Debug.Log($"[Quacks SDK] Processing: {jsonMessage}");

                // Parse message
                JObject messageObj;
                try
                {
                    messageObj = JObject.Parse(jsonMessage);
                }
                catch (Exception e)
                {
                    LogError($"Invalid JSON format: {e.Message}");
                    LogError($"Received: {jsonMessage}");
                    OnCommandError?.Invoke("parse_error", $"Invalid JSON: {e.Message}");
                    return;
                }

                // Extract command name
                string commandName = messageObj["command"]?.ToString();
                if (string.IsNullOrEmpty(commandName))
                {
                    LogError("Invalid message: missing 'command' field");
                    LogError($"Message structure: {messageObj}");
                    OnCommandError?.Invoke("unknown", "Missing command field");
                    return;
                }

                // Extract parameters
                JToken parametersToken = messageObj["parameters"];
                if (parametersToken == null)
                {
                    LogWarning($"Command '{commandName}' has no parameters");
                }

                // Execute command
                ExecuteCommand(commandName, parametersToken);
            }
            catch (Exception e)
            {
                LogError($"Unexpected error processing message: {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
                OnCommandError?.Invoke("unknown", $"Unexpected error: {e.Message}");
            }
        }

        public int GetRegisteredCommandCount()
        {
            return registry.GetCommandCount();
        }

        #endregion

        #region Internal Logic

        private void ExecuteCommand(string commandName, JToken parametersToken)
        {
            // Lookup command in registry
            if (!registry.TryGetCommand(commandName, out Delegate callback))
            {
                LogError($"Command not found: '{commandName}'");

                // Suggest similar commands
                var availableCommands = registry.GetCommandNames();
                LogError($"Available commands: {string.Join(", ", availableCommands)}");

                // Find closest match
                string suggestion = FindClosestCommand(commandName, availableCommands);
                if (!string.IsNullOrEmpty(suggestion))
                {
                    LogError($"Did you mean: '{suggestion}'?");
                }

                OnCommandError?.Invoke(commandName, "Command not found");
                return;
            }

            // Determine parameter type
            Type parameterType = GetParameterType(callback);
            if (parameterType == null)
            {
                LogError($"Command '{commandName}' has invalid signature");
                OnCommandError?.Invoke(commandName, "Invalid command signature");
                return;
            }

            // Convert parameters
            object parameter;
            try
            {
                parameter = ConvertParameter(parametersToken, parameterType);

                if (parameter == null && parameterType.IsValueType)
                {
                    LogError($"Failed to convert parameters for '{commandName}' (type: {parameterType.Name})");
                    OnCommandError?.Invoke(commandName, "Parameter conversion failed");
                    return;
                }
            }
            catch (Exception e)
            {
                LogError($"Parameter conversion error for '{commandName}': {e.Message}");
                OnCommandError?.Invoke(commandName, $"Conversion error: {e.Message}");
                return;
            }

            // Execute command with error handling
            try
            {
                callback.DynamicInvoke(parameter);
                Debug.Log($"[Quacks SDK] Executed: {commandName}");
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                // Unwrap the actual exception
                Exception innerException = e.InnerException ?? e;
                LogError($"Command '{commandName}' execution failed: {innerException.Message}");
                LogError($"Stack trace: {innerException.StackTrace}");
                OnCommandError?.Invoke(commandName, $"Execution error: {innerException.Message}");
            }
            catch (Exception e)
            {
                LogError($"Unexpected error executing '{commandName}': {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
                OnCommandError?.Invoke(commandName, $"Unexpected error: {e.Message}");
            }
        }

        private Type GetParameterType(Delegate callback)
        {
            var parameters = callback.Method.GetParameters();

            if (parameters.Length == 0)
            {
                LogError("Callback has no parameters");
                return null;
            }

            return parameters[0].ParameterType;
        }

        private object ConvertParameter(JToken parametersToken, Type targetType)
        {
            if (parametersToken == null)
            {
                LogWarning($"Parameters token is null for type {targetType.Name}");

                // Check if type is nullable
                if (!targetType.IsValueType)
                {
                    return null;  // OK for reference types
                }

                LogError($"Cannot pass null to value type {targetType.Name}");
                throw new ArgumentNullException($"Parameters required for type {targetType.Name}");
            }

            try
            {
                // Handle primitives with wrappers
                if (targetType == typeof(int))
                    return ConvertInt(parametersToken);

                if (targetType == typeof(float))
                    return ConvertFloat(parametersToken);

                if (targetType == typeof(string))
                    return ConvertString(parametersToken);

                // Handle Unity types
                if (targetType == typeof(Vector3))
                    return ConvertVector3(parametersToken);

                if (targetType == typeof(Color))
                    return ConvertColor(parametersToken);

                // Handle custom types
                object result = converter.ConvertFromJson(parametersToken.ToString(), targetType);

                if (result == null)
                {
                    throw new InvalidOperationException($"Failed to deserialize type {targetType.Name}");
                }

                return result;
            }
            catch (Exception e)
            {
                LogError($"Conversion error for {targetType.Name}: {e.Message}");
                throw;  // Re-throw to be caught in ExecuteCommand
            }
        }

        #endregion

        #region Type Converters

        private int ConvertInt(JToken token)
        {
            var wrapper = converter.ConvertFromJson<IntParameter>(token.ToString());

            if (wrapper == null)
            {
                throw new InvalidOperationException("Failed to parse IntParameter");
            }

            return wrapper.value;
        }

        private float ConvertFloat(JToken token)
        {
            var wrapper = converter.ConvertFromJson<FloatParameter>(token.ToString());

            if (wrapper == null)
            {
                throw new InvalidOperationException("Failed to parse FloatParameter");
            }

            return wrapper.value;
        }

        private string ConvertString(JToken token)
        {
            var wrapper = converter.ConvertFromJson<StringParameter>(token.ToString());

            if (wrapper == null)
            {
                throw new InvalidOperationException("Failed to parse StringParameter");
            }

            return wrapper.value;
        }

        private Vector3 ConvertVector3(JToken token)
        {
            var wrapper = converter.ConvertFromJson<Vector3Parameter>(token.ToString());

            if (wrapper == null)
            {
                throw new InvalidOperationException("Failed to parse Vector3Parameter");
            }

            return new Vector3(wrapper.x, wrapper.y, wrapper.z);
        }

        private Color ConvertColor(JToken token)
        {
            var wrapper = converter.ConvertFromJson<ColorParameter>(token.ToString());

            if (wrapper == null)
            {
                throw new InvalidOperationException("Failed to parse ColorParameter");
            }

            return new Color(wrapper.r, wrapper.g, wrapper.b, wrapper.a);
        }

        #endregion

        #region Validation & Helpers

        private bool ValidateRegistration(string commandName, Delegate callback)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                LogError("Command name cannot be empty");
                return false;
            }

            if (callback == null)
            {
                LogError($"Callback for '{commandName}' cannot be null");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Find the closest matching command name (typo suggestion)
        /// </summary>
        private string FindClosestCommand(string input, System.Collections.Generic.List<string> availableCommands)
        {
            if (availableCommands.Count == 0)
                return null;

            string closest = null;
            int minDistance = int.MaxValue;

            foreach (var command in availableCommands)
            {
                int distance = LevenshteinDistance(input.ToLower(), command.ToLower());

                if (distance < minDistance && distance <= 3)  // Max 3 character difference
                {
                    minDistance = distance;
                    closest = command;
                }
            }

            return closest;
        }

        /// <summary>
        /// Calculate Levenshtein distance for typo detection
        /// </summary>
        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        private void LogError(string message)
        {
            Debug.LogError($"[Quacks SDK] {message}");
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[Quacks SDK] {message}");
        }

        #endregion
    }
}