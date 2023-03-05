# NeoFPS_EmeraldAI
NeoFPS and Emerald AI 3 integration assets

## Requirements
This repository was created using Unity 2019.4

It requires the assets [NeoFPS](https://assetstore.unity.com/packages/templates/systems/neofps-150179?aid=1011l58Ft) and [Emerald AI](https://assetstore.unity.com/packages/tools/ai/emerald-ai-3-0-203904?aid=1011l58Ft).

Last updated with version 1.1.24 of NeoFPS and 3.2.1 of Emerald AI. **If you need an integration for Emerald AI 2, you can use the branches and tags dropdown to select the Emerald AI 2 branch.**

## Installation
This integration example is intended to be dropped in to a fresh project along with NeoFPS and Emerald AI.

1. Import NeoFPS and apply the required Unity settings using the NeoFPS Settings Wizard. You can find more information about this process [here](https://docs.neofps.com/manual/neofps-installation.html).

2. Import the Emerald AI asset.

3. Clone this repository to a folder inside the project Assets folder such as "NeoFPS_EmeraldAI"

4. Remove the **EmeraldAIPlayerDamage.cs** script located in **Emerald AI/Scripts/Components** to fix the duplicate script error (you want to use the NeoFPS script)
	
## Integration
For the most part, this integration only requires that you modify the AI characters and replace the player damage script as mentioned above to work. The NeoFPS weapons do not need any modifications to work once the AI have been correctly set up. The NeoFPS player character needs an **NeoFpsEmeraldAI_TargetPositionModifier** behaviour adding to its root if you are using ranged AI enemies. This will help them aim at your head / upper body when you stand or crouch.

The following are the important assets in this repo that enable NeoFPS and Emerald AI to work side by side.

#### MonoBehaviours
There is a new custom **EmeraldAIPlayerDamage.cs** in **NeoFPS_EmeraldAI/Scripts** which replaces the one in **Emerald AI/Scripts/Player**. This is a simple behaviour that Emerald AI characters add to any targets they attack in order to communicate AI to player damage. This script applies player damage as well as providing information about the damage source to the NeoFPS character, and triggering hit effects such as camera knock. You can add this to the NeoFPS player character if you want, but it will be added automatically when you are first attacked if not.

The script **NeoFpsEmeraldAI_DamageHandler.cs** located in **NeoFPS_EmeraldAI/Scripts** is a NeoFPS damage handler which passes damage to the Emerald AI character as well as letting the character know the damage source. This must be added to all Emerald AI characters so the player can damage them, but only if the AI is not using location based damage (damage handlers per limb).

**NeoFpsEmeraldAI_LocationBasedDamageHandler.cs** located in **NeoFPS_EmeraldAI/Scripts** is a NeoFPS damage handler specifically for AI that use location based damage. It passes the damage to the AI character's `LocationBasedDamageArea` components instead of the root `EmeraldAISystem` so that separate damage multipliers can be applied.

The script **NeoFpsEmeraldAI_AsyncLoadFixer.cs** is required for Emerald AI to work with async scene loading. This should be present in every scene and the easiest way is by adding it to any Emerald AI characters.

**NeoFpsEmeraldAI_AISoundDetection.cs** is used to add audio based detection to the AI when the player fires a weapon.

**EmeraldAISystemFormatter.cs** is a simple data formatter that allows you to save basic AI information (eg alive/dead).

**NeoFpsEmeraldAI_InitializerOverride.cs** is a legacy script that made it possible to support location based damage before Emerald AI had its own system added. It is now preferred to use the `NeoFpsEmeraldAI_LocationBasedDamageHandler` as listed above.

Lastly, **NeoFpsEmeraldAI_TargetPositionModifier.cs** is ties into the player character's character controller and tracks when it crouches or stands. It uses this to notify ranged AI and AI that look at the player how high to aim.

#### Demo Scene
The demo scene is a basic prototype scene containing 2 types of AI, both adapted from prefabs provided with Emerald AI. The first is a "Chomper" creature that is aggressive towards the player. This will approach and attack as soon as it sees you. The other is the "Ellen" character. She is neutral towards the player and chompers, and is just provided as an example of how to use the newer location based damage system. You can find both of the prefabs in this repository's *Prefabs* sub-folder.

#### AI Setup
The following are the important settings to apply to Emerald AI that work with NeoFPS:
- Switch tag on root object to "AI" and layer to "CharacterControllers"
- On **EmeraldAISystem** component:
  - **Detection & Tags/Tag Options**
    - Set **Emerald AI Unity Tag** to "AI"
    - Set **Detection Layers** to "CharacterControllers" and "AiVisiblity "
    - Set **Follower Tag** to "Untagged"
  - If you are using a health bar or UI damage counters, int **UI Settings**:
    - Set **Camera Tag** to "MainCamera"
	- Set ** UI Tag** to "Player"
	- Set **UI Layers** to "Default", "CharacterControllers" and "AiVisiblity"
- Added **NeoFpsEmeraldAI_AsyncLoadFixer** behaviour to the root object
- Added **SimpleSurface** behaviour to the root object with the relevant surface type

If you want your AI to have limb / location based damage then you will also need to do the following:
- Set the AI up with a ragdoll and **LocationBasedDamage** behaviour as per the [Emerald AI documentation](https://github.com/Black-Horizon-Studios/Emerald-AI/wiki/Using-Location-Based-Damage)
- Add each of the colliders to the list of location damage colliders in that **LocationBasedDamage** component
- Set the layer for each of the ragdoll colliders to **CharacterRagdoll**
- Add the **NeoFpsEmeraldAI_LocationBasedDamageHandler** behaviour to each limb collider with a multiplier of 0.1 (set the head to "Critical" = true)
Otherwise, you can simply do the following:
- Add a **NeoFpsEmeraldAI_DamageHandler** behaviour to the root object with multiplier 0.1

#### Player Character Setup
The only change required on the player character is to add an **NeoFpsEmeraldAI_TargetPositionModifier** behaviour to the root. This will help them aim at your head / upper body when you stand or crouch.

## Issues

#### Save Games Not Currently Implemented
The NeoFPS save games system has not yet been integrated with Emerald AI. Due to the complexity of the Emerald AI code base, and it not being written with save systems in mind (eg initialisation setting properties that would override the load state), it is not possible to implement save games to Emerald AI without considerable changes to its code. This is outside the scope of this integration.

#### Async Loads (Loading Screen)
NeoFPS uses a loading screen within its own dedicated scene, and then async loads game scenes behind this. Any objects that are instantiated within this period will be instantiated in the loading scene since this is the scene that is active. This also means that they will be destroyed once the loading screen is unloaded. In code, this can be fixed either by instantiating in Start instead, or by setting the objects' scene via:
```
SceneManager.MoveGameObjectToScene(newGameObject, instantiatingBehaviour.gameObject.scene);
```
Emerald AI creates and initialises its object pooling system and combat text system from within the Awake() method of the AI. This means that when combined with NeoFPS' async scene loading, they will be instantiated in the wrong scene and then destroyed. The provided **NeoFpsEmeraldAI_AsyncLoadFixer** behaviour is a workaround for this problem, which checks the object pool and combat text system after a 1 frame delay and re-instantiates them if they are null.

## Future Work
This integration should be updated along with any major updates to Emerald AI, and when new features are added to NeoFPS.

Keep an eye on this repo for updates and fixes.

Enjoy!
