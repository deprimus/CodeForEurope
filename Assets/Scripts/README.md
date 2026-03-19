# 📜 Scripts Folder

This folder contains the core scripts that power the main gameplay, UI, and systems for the CodeForEurope project. The scripts are organized by functionality, with a strong focus on modularity and extensibility for educational and storytelling purposes.

**Content outside Unity:** designers and educators can edit structured game data in the browser with the [CodeForEurope Data Editor](https://deprimus.github.io/CodeForEurope/) (loads `Assets/Resources/GameData/game_database.json`). Creating **new** 3D NPC prefabs remains a Unity workflow; the JSON maps **existing** `prefabPath` entries to loaded resources.

## 🧩 Key Functionalities

### 1. 🔧 NPC Creator Component
- **Scripts:** `NPCWindow.cs`, `NPC.cs`, `NPCManager.cs`, `NPCView.cs`
- **Purpose:** Allows designers to create new NPCs, define **value orientations** (internal `FactionType` indices), visuals, names, traits, and behavior profiles. The `NPCWindow` Unity Editor tool provides a user-friendly interface for creating and managing NPCs as ScriptableObjects and prefabs. A fuller NPC authoring path on the web is **work in progress**.

### 2. 💬 NPC Interaction Creator
- **Scripts:** `NPCInteractionWindow.cs`, `NPCInteraction.cs`, `NPCManager.cs`, `NPCView.cs`
- **Purpose:** Enables dialogue for office visits, player–NPC interactions, and typed **interaction effects** (`InteractionEffectType`). The `NPCInteractionWindow` Editor tool is used to author lines, effects, and links to NPCs.

### 3. 🏛️ Law Manager & welfare system
- **Scripts:** `LawWindow.cs`, `LawManager.cs`, `UIView_Law.cs`, `UIView_BeaureauLaw.cs`, `WelfareManager.cs`, `RoundTableManager.cs`
- **Purpose:** Tools to add law proposals, descriptions, icons (when used), **law effects** (orientation shifts), and **welfare effects** (`WelfareIndicator`: GDP, Gini, human capital, life expectancy). `WelfareManager` updates tracked societal indicators after votes and supports end-game welfare-based messaging.

### 4. 🗃️ Game database (JSON)
- **Scripts:** `GameDatabase.cs`, `GameDatabaseJson.cs`
- **Purpose:** At runtime, `GameDatabase` loads `Resources/GameData/game_database.json`, builds NPCs, interactions, laws (including `welfareEffects`), and indexes **EuroChat** `posts` by law name. Enum metadata in `fieldDetails` documents numeric types for authors. The JSON file may also include an **`opinions`** array for tooling and the web editor; extend `GameDatabaseRoot` if you need that data in play mode.

## 🗂️ File Organization

- **Editor Tools:**
  - `NPCWindow.cs` – Editor window for creating and managing NPCs.
  - `NPCInteractionWindow.cs` – Editor window for creating and managing NPC interactions.
  - `LawWindow.cs` – Editor window for creating and managing laws.

- **Core Data & Logic:**
  - `NPC.cs` – ScriptableObject representing an NPC.
  - `NPCManager.cs` – Holds and manages all NPC interactions.
  - `NPCInteraction.cs` – ScriptableObject for a single NPC interaction (dialogue, effects).
  - `LawManager.cs` – Manages laws, current law effects, and references to interactions.
  - `GameDatabase.cs` / `GameDatabaseJson.cs` – JSON schema and loading for `game_database.json`.
  - `WelfareManager.cs` – Societal indicators and welfare outcomes after votes.

- **UI & Gameplay:**
  - `NPCView.cs` – Handles NPC visuals and in-game behavior.
  - `UIView_Law.cs`, `UIView_BeaureauLaw.cs` – UI for displaying law information and effects.

- **Other Scripts:**
  - Additional scripts handle supporting systems (sound, transitions, state management, etc.) and are organized by their respective gameplay or UI function.

## ✨ Highlighted Scripts

- **NPCWindow.cs:** Main tool for creating and editing NPCs, including their traits and orientations.
- **NPCInteractionWindow.cs:** Tool for building dialogue trees and defining consequences for player choices.
- **LawWindow.cs:** Tool for adding and editing laws, linking them to NPC interactions, and defining law and welfare effects.

## 🗃️ Managers

This section describes the main manager scripts that coordinate the core systems and scene flow in the project. Each manager is responsible for a specific aspect of the game, from scene transitions to law and NPC management.

- **GameManager.cs:** The main entry point for the game. Handles scene activation, round progression, and coordinates other managers (LawManager, NPCManager, WelfareManager, etc.). Maintains global game state and orientation-influence scoring.
- **LawManager.cs:** Manages all laws in the game, including their effects and the current law in play. Provides methods to pick, set, and update laws and their effects.
- **WelfareManager.cs:** Tracks GDP, Gini, human capital, and life expectancy; applies `WelfareEffect` lists from laws; drives indicator UI and composite welfare endings.
- **GameDatabase.cs:** Loads and parses `game_database.json`; constructs runtime NPCs, interactions, laws (with welfare effects), and EuroChat post lookup.
- **LibraryManager.cs:** Handles the library scene, including tracking player actions (book/laptop usage), storing NPC interactions, and managing the UI for reviewing past choices and debunking misinformation.
- **RoundTableManager.cs:** Controls the round table scene, showing law cards, managing faction moods, and handling voting and influence mechanics.
- **BeaureauManager.cs:** Manages the office scene, NPC queue, and the flow of NPC interactions and choices. Coordinates with LawManager and RoundTableManager.
- **StateManager.cs:** Controls scene transitions and the current state of the game (e.g., RoundTable, Bureau, Library, GameEnd).
- **SoundManager.cs:** Centralizes audio playback for UI and game events, ensuring consistent sound effects and music.
- **GameEndManager.cs:** Handles the end-of-game sequence, displaying endings based on player choices, orientation influence, and welfare outcomes.
- **CameraManager.cs:** Manages the main camera reference and switching between camera views as needed.

---

For more details on each main script, see the documentation at the top of each file. Supporting scripts are named to be self-explanatory and follow Unity conventions.

---

**Note:** The `Tale` script is a third-party utility for prop manipulation and is not documented here. See [Tale on GitHub](https://github.com/deprimus/Tale) for details.
