# NeoFPS_EmeraldAI
This repository contains the scripts required to integrate NeoFPS and Emerald AI 2024, along with a simple demo of it in use.

**If you need an integration for Emerald AI 3 or 2, you can use the branches and tags dropdown to select the relevant branch of this repository.**

## Requirements
This repository was created using Unity 2020.3 since this is the version that Emerald AI was uploaded to the store with.

It requires the assets [NeoFPS](https://assetstore.unity.com/packages/templates/systems/neofps-150179?aid=1011l58Ft) and [Emerald AI 2024](https://assetstore.unity.com/packages/tools/behavior-ai/emerald-ai-2024-268519?aid=1011l58Ft).

Last updated with version 1.1.24 of NeoFPS and 1.0.6.1 of Emerald AI 2024. 

## Installation
This integration example is intended to be dropped in to a fresh project along with NeoFPS and Emerald AI.

1. Import NeoFPS and apply the required Unity settings using the NeoFPS Settings Wizard. You can find more information about this process [here](https://docs.neofps.com/manual/neofps-installation.html).

2. Import the Emerald AI asset.

3. Clone this repository to a folder inside the project Assets folder such as "NeoFPS_EmeraldAI"

4. Create new AI and player prefabs required or modify the existing ones using the integration scripts and the instructions below.

## Demo Scene and Assets
The demo scene is a basic prototype scene containing a number of the example AI provided with Emerald AI, which have been tweaked to work with NeoFPS.

There are also a number of demo prefabs and assets which have been divided into the following folders:
- **Abilities** contains duplicates of the Emerald AI spell abilities. These have been tweaked to use the correct layers and tags for NeoFPS
- **AI** contains modified duplicates of Emerald AI demo NPCs. All the AI are set to be enemies of the player and they have been given a random wander movement setup. They have also all been set up with location based damage and sound detection to demonstrate those features.
- **Player Character** contains a version of a NeoFPS player character which is set up with Emerald AI's player bridge and audio attractors.
- **Scene** contains a simple layout prefab for the scene geometry.
- **Utilities** contains a prefab you can place in your NeoFPS scene to fix an issue where Emerald AI creates its combat UI in the loading screen additive scene so it is destroyed when the loading screen is removed.
- **Weapons** contains modified versions of some NeoFPS demo weapons. The assault rifle, pistol and frag grenades have been modified to add audio attractors that alert the AI to their use. The pistol has also been modified to replace its damage effect with a stun ammo effect. The melee weapon has both damage and stun effects.

## Integration
The instructions on how to leverage this integration are split into 4 main sections: the NeoFPS player character, NeoFPS weapons, Emerald AI NPCs, and Emerald AI abilities. Each of these requires some tweaks or specific settings to get the most out of the integration.

#### Setting Up The NeoFPS Player Character
The main thing to do to configure your NeoFPS player character for use with Emerald AI is to add a **NeoFpsEmeraldAI_PlayerBridge** behaviour to the root of the character. This behaviour has no settings that need changing, and it also automatically adds 2 other behaviours: a **FactionExtension** and a **NeoFpsEmeraldAI_TargetPositionModifier**. In the **Faction Extension** behaviour, you should set the faction to `Player`.

If you want your AI to take advantage of sound detection and attraction, then you will need to take some extra steps:
- Add an **AttractModifier** to the player character root
  - This will add an audio source. Collapse and move this out of the way since it will not be used directly
  - For the "Emerald AI Layer" property, set this to `CharacterControllers`
  - Set an appropriate reaction such as suspicious (since this will be used for footsteps)
  - Set the "Player Faction" to `Player`
  - Set an appropriate radius and cooldown
  - Set the "Trigger Type" to `On Custom Call`
- Add a **NeoFpsEmeraldAI_FootstepAttractor** to the root also
  - Point its "Motion Controller" property and the motion controller component on the root of the character
  
#### Setting up NeoFPS Weapons
The NeoFPS weapons will work out of the box if the other elements here are set up as instructed. However there are 2 Emerald AI features that require some additions or changes. These are stunning AI and sound detection.

Sound detection requires the following changes:
- For simple firearms that do not use different attachments to change the muzzle effect (eg adding a silencer) you can add a **NeoFpsEmeraldAI_FirearmAttractor** behaviour to the root of the gun. This does not have any custom settings.
  - This will add an **AttractModifier** alongside, which you should set up as follows:
    - For the "Emerald AI Layer" property, set this to `CharacterControllers`
    - Set an appropriate reaction such as suspicious (since this will be used for footsteps)
    - Set the "Player Faction" to `Player`
    - Set an appropriate radius and cooldown
    - Set the "Trigger Type" to `On Custom Call`
- For firearms that use attachments to change muzzle effects, you should add the **NeoFpsEmeraldAI_FirearmAttractor** to the attachment instead.
  - This will add the **AttractModifier** to the attachment. Set it up as above.
  - This will also add an audio source to the attachment if it was not there already. The firearm attractor does not make use of the audio source, so you can essentially ignore it.
- In order for explosives such as frag grenades to work with sound detection, you will need to replace the **PooledExplosion** behaviour on the explosion itself (eg not the thrown weapon or projectile that triggers it) with a **NeoFpsEmeraldAI_PooledExplosion** behaviour.
  - This will add an **AttractModifier** alongside, which you should set up as the previous example.
  
Applying a stun effect to AI via Emerald requires the following changes:
- For firearms you can use a **NeoFpsEmeraldAI_StunAmmoEffect** as your firearm's ammo effect. This also uses particle system based hit effects instead of the usual surface system, so that you can add a visual effect like electrical charges instead of fleshy impacts.
- For melee, you will have to replace the **MeleeWeapon** behaviour on your weapon prefab with a **NeoFpsEmeraldAI_StunMeleeWeapon** behaviour.
  - The easiest way to do this is to switch the Unity inspector to debug mode using the dropdown options at the top right, and then find the **MeleeWeapon** behaviour and swap its "Script" property to point at `NeoFpsEmeraldAI_StunMeleeWeapon.cs`. This means that any references to the behaviour in other inspectors will not be lost.
- For explosions, you should replace the **PooledExplosion** behaviour on the explosion itself (eg not the thrown weapon or projectile that triggers it) with a **NeoFpsEmeraldAI_StunExplosion**.
  - This is inherited from the **NeoFpsEmeraldAI_PooledExplosion** above, and so also adds the sound detection features if desired.
  
#### Setting Up Emerald AI NPCs
The following settings and tweaks are required to get an Emerald AI working with NeoFPS:
- In the **Detection** module
  - In "Detection Settings"
    - "Obstruction Ignore Layers" should be set to `Default, Environment Rough, Moving Platforms, Dynamic Props, Doors`
  - In "Tag & Layer Settings"
    - "Player Unity Tag" should be set to `Player`
    - "Detection Layers" should be set to `CharacterControllers, AIVisibility`
- In the **Health module**, some Emerald AI examples use a very low health value such as 5. This will mean the enemy is often one-shot by the player, so it is generally better to raise it to 100 or more and adjust your attacks appropriately.
- In your **Location Based Damage** module (if you are using it) you should set the layer to `Character Physics` and tag to `AI`
  - For each of the colliders in the collider list, you should add a **NeoFpsEmeraldAI_LocationBasedDamageHandler** behaviour, along with a **SimpleSurface**. For colliders such as the head, you can set the **NeoFpsEmeraldAI_LocationBasedDamageHandler** to pass any damage along as critical
  - If you are not using location based damage, you should instead add a collider to your AI on a layer that NeoFPS weapons can hit such as `CharacterPhysics`, and add a **NeoFpsEmeraldAI_DamageHandler** and **SimpleSurface** to that instead.
- Make sure that any weapon models are on a layer that the player can see such as `Default`

#### Setting Up Emerald AI Abilities
The AI abilities such as the different spells cast by the example NPC in the demos need to use the correct layers, or else they will not deal damage to the NeoFPS player character.
- For abilities with a **Collider Module**:
  - Set the "Collidable Layers" to `Default, CharacterControllers, CharacterPhysics, AiVisiblity`
  - Set the "Projectile Layer" to `Default`
- For abilities with a **Bullet Projectile Module**:
  - Set the "Ignore Layers" to `TransparentFX, IgnoreRaycast, Water, UI, PostProcessingVolumes, EnvironmentDetail, CharacterFirstPerson, CharacterNonColliding, WieldablesFirstPerson, TriggerZones, InteractiveObjects, SmallDynamicObjects, Effects, AiVisiblity`
- For abilities with a **Ground Projectile Module**:
  - Set "Alignment Layers" to `EnvironmentRough`

## Issues

#### AI Do Not Null Check Their Target
When the NeoFPS player character dies and respawns, this destroys their old character object. Any Emerald AI that were targeting the player character will then break and throw errors to the console. This has been reported as a bug to the Emerald AI developers and should hopefully be fixed soon.

#### Save Games Not Currently Implemented
The NeoFPS save games system has not yet been integrated with Emerald AI. Due to the complexity of the Emerald AI code base, and it not being written with save systems in mind (eg initialisation setting properties that would override the load state), it is not possible to implement save games to Emerald AI without considerable changes to its code. This is outside the scope of this integration.

#### Async Loads (Loading Screen)
NeoFPS uses a loading screen within its own dedicated "additive" scene, and then async loads game scenes behind this. Any objects that are instantiated from code within this period will be created in the loading scene since this is the scene that is currently active. This also means that they will be destroyed once the loading screen is unloaded. In code, this can be fixed either by instantiating in Start instead, or by setting the objects' scene via:
```
SceneManager.MoveGameObjectToScene(newGameObject, instantiatingBehaviour.gameObject.scene);
```
Emerald AI creates and initialises its object pooling system and combat text system from within the Awake() method of the AI. This means that when combined with NeoFPS' async scene loading, they will be instantiated in the wrong scene and then destroyed. The provided **NeoFpsEmeraldAI_AsyncLoadFixer** behaviour is a workaround for this problem, which checks the object pool and combat text system after a 1 frame delay and re-instantiates them if they are null. This can be placed on an object in your game scene and it will perform the check silently in the background. There is also a prefab provided that you can use instead.