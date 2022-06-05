# Third Person Camera

A third person camera mod for VRising. For lack of a better name, it includes a combat mode which will lock the camera as if you were holding the rotate camera mouse button. While in combat mode your character will always face and attack in the direction you are looking.

### Installation

- Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/).
- Install [Wetstone](https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/).
- Extract _ThirdPersonCamera.dll_ into _`(VRising folder)/BepInEx/plugins`_.

### Configuration

The combat mode keybinding can be changed from within the game.

### Support

Join the [V Rising Mod Community](https://discord.gg/r87Vdez2Br), and ping `@iZastic#0365`.

Post an issue on [GitHub](https://github.com/iZastic/vrising-thirdpersoncamera/issues).

### Known Issues
- Zooming in to far causes some world space UI elements to become fully transparent
- Has not been tested with abilities that target a location on the ground (I don't have any yet)

### Changelog

- **1.1.2**
    - Fixed an issue with camera twitching
    - Fixed an issue where look offsets were only applied at 50% value
- **1.1.1**
    - Fixed an issue with right clicking while a menu is open
- **1.0.0**
    - Added config options
    - Added aim offsets so the character isn't always hovered
    - Fixed action and emote wheel in combat mode
- **1.0.0**
    - Initial release