using UnityEngine;
using CustomTypes;
using SDK;

namespace Demo
{
    /// <summary>
    /// Duck game logic - demonstrates SDK usage
    /// Registers commands and implements game behavior
    /// </summary>
    public class DuckController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private ParticleSystem eventParticles;
        [SerializeField] private AudioSource quackAudioSource;

        [Header("Duck State")]
        [SerializeField] private DuckStats stats = new DuckStats();

        private QuacksSDK sdk;
        private Renderer duckRenderer;

        #region Initialization

        void Start()
        {
            InitializeDuck();
            RegisterAllCommands();
            UpdateUI();
        }

        private void InitializeDuck()
        {
            sdk = QuacksSDK.Instance;

            if (sdk == null)
            {
                Debug.LogError("[Duck] Quacks SDK not found!");
                return;
            }

            duckRenderer = GetComponent<Renderer>();
            if (duckRenderer != null)
            {
                duckRenderer.material.color = stats.currentColor;
            }
  
            if (eventParticles != null)
            {
                eventParticles.Stop();
            }

            // Initialize audio
            if (quackAudioSource != null)
            {
                quackAudioSource.volume = stats.quackVolume;
                quackAudioSource.loop = false;
            }
            Debug.Log("[Duck] Duck initialized!");
        }

        private void RegisterAllCommands()
        {
            // Primitive type commands
            sdk.RegisterCommand<int>("FeedDuck", FeedDuck);
            sdk.RegisterCommand<float>("SetQuackVolume", SetQuackVolume);

            // Unity type commands
            sdk.RegisterCommand<Color>("ChangeDuckColor", ChangeDuckColor);
            sdk.RegisterCommand<Vector3>("MoveToPond", MoveToPond);

            // Custom type commands
            sdk.RegisterCommand<DuckReward>("GiveReward", GiveReward);
            sdk.RegisterCommand<PondInfo>("TeleportToPond", TeleportToPond);
            sdk.RegisterCommand<EventData>("StartEvent", StartEvent);

            Debug.Log($"[Duck] -- Registered {sdk.GetRegisteredCommandCount()} commands");
        }

        #endregion

        #region Command Handlers

        // INT command
        void FeedDuck(int amount)
        {
            stats.corn += amount;
            Debug.Log($"Fed duck {amount} corn! Total: {stats.corn}");
            PlayQuack();
            UpdateUI();
        }

        // FLOAT command
        void SetQuackVolume(float volume)
        {
            stats.quackVolume = Mathf.Clamp01(volume);

            // Update AudioSource volume
            if (quackAudioSource != null)
            {
                quackAudioSource.volume = stats.quackVolume;
            }

            Debug.Log($"Volume set to {stats.quackVolume * 100:F0}%");

            PlayQuack();  // ← Test the new volume!
            UpdateUI();
        }

        // COLOR command
        void ChangeDuckColor(Color color)
        {
            stats.currentColor = color;

            if (duckRenderer != null)
            {
                duckRenderer.material.color = color;
            }

            Debug.Log($"Duck color changed to {GetColorName(color)}");
            UpdateUI();
        }

        // VECTOR3 command
        void MoveToPond(Vector3 position)
        {
            transform.position = position;
            stats.position = position;
            stats.currentPond = "Pond Area";

            // Bonus corn for moving to pond
            stats.corn += 5;

            Debug.Log($"Duck moved to {position}");

            PlayQuack();
            UpdateUI();
        }

        // CUSTOM TYPE: DuckReward
        void GiveReward(DuckReward reward)
        {
            Debug.Log("Reward received!");

            // Apply corn
            stats.corn += reward.cornAmount;
            Debug.Log($"Corn: +{reward.cornAmount}");

            // Apply color
            if (reward.duckColor != null)
            {
                Color color = reward.duckColor.ToColor();
                stats.currentColor = color;

                if (duckRenderer != null)
                {
                    duckRenderer.material.color = color;
                }
            }

            // Show message
            if (!string.IsNullOrEmpty(reward.message))
            {
                Debug.Log($"Message: {reward.message}");
            }

            // Apply volume boost
            if (reward.volumeBoost > 0)
            {
                stats.quackVolume = Mathf.Clamp01(stats.quackVolume + reward.volumeBoost);

                if (quackAudioSource != null)
                {
                    quackAudioSource.volume = stats.quackVolume;
                }

                Debug.Log($"Volume increased by {reward.volumeBoost * 100:F0}%");
            }

            PlayQuack();  // ← Quack for reward!
            UpdateUI();
        }

