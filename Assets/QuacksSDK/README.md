# Quacks SDK - Remote Command Execution

A Unity SDK that lets game servers send commands to game clients safely.

---

## What It Does

The SDK allows your game server to trigger functions in Unity by sending JSON messages.

**Example:**
- Server sends: `{"command":"FeedDuck","parameters":{"value":10}}`
- Unity executes: `FeedDuck(10)` function
- Duck gets 10 corn!

---

## Quick Start

### 1. Add SDK to Your Scene

Add a GameObject with the `QuacksSDK` script (it will persist across scenes automatically).

### 2. Register Your Commands
```csharp
using SDK;

public class MyGame : MonoBehaviour
{
    void Start()
    {
        QuacksSDK sdk = QuacksSDK.Instance;
        
        // Register commands
        sdk.RegisterCommand<int>("FeedDuck", FeedDuck);
        sdk.RegisterCommand<Color>("ChangeColor", ChangeColor);
    }
    
    void FeedDuck(int amount)
    {
        Debug.Log($"Fed duck {amount} corn!");
        // Your game logic here
    }
    
    void ChangeColor(Color newColor)
    {
        Debug.Log($"Changed color to {newColor}");
        // Your game logic here
    }
}
```

### 3. Process Server Messages
```csharp
// When you receive JSON from server
string json = "{\"command\":\"FeedDuck\",\"parameters\":{\"value\":10}}";
sdk.ProcessServerMessage(json);
```

Done! The SDK automatically finds and executes the right function.

---

## Supported Types

**Primitives:**
- `int`, `float`, `string`

**Unity Types:**
- `Vector3`, `Color`

**Custom Types:**
```csharp
[System.Serializable]
public class DuckReward
{
    public int cornAmount;
    public ColorData duckColor;
    public string message;
}

sdk.RegisterCommand<DuckReward>("GiveReward", GiveReward);
```

---

## Testing

Use `MockServer` to test without a real server:

1. Add `MockServer` script to a GameObject
2. Choose mode in Inspector:
   - **useFileIO = true**: Loads commands from `ServerCommands.json`
   - **useFileIO = false**: Generates test commands in code


---

## Project Structure
```
QuacksSDK/
├── Scripts/
│   ├── SDK/              # Core SDK (QuacksSDK, CommandRegistry, TypeConverter)
│   ├── Testing/          # MockServer, CommandBuilder
│   ├── Demo/             # Example game (DuckController, UIManager)
│   └── CustomTypes/      # Custom data types (DuckReward, EventData, etc.)
│
├── Scenes/
│   └── DemoScene         # Working example
│
├── Audio/
│   └── quack.wav         # Sound effect
│
└── Materials/
    └── ...               # Visual materials
```

---

## JSON Format

### Simple Command
```json
{
  "command": "FeedDuck",
  "parameters": {
    "value": 10
  }
}
```

### Complex Command
```json
{
  "command": "GiveReward",
  "parameters": {
    "cornAmount": 50,
    "duckColor": {"r": 1.0, "g": 0.84, "b": 0.0, "a": 1.0},
    "message": "Daily Bonus!",
    "volumeBoost": 0.2
  }
}
```

---


## Demo Scene

Open `DemoScene` to see the SDK in action:
- Duck game with feeding, color changes, teleporting
- UI showing current stats
- MockServer with test buttons

---

## Requirements

- Unity 6.2 or later
- Newtonsoft.Json package

---

## Example Commands

| Command | Parameters | What It Does |
|---------|------------|--------------|
| FeedDuck | int amount | Adds corn to duck |
| ChangeDuckColor | Color color | Changes duck color |
| MoveToPond | Vector3 position | Moves duck to position |
| SetQuackVolume | float volume | Changes sound volume |
| GiveReward | DuckReward reward | Gives bundled reward |

---

## How It Works

1. **Register** commands when game starts
2. **Server** sends JSON message
3. **SDK** parses JSON and finds command
4. **SDK** converts parameters to correct types
5. **SDK** executes your function
6. **Game** logic runs!

---

## Error Handling

The SDK handles:
- Unknown commands → Logs error, suggests similar commands
- Invalid JSON → Logs parsing error
- Wrong parameter types → Logs conversion error
- Missing parameters → Logs validation error
- Command execution errors → Catches and logs, continues running

---

