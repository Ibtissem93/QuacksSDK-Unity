# Technical Documentation - Usage Guide

Quick guide to understand and test the Quacks SDK project.

---

## What This Project Does

The SDK lets a game server send JSON commands to Unity, and Unity automatically executes the right functions.

**Example:** Server sends `{"command":"FeedDuck","parameters":{"value":10}}` → Duck gets 10 corn

---


## How It Works

### Step 1: Register Commands (DuckController.cs)

When the game starts, register which commands the server can call:
```csharp
void Start()
{
    QuacksSDK sdk = QuacksSDK.Instance;
    
    // Register commands
    sdk.RegisterCommand<int>("FeedDuck", FeedDuck);
    sdk.RegisterCommand<Color>("ChangeDuckColor", ChangeDuckColor);
    // ... more commands
}

// Define what happens when command is received
void FeedDuck(int amount)
{
    stats.corn += amount;
    UpdateUI();
}
```

---

### Step 2: Server Sends JSON (MockServer.cs)

The server (or MockServer for testing) sends a JSON message:
```json
{
  "command": "FeedDuck",
  "parameters": {
    "value": 10
  }
}
```

---

### Step 3: SDK Processes Message (QuacksSDK.cs)

The SDK automatically:
1. Parses the JSON
2. Looks up "FeedDuck" in the registry
3. Converts parameters to the right type (int)
4. Calls `FeedDuck(10)`

---

### Step 4: Game Updates

The `FeedDuck()` function runs, updates the duck's corn count, and refreshes the UI.

---

## How to Test

### Open the Demo Scene

1. Open `DemoScene.unity`
2. Press Play
3. You'll see a duck and UI showing stats
4. Click any button and watch the duck respond!

---

## Script Explanations

### Core SDK Scripts

#### **QuacksSDK.cs**
Main SDK class. Handles everything:
- Receives JSON messages from server
- Parses JSON and extracts command name
- Looks up command in registry
- Converts parameters to correct C# types
- Executes the registered function
- Error handling (unknown commands, invalid JSON, wrong types)

**Key Methods:**
- `RegisterCommand<T>(name, callback)` - Register a command
- `ProcessServerMessage(json)` - Process server JSON

---

#### **CommandRegistry.cs**
Stores all registered commands in a Dictionary.
- Fast lookup (O(1))
- Maps command names to functions
- Example: `"FeedDuck"` → `FeedDuck()` function

---

#### **TypeConverter.cs**
Converts JSON to C# objects using Newtonsoft.Json.
- Handles Unity types (Vector3, Color)
- Handles custom types (DuckReward, EventData)

---

#### **ParameterWrappers.cs**
Wrapper classes for primitives because JSON needs objects.
- `IntParameter` wraps `int`
- `FloatParameter` wraps `float`
- `Vector3Parameter` wraps Vector3 as `{x, y, z}`
- `ColorParameter` wraps Color as `{r, g, b, a}`

---

#### **ServerCommand.cs**
Simple data structure representing a server command.
```csharp
{
    string command;      // Command name
    object parameters;   // Command data
}
```

---

### Demo Scripts

#### **DuckController.cs**
Example game that uses the SDK.
- Registers 7 commands in `Start()`
- Implements command handlers (FeedDuck, ChangeDuckColor, etc.)
- Updates UI after each command
- Shows how to use primitives, Unity types, and custom types

**Registered Commands:**
1. `FeedDuck(int)` - Add corn
2. `SetQuackVolume(float)` - Change volume
3. `ChangeDuckColor(Color)` - Change color
4. `MoveToPond(Vector3)` - Move duck
5. `GiveReward(DuckReward)` - Give bundled reward
6. `TeleportToPond(PondInfo)` - Teleport to pond
7. `StartEvent(EventData)` - Start timed event

---

#### **UIManager.cs**
Updates UI text elements to show duck stats.
- Displays corn count, color, volume, pond, event
- Called by DuckController after each command

---

#### **DuckStats.cs**
Holds the duck's current state.
```csharp
{
    int corn;
    Color currentColor;
    float quackVolume;
    string currentPond;
    Vector3 position;
    string activeEvent;
    bool inEvent;
}
```

---

### Testing Scripts

#### **MockServer.cs**
Simulates a real game server for testing.

**Two Modes:**
1. **File I/O Mode** (`useFileIO = true`)
   - Loads commands from `ServerCommands.json`
   - Good for testing with realistic server data

2. **In-Memory Mode** (`useFileIO = false`)
   - Generates commands in code
   - Faster for quick testing

**Features:**
- Stores list of pre-made commands
- Can send random command or specific command by name
- Has multiple variants (e.g., FeedDuck with 10, 25, or 50 corn)

---

#### **CommandBuilder.cs**
Helper to create ServerCommand objects easily.

**Instead of writing:**
```csharp
new ServerCommand("FeedDuck", new { value = 10 })
```

**Just write:**
```csharp
CommandBuilder.FeedDuck(10)
```

Much cleaner!

---

### Custom Type Scripts

#### **DuckReward.cs**
Bundles multiple values into one parameter.
```csharp
{
    int cornAmount;
    ColorData duckColor;
    string message;
    float volumeBoost;
}
```

Shows how to send complex data in one command.

Also includes `ColorData` - a JSON-compatible version of Unity's Color.

---

#### **EventData.cs**
Data for game events.
```csharp
{
    string eventName;
    int rewardCorn;
    ColorData specialColor;
    float durationSeconds;
    bool showParticles;
}
```

---

#### **PondInfo.cs**
Pond location data.
```csharp
{
    Vector3Data position;
    string pondName;
    int cornBonus;
}
```

Also includes `Vector3Data` - a JSON-compatible version of Unity's Vector3.

---

## Testing Workflow

### Quick Test
1. Open `DemoScene`
2. Press Play
3. Click "Send Random Command" button
4. Watch duck respond

### Test Specific Commands
1. Click specific buttons (Feed Duck, Change Color, etc.)
2. Watch Console logs to see what's happening
3. Watch UI update with new values


---

## Supported Data Types

| Type | Example JSON | Used For |
|------|-------------|----------|
| **int** | `{"value": 10}` | Numbers, amounts |
| **float** | `{"value": 0.5}` | Decimals, percentages |
| **string** | `{"value": "hello"}` | Text, messages |
| **Vector3** | `{"x": 5, "y": 0, "z": 3}` | Positions, movement |
| **Color** | `{"r": 1, "g": 0, "b": 0, "a": 1}` | Colors |
| **Custom** | `{...multiple fields...}` | Complex data |

---

## What to Look For

When testing, observe:

1. **Console Logs**
   - `[Quacks SDK] Initialized` - SDK started
   - `[Registry] -- Registered: 'FeedDuck'` - Commands registered
   - `[MockServer] Sending: FeedDuck` - Command sent
   - `[Quacks SDK] Executed: FeedDuck` - Command executed
   - `Fed duck 10 corn! Total: 10` - Game logic ran

2. **UI Updates**
   - Corn count increases
   - Color name changes
   - Volume percentage updates
   - Pond name changes
   - Event status shows

3. **Visual Changes**
   - Duck changes color
   - Duck moves position
   - Particles appear during events

---


**Just open DemoScene and press Play to see it work!** 