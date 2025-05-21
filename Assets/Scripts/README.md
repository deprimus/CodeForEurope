# 📜 Scripts Folder

This folder contains the core scripts that power the main gameplay, UI, and systems for the CodeForEurope project. The scripts are organized by functionality, with a strong focus on modularity and extensibility for educational and storytelling purposes.

## 🧩 Key Functionalities

### 1. 🔧 NPC Creator Component
- **Scripts:** `NPCWindow.cs`, `NPC.cs`, `NPCManager.cs`, `NPCView.cs`
- **Purpose:** Allows designers to create new NPCs, define their political orientation, visuals, names, traits, and behavior profiles. The `NPCWindow` Unity Editor tool provides a user-friendly interface for creating and managing NPCs as ScriptableObjects and prefabs.

### 2. 💬 NPC Interaction Creator
- **Scripts:** `NPCInteractionWindow.cs`, `NPCInteraction.cs`, `NPCManager.cs`, `NPCView.cs`
- **Purpose:** Enables the creation of dialogue trees, player-NPC interactions, and the definition of reactions and consequences for player choices. The `NPCInteractionWindow` Editor tool is used to author dialogue, set up branching outcomes, and link interactions to NPCs.

### 3. 🏛️ Law Manager
- **Scripts:** `LawWindow.cs`, `LawManager.cs`, `UIView_Law.cs`, `UIView_BeaureauLaw.cs`
- **Purpose:** Provides tools to add new law proposals, define their properties (title, description, icon), attach effects, and link to NPC interactions. The `LawWindow` Editor tool is central for managing laws and their impact on the game world.

## 🗂️ File Organization

- **Editor Tools:**
  - `NPCWindow.cs` – Editor window for creating and managing NPCs.
  - `NPCInteractionWindow.cs` – Editor window for creating and managing NPC interactions.
  - `LawWindow.cs` – Editor window for creating and managing laws.

- **Core Data & Logic:**
  - `NPC.cs` – ScriptableObject representing an NPC.
  - `NPCManager.cs` – Holds and manages all NPC interactions.
  - `NPCInteraction.cs` – ScriptableObject for a single NPC interaction (dialogue, effects).
  - `LawManager.cs` – ScriptableObject for managing laws and their effects.

- **UI & Gameplay:**
  - `NPCView.cs` – Handles NPC visuals and in-game behavior.
  - `UIView_Law.cs`, `UIView_BeaureauLaw.cs` – UI for displaying law information and effects.

- **Other Scripts:**
  - Additional scripts handle supporting systems (sound, transitions, state management, etc.) and are organized by their respective gameplay or UI function.

## ✨ Highlighted Scripts

- **NPCWindow.cs:** Main tool for creating and editing NPCs, including their traits and orientations.
- **NPCInteractionWindow.cs:** Tool for building dialogue trees and defining consequences for player choices.
- **LawWindow.cs:** Tool for adding and editing laws, linking them to NPC interactions, and defining their effects on factions and gameplay.

## 🗃️ Managers

This section describes the main manager scripts that coordinate the core systems and scene flow in the project. Each manager is responsible for a specific aspect of the game, from scene transitions to law and NPC management.

- **GameManager.cs:** The main entry point for the game. Handles scene activation, round progression, and coordinates other managers (LawManager, NPCManager, etc.). Maintains global game state and points for each faction.
- **LawManager.cs:** Manages all laws in the game, including their effects and the current law in play. Provides methods to pick, set, and update laws and their effects.
- **LibraryManager.cs:** Handles the library scene, including tracking player actions (book/laptop usage), storing NPC interactions, and managing the UI for reviewing past choices and debunking misinformation.
- **RoundTableManager.cs:** Controls the round table scene, showing law cards, managing faction moods, and handling voting and influence mechanics.
- **BeaureauManager.cs:** Manages the office scene, NPC queue, and the flow of NPC interactions and choices. Coordinates with LawManager and RoundTableManager.
- **StateManager.cs:** Controls scene transitions and the current state of the game (e.g., RoundTable, Bureau, Library, GameEnd).
- **SoundManager.cs:** Centralizes audio playback for UI and game events, ensuring consistent sound effects and music.
- **GameEndManager.cs:** Handles the end-of-game sequence, displaying the appropriate ending based on player choices and faction outcomes.
- **CameraManager.cs:** Manages the main camera reference and switching between camera views as needed.

---

For more details on each main script, see the documentation at the top of each file. Supporting scripts are named to be self-explanatory and follow Unity conventions.

---

**Note:** The `Tale` script is a third-party utility for prop manipulation and is not documented here. See [Tale on GitHub](https://github.com/deprimus/Tale) for details.
