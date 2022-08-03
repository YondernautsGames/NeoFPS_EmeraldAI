# NeoFPS_EmeraldAI
NeoFPS and Emerald AI 3 integration assets

## Requirements
This repository was created using Unity 2018.4

It requires the assets [NeoFPS](https://assetstore.unity.com/packages/templates/systems/neofps-150179?aid=1011l58Ft) and [Emerald AI](https://assetstore.unity.com/packages/tools/ai/emerald-ai-3-0-203904?aid=1011l58Ft).

Last updated with version 1.1.08 of NeoFPS and 3.1 of Emerald AI. If you need an integration for Emerald AI 2, you can use the branches and tags dropdown to select the Emerald AI 2 branch.

## Installation
This integration example is intended to be dropped in to a fresh project along with NeoFPS and Emerald AI.

1. Import NeoFPS and apply the required Unity settings using the NeoFPS Settings Wizard. You can find more information about this process [here](https://docs.neofps.com/manual/neofps-installation.html).

2. Import the Emerald AI asset.

3. Clone this repository to a folder inside the project Assets folder such as "NeoFPS_EmeraldAI"

4. Remove the **EmeraldAIPlayerDamage.cs** script located in **Emerald AI/Scripts/Components** to fix the duplicate script error (you want to use the NeoFPS script)
	
## Integration
This integration only requires that you modify the AI characters, and replace the player damage script as mentioned above to work. The NeoFPS characters and weapons do not need any modifications to work once the AI have been correctly set up.

The following are the important assets in this repo that enable NeoFPS and Emerald AI to work side by side.

#### MonoBehaviours
There is a new custom **EmeraldAIPlayerDamage.cs** in **NeoFPS_EmeraldAI/Scripts** which replaces the one in **Emerald AI/Scripts/Player**. This is a simple behaviour that Emerald AI characters add to any targets they attack in order to communicate AI to player damage. This script applies player damage as well as providing information about the damage source to the NeoFPS character, and triggering hit effects such as camera knock. You can add this to the NeoFPS player character if you want, but it will be added automatically when you are first attacked if not.

The script **NeoFpsEmeraldAI_DamageHandler.cs** located in **NeoFPS_EmeraldAI/Scripts** is a NeoFPS damage handler which passes damage to the Emerald AI character as well as letting the character know the damage source. This must be added to all Emerald AI characters so the player can damage them.

The script **NeoFpsEmeraldAI_AsyncLoadFixer.cs** is required for Emerald AI to work with async scene loading. This should be present in every scene and the easiest way is by adding it to any Emerald AI characters.

#### Demo Scene
The demo scene is a modified version of Emerald AI's **Playable Demo** scene. The character prefabs have been replaced with new ones as described below, and the Emerald AI demo player character has been replaced by a NeoFPS spawner which spawns the standard NeoFPS demo character.

#### Prefabs
The three character prefabs from the original Emerald AI **Playable Demo** have been replaced with duplicates set up to work with NeoFPS. For reference (this does not need repeating for the demo), this involved the following changes:
- Switched tag on root object to "AI" and layer to "Default"
- On **EmeraldAISystem** component:
  - **Detection & Tags/Tag Options**
    - Set **Emerald AI Unity Tag** to "AI"
    - Set **Detection Layers** to "CharacterControllers" and "AiVisiblity "
    - Set **Follower Tag** to "Untagged"
  - **Sounds/Combat**
    - Removed all injured sounds and set injured sounds volume to 0 (too loud in first person, especially when spawned together due to shotgun hits)
- Added **NeoFpsEmeraldAI_DamageHandler** behaviour to the root object with multiplier 0.1
- Added **NeoFpsEmeraldAI_AsyncLoadFixer** behaviour to the root object
- Added **SimpleSurface** behaviour to the root object with the relevant surface type

If you want your AI to have limb / location based damage then you will also need to do the following:
- Add a ragdoll to the AI character as per the Emerald AI docs
- Add the NeoFpsEmeraldAI_DamageHandler component to each limb collider
- Add a **LocationBasedDamage** component to the root of the character
- Add each of the colliders to the list of location damage colliders in that **LocationBasedDamage** component
- Set the layer for each of the ragdoll colliders to **CharacterRagdoll**

## Issues

#### Save Games Not Currently Implemented
The NeoFPS save games system has not yet been integrated with Emerald AI. We're looking at how to achieve this, but it will require work from both sides.

#### Async Loads (Loading Screen)
NeoFPS uses a loading screen within its own dedicated scene, and then async loads game scenes behind this. Any objects that are instantiated within this period will be instantiated in the loading scene since this is the scene that is active. This also means that they will be destroyed once the loading screen is unloaded. In code, this can be fixed either by instantiating in Start instead, or by setting the objects' scene via:
```
SceneManager.MoveGameObjectToScene(newGameObject, instantiatingBehaviour.gameObject.scene);
```
Emerald AI creates and initialises its object pooling system and combat text system from within the Awake() method of the AI. This means that when combined with NeoFPS' async scene loading, they will be instantiated in the wrong scene and then destroyed. The provided NeoFpsEmeraldAI_AsyncLoadFixer behaviour is a workaround for this problem, which checks the object pool and combat text system after a 1 frame delay and re-instantiates them if they are null.

## Future Work
This integration will be updated along with any major updates to Emerald AI, and when new features are added to NeoFPS. For example, the damage handlers will be altered to enable blocking when the additional NeoFPS melee features are added.

Keep an eye on this repo for updates and fixes.

Enjoy!
