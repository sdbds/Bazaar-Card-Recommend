# BazaarRecommend

A BepInEx mod for The Bazaar that overlays tier rating badges on item cards, sourced from the [Unduel BazaarDB all-items tier list](https://unduel.com/u/bazaardb/all-items-in-the-bazaar-2p1gJ5h503y0TBHOLeR5RZ/tier-list).

![Preview](sample.png)

Badge colors by tier:

| Tier | Color  |
|------|--------|
| S    | Gold   |
| A    | Orange |
| B    | Green  |
| C    | Blue   |
| D    | Gray   |
| F    | White  |

---

## Prerequisite: Install BazaarPlusPlus

This mod requires BepInEx. The easiest way to get it is by installing [BazaarPlusPlus](https://github.com/cauyxy/BazaarPlusPlus) first — it ships with BepInEx and sets everything up automatically.

---

## Option 1: Download the DLL (Recommended)

1. Go to the [Releases](../../releases) page and download the latest `bazaar-recommend.dll`
2. Place the DLL in your game's BepInEx plugins folder:

   **Steam**
   ```
   C:\Program Files (x86)\Steam\steamapps\common\The Bazaar\BepInEx\plugins\
   ```

   **Tempo Launcher**
   ```
   C:\Users\<username>\AppData\Roaming\Tempo Launcher - Beta\game\buildx64\BepInEx\plugins\
   ```

3. Launch the game. Tier badges will appear in the bottom-right corner of item cards during a match.

---

## Option 2: Build from Source

### Requirements

- [.NET SDK 6+](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or Rider (optional)
- The Bazaar with [BazaarPlusPlus](https://github.com/cauyxy/BazaarPlusPlus) installed (includes BepInEx)

### Steps

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd BazaarPlannerMod
   ```

2. Open `BazaarRecommend.csproj` and update the game path to match your installation:
   ```xml
   <SteamPath>D:\Program Files (x86)\Steam\steamapps\common\The Bazaar</SteamPath>
   ```

3. Build in Release mode:
   ```bash
   dotnet build -c Release
   ```
   The compiled DLL will be automatically copied to `BepInEx\plugins\bazaar-recommend.dll` in your game directory.

4. Launch the game.

### Updating Tier Data

Tier data is embedded from `Data/tierlist.json`, extracted from Unduel's all-items tier list and aligned to local game card GUIDs. To update ratings, edit the JSON file and rebuild.

---

## Credits

- [BazaarPlusPlus](https://github.com/cauyxy/BazaarPlusPlus) — BepInEx environment and mod framework
- [BazaarPlannerMod](https://github.com/oceanseth/BazaarPlannerMod) — project structure reference