        // CUSTOM TYPE: PondInfo
        void TeleportToPond(PondInfo pondInfo)
        {
            Vector3 position = pondInfo.position.ToVector3();
            transform.position = position;
            stats.position = position;
            stats.currentPond = pondInfo.pondName;
            stats.corn += pondInfo.cornBonus;

            Debug.Log($"Teleported to {pondInfo.pondName}");
            Debug.Log($"Received {pondInfo.cornBonus} bonus corn!");
            PlayQuack();
            UpdateUI();
        }

        // CUSTOM TYPE: EventData
        void StartEvent(EventData eventData)
        {
            stats.inEvent = true;
            stats.activeEvent = eventData.eventName;
            stats.corn += eventData.rewardCorn;

            // Change color if specified
            if (eventData.specialColor != null)
            {
                ChangeDuckColor(eventData.specialColor.ToColor());
            }

            // Handle particles
            if (eventData.showParticles && eventParticles != null)
            {
                // Customize particle color based on event
                if (eventData.specialColor != null)
                {
                    var main = eventParticles.main;
                    main.startColor = eventData.specialColor.ToColor();
                }

                eventParticles.Play();
                Debug.Log("Particles started!");
            }
            else if (eventData.showParticles && eventParticles == null)
            {
                Debug.LogWarning("Particles requested but no ParticleSystem assigned!");
            }

            Debug.Log($"Event started: {eventData.eventName}");
            Debug.Log($"Reward: {eventData.rewardCorn} corn");
            Debug.Log($"Duration: {eventData.durationSeconds}s");


            PlayQuack();
            UpdateUI();

            // Auto-end event after duration
            Invoke(nameof(EndEvent), eventData.durationSeconds);
        }

        void EndEvent()
        {
            Debug.Log($"Event ended: {stats.activeEvent}");
            stats.inEvent = false;
            stats.activeEvent = "None";
            // Stop particles
            if (eventParticles != null && eventParticles.isPlaying)
            {
                eventParticles.Stop();
                Debug.Log("Particles stopped");
            }
            UpdateUI();
        }

        #endregion

        #region Audio System
        /// <summary>
        /// Play quack sound at current volume
        /// </summary>
        private void PlayQuack()
        {
            if (quackAudioSource != null && quackAudioSource.clip != null)
            {
                quackAudioSource.Play();
                Debug.Log($"Quack! (Volume: {stats.quackVolume * 100:F0}%)");
            }
        }



        #endregion

        #region Helpers

        private void UpdateUI()
        {
            if (uiManager != null)
            {
                uiManager.UpdateDisplay(stats);
            }
        }

        private string GetColorName(Color color)
        {
            if (color == Color.yellow) return "Yellow";
            if (color == Color.white) return "White";
            if (color == Color.cyan) return "Cyan";
            if (color == Color.blue) return "Blue";
            if (color == Color.green) return "Green";
            if (color == Color.red) return "Red";
            if (color == Color.magenta) return "Magenta";
            if (color == Color.black) return "Black";

            // Gold
            if (Mathf.Approximately(color.r, 1.0f) &&
                Mathf.Approximately(color.g, 0.84f) &&
                Mathf.Approximately(color.b, 0.0f))
            {
                return "Gold";
            }

            // Pink
            if (color.r > 0.9f && color.g > 0.7f && color.b > 0.7f)
            {
                return "Pink";
            }

            return $"RGB({color.r:F2}, {color.g:F2}, {color.b:F2})";
        }

        #endregion

       
    }
}