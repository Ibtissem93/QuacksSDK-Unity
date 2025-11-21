using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SDK;
using UnityEngine;
using CustomTypes;

namespace Testing
{
    public class MockServer : MonoBehaviour
    {
        [Header("Server Mode")]
        [SerializeField] private bool useFileIO = false;

        [Header("File Settings")]
        [SerializeField] private string jsonFileName = "ServerCommands.json";

        [Header("Debug")]
        [SerializeField] private bool showDetailedLogs = true;

        private List<ServerCommand> commands = new List<ServerCommand>();
        private QuacksSDK sdk;

        void Start()
        {
            sdk = QuacksSDK.Instance;

            if (sdk == null)
            {
                Debug.LogError("[MockServer] Quacks SDK not found!");
                return;
            }

            LoadCommands();
            Log($"Initialized with {commands.Count} commands");
            ShowAvailableCommands();
        }

        private void LoadCommands()
        {
            if (useFileIO)
                LoadFromFile();
            else
                GenerateInMemory();
        }

        private void LoadFromFile()
        {
            string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

            if (!File.Exists(path))
            {
                Debug.LogError($"[MockServer] File not found: {path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                commands = JsonConvert.DeserializeObject<List<ServerCommand>>(json, settings);
                Log($"✓ Loaded {commands.Count} commands from file");
            }
            catch (Exception e)
            {
                Debug.LogError($"[MockServer] Failed to load file: {e.Message}");
            }
        }

        private void GenerateInMemory()
        {
            commands.Clear();

            // INT commands
            commands.Add(CommandBuilder.FeedDuck(10));
            commands.Add(CommandBuilder.FeedDuck(25));
            commands.Add(CommandBuilder.FeedDuck(50));

            // COLOR commands
            commands.Add(CommandBuilder.ChangeColor(Color.black));
            commands.Add(CommandBuilder.ChangeColor(Color.white));
            commands.Add(CommandBuilder.ChangeColor(Color.orange));
            commands.Add(CommandBuilder.ChangeColor(Color.blue));

            // VECTOR3 commands
            commands.Add(CommandBuilder.MoveToPond(new Vector3(3, 0, 2)));
            commands.Add(CommandBuilder.MoveToPond(new Vector3(-2, 0, 1)));
            commands.Add(CommandBuilder.MoveToPond(Vector3.zero));

            // FLOAT commands
            commands.Add(CommandBuilder.SetVolume(1.0f));
            commands.Add(CommandBuilder.SetVolume(0.3f));
            commands.Add(CommandBuilder.SetVolume(0.7f));

            // CUSTOM TYPES
            commands.Add(CommandBuilder.GiveReward(new DuckReward
            {
                cornAmount = 50,
                duckColor = new ColorData(1.0f, 0.84f, 0.0f, 1.0f),
                message = "Daily Bonus!",
                volumeBoost = 0.2f
            }));


            commands.Add(CommandBuilder.TeleportToPond( new PondInfo
            {
                position = Vector3Data.FromVector3(new Vector3(-3, 0, 3)),
                pondName = "Golden Lake",
                cornBonus = 25
            }));

            commands.Add(CommandBuilder.StartEvent(new EventData
            {
                eventName = "Summer Splash",
                rewardCorn = 75,
                specialColor = new ColorData(1.0f, 1.0f, 0.0f, 1.0f),
                durationSeconds = 30.0f,
                showParticles = true
            }));

            Log($"Generated {commands.Count} commands in memory");
        }

        private void ShowAvailableCommands()
        {
            Debug.Log($"[MockServer] === Available Commands ({commands.Count}) ===");

            // Group commands by name for better readability
            Dictionary<string, int> commandCounts = new Dictionary<string, int>();

            foreach (var cmd in commands)
            {
                if (commandCounts.ContainsKey(cmd.command))
                    commandCounts[cmd.command]++;
                else
                    commandCounts[cmd.command] = 1;
            }

            foreach (var kvp in commandCounts)
            {
                Debug.Log($"[MockServer] - {kvp.Key} ({kvp.Value} variant{(kvp.Value > 1 ? "s" : "")})");
            }

            Debug.Log("[MockServer] =====================================");
        }

        #region Automated Testing

        [ContextMenu("Send Random Command")]
        public void SendRandomCommand()
        {
            if (!ValidateState()) return;

            int index = UnityEngine.Random.Range(0, commands.Count);
            Debug.Log($"[MockServer] Randomly selected: {commands[index].command}");
            SendCommand(commands[index]);
        }

        [ContextMenu("Send All Commands in Sequence")]
        public void SendAllCommands()
        {
            StartCoroutine(SendAllWithDelay(1.0f));
        }

        private IEnumerator SendAllWithDelay(float delay)
        {
            Debug.Log($"[MockServer] Sending all {commands.Count} commands...");

            for (int i = 0; i < commands.Count; i++)
            {
                Debug.Log($"[MockServer] Sending command {i + 1}/{commands.Count}: {commands[i].command}");
                SendCommand(commands[i]);
                yield return new WaitForSeconds(delay);
            }

            Debug.Log("[MockServer] All commands sent!");
        }

        #endregion

        #region Manual Testing by Name

        [ContextMenu("Test: FeedDuck")]
        public void TestFeedDuck()
        {
            SendRandomCommandByName("FeedDuck");
        }

        [ContextMenu("Test: ChangeDuckColor")]
        public void TestChangeDuckColor()
        {
            SendRandomCommandByName("ChangeDuckColor");
        }

        [ContextMenu("Test: MoveToPond")]
        public void TestMoveToPond()
        {
            SendRandomCommandByName("MoveToPond");
        }

        [ContextMenu("Test: SetQuackVolume")]
        public void TestSetQuackVolume()
        {
            SendRandomCommandByName("SetQuackVolume");
        }

        [ContextMenu("Test: GiveReward")]
        public void TestGiveReward()
        {
            SendRandomCommandByName("GiveReward");
        }

        [ContextMenu("Test: TeleportToPond")]
        public void TestTeleportToPond()
        {
            SendRandomCommandByName("TeleportToPond");
        }

        [ContextMenu("Test: StartEvent")]
        public void TestStartEvent()
        {
            SendRandomCommandByName("StartEvent");
        }

        #endregion

        #region Core Functionality

        /// <summary>
        /// Send a random command with the specified name from the stored list
        /// If multiple variants exist, picks one randomly
        /// </summary>
        public void SendRandomCommandByName(string commandName)
        {
            List<ServerCommand> matchingCommands = commands.FindAll(c => c.command == commandName);

            if (matchingCommands.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, matchingCommands.Count);

                if (matchingCommands.Count > 1)
                {
                    Debug.Log($"[MockServer] Found {matchingCommands.Count} '{commandName}' variants, sending variant #{index + 1}");
                }
                else
                {
                    Debug.Log($"[MockServer] Sending: {commandName}");
                }

                SendCommand(matchingCommands[index]);
            }
            else
            {
                Debug.LogError($"[MockServer] Command '{commandName}' not found in list!");
                Debug.LogError($"[MockServer] Available commands: {string.Join(", ", GetUniqueCommandNames())}");
            }
        }

        /// <summary>
        /// Get list of unique command names from stored list
        /// </summary>
        private List<string> GetUniqueCommandNames()
        {
            HashSet<string> uniqueNames = new HashSet<string>();
            foreach (var cmd in commands)
            {
                uniqueNames.Add(cmd.command);
            }
            return new List<string>(uniqueNames);
        }

        public void SendCommand(ServerCommand cmd)
        {
            if (sdk == null)
            {
                Debug.LogError("[MockServer] SDK not initialized!");
                return;
            }

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(cmd, settings);

            Log($"Sending: {cmd.command}");
            sdk.ProcessServerMessage(json);
        }

        public int GetCommandCount()
        {
            return commands.Count;
        }

        private bool ValidateState()
        {
            if (commands == null || commands.Count == 0)
            {
                Debug.LogError("[MockServer] No commands available!");
                return false;
            }

            if (sdk == null)
            {
                Debug.LogError("[MockServer] SDK not initialized!");
                return false;
            }

            return true;
        }

        private void Log(string message)
        {
            if (showDetailedLogs)
            {
                Debug.Log($"[MockServer] {message}");
            }
        }

        #endregion
    }
}