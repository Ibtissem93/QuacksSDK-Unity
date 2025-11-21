using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    /// <summary>
    /// Manages UI display for duck stats
    /// Updates text elements to show current duck state
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Text References")]
        [Tooltip("Text element for displaying corn count")]
        public Text cornText;

        [Tooltip("Text element for displaying duck color")]
        public Text colorText;

        [Tooltip("Text element for displaying quack volume")]
        public Text volumeText;

        [Tooltip("Text element for displaying current pond")]
        public Text pondText;

        [Tooltip("Text element for displaying active event")]
        public Text eventText;

        [Header("Display Settings")]
        [Tooltip("Prefix for corn display")]
        public string cornPrefix = "Corn: ";

        [Tooltip("Prefix for color display")]
        public string colorPrefix = "Color: ";

        [Tooltip("Prefix for volume display")]
        public string volumePrefix = "Volume: ";

        [Tooltip("Prefix for pond display")]
        public string pondPrefix = "Pond: ";

        [Tooltip("Prefix for event display")]
        public string eventPrefix = "Event: ";

        //===============================================
        // UPDATE DISPLAY
        //===============================================

        /// <summary>
        /// Update all UI elements with current duck stats
        /// </summary>
        /// <param name="stats">Current duck stats to display</param>
        public void UpdateDisplay(DuckStats stats)
        {
            if (stats == null)
            {
                Debug.LogWarning("UIManager: Received null stats!");
                return;
            }

            // Update corn
            if (cornText != null)
            {
                cornText.text = $"{cornPrefix}{stats.corn}";
            }

            // Update color
            if (colorText != null)
            {
                string colorName = GetColorName(stats.currentColor);
                colorText.text = $"{colorPrefix}{colorName}";
            
                // Optional: Change text color to match duck color
                colorText.color = stats.currentColor;
            }

            // Update volume
            if (volumeText != null)
            {
                int percentage = Mathf.RoundToInt(stats.quackVolume * 100);
                volumeText.text = $"{volumePrefix}{percentage}%";
            }

            // Update pond
            if (pondText != null)
            {
                pondText.text = $"{pondPrefix}{stats.currentPond}";
            }

            // Update event
            if (eventText != null)
            {
                if (stats.inEvent)
                {
                    eventText.text = $"{eventPrefix}{stats.activeEvent}";
                    eventText.color = Color.yellow; // Highlight active events
                }
                else
                {
                    eventText.text = $"{eventPrefix}None";
                    eventText.color = Color.white;
                }
            }
        }

        //===============================================
        // UTILITY METHODS
        //===============================================

        /// <summary>
        /// Convert Color to friendly name for display
        /// Same logic as DuckController for consistency
        /// </summary>
        string GetColorName(Color color)
        {
            // Check common colors
            if (color == Color.yellow) return "Yellow";
            if (color == Color.white) return "White";
            if (color == Color.cyan) return "Cyan";
            if (color == Color.blue) return "Blue";
            if (color == Color.green) return "Green";
            if (color == Color.red) return "Red";
            if (color == Color.magenta) return "Magenta";
            if (color == Color.black) return "Black";

            // Check gold (approximately)
            if (Mathf.Approximately(color.r, 1.0f) && 
                Mathf.Approximately(color.g, 0.84f) && 
                Mathf.Approximately(color.b, 0.0f))
            {
                return "Gold";
            }

            // Check pink (approximately)
            if (color.r > 0.9f && color.g > 0.7f && color.b > 0.7f)
            {
                return "Pink";
            }

            // Default: show RGB values
            return $"RGB({color.r:F2}, {color.g:F2}, {color.b:F2})";
        }

        //===============================================
        // INITIALIZATION
        //===============================================

        void Start()
        {
            // Validate all references
            ValidateReferences();
        }

        /// <summary>
        /// Check if all UI references are assigned
        /// </summary>
        void ValidateReferences()
        {
            bool hasErrors = false;

            if (cornText == null)
            {
                Debug.LogWarning("UIManager: cornText is not assigned!");
                hasErrors = true;
            }

            if (colorText == null)
            {
                Debug.LogWarning("UIManager: colorText is not assigned!");
                hasErrors = true;
            }

            if (volumeText == null)
            {
                Debug.LogWarning("UIManager: volumeText is not assigned!");
                hasErrors = true;
            }

            if (pondText == null)
            {
                Debug.LogWarning("UIManager: pondText is not assigned!");
                hasErrors = true;
            }

            if (eventText == null)
            {
                Debug.LogWarning("UIManager: eventText is not assigned!");
                hasErrors = true;
            }

            if (!hasErrors)
            {
                Debug.Log("UIManager: All UI references assigned!");
            }
        }

        //===============================================
        // MANUAL UPDATE (For Testing)
        //===============================================

        /// <summary>
        /// Manually trigger UI update with test data
        /// Can be called from Inspector for testing
        /// </summary>
        [ContextMenu("Test UI Update")]
        void TestUIUpdate()
        {
            DuckStats testStats = new DuckStats
            {
                corn = 123,
                currentColor = Color.cyan,
                quackVolume = 0.75f,
                currentPond = "Test Pond",
                activeEvent = "Test Event",
                inEvent = true
            };

            UpdateDisplay(testStats);
            Debug.Log("UI updated with test data!");
        }
    }
}