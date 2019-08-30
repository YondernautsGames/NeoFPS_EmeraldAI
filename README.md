# NeoFPS_EmeraldAI
NeoFPS and Emerald AI 2 integration assets

## Installation
This integration example is intended to be dropped in to a fresh project along with NeoFPS and Emerald AI.

1. Import NeoFPS and apply the required Unity settings using the NeoFPS Settings Wizard. You can find more information about this process [here](https://docs.neofps.com/manual/neofps-installation.html).

2. Import the Emerald AI asset.

3. Clone this repository to a folder inside the project Assets folder such as "NeoFPS_EmeraldAI"

4. Remove the **EmeraldAIPlayerDamage.cs** script located in **Emerald AI/Scripts/Player**

5. Apply the following settings changes *(these will be included in NeoFPS update 1.0.03)*
  - In **Preferences/Layers and Tags** add a new tag called "AI"
  - In **Preferences/Physics** update the layer collision matrix so the the layer **IgnoreRaycasts** collides with the following layers:
    - **AiVisiblity**
	- **CharacterControllers**
	- **IgnoreRaycasts**
	- **Default**
	
## Integration
The following are the important assets in this repo that enable NeoFPS and Emerald AI to work side by side.

#### Scripts
There is a new custom **EmeraldAIPlayerDamage.cs** in **NeoFPS_EmeraldAI/Scripts** which replaces the one in **Emerald AI/Scripts/Player**. This is a simple behaviour that Emerald AI characters add to any targets they attack in order to communicate AI to player damage. This script applies player damage as well as providing information about the damage source to the NeoFPS character, and triggering hit effects such as camera knock.

The script **NeoFpsEmeraldAI_DamageHandler.cs** located in **NeoFPS_EmeraldAI/Scripts** is a NeoFPS damage handler which passes damage to the Emerald AI character as well as letting the character know the damage source.

#### Demo Scene
The demo scene is a modified version of Emerald AI's **Playable Demo** scene. The character prefabs have been replaced with new ones as described below, and the player character has been replaced by a NeoFPS test setup spawner which spawns the standard NeoFPS demo character.

#### Prefabs
The three character prefabs from the original Emerald AI **Playable Demo** have been replaced with duplicates set up to work with NeoFPS. For reference (this does not need repeating for the demo), this involved the following changes:
- Switched tag on root object to "AI" and layer to "Default"
- On **EmeraldAISystem** component:
  - **Detection & Tags/Tag Options**
    - Set **Emerald AI Unity Tag** to "AI"
    - Set **Detection Layers** to "CharacterControllers" and "AiVisiblity "
    - Set **Follower Tag** to "AI"
  - **Sounds/Combat**
    - Removed all injured sounds and set injured sounds volume to 0 (too loud in first person, especially when spawned together due to shotgun hits)
- Added **NeoFpsEmeraldAI_DamageHandler** behaviour to the root object with multiplier 0.1
- Added **SimpleSurface** behaviour to the root object with the relevant surface type

## Issues

#### Script Errors
There are a few places in Emerald AI where the NeoFPS respawn system can destroy objects that are being tracked.

The AI health bars record the main camera component for orientation and scaling purposes. If the camera is changed for any reason, the script will still orient towards the old camera, and if it is destroyed then the script will error each frame. This can be fixed by modifying the file **EmeraldAIHealthBar.cs** located at **Emerald AI/Scripts/UI** by adding the following at the top of the `Update()` method.
```csharp
void Update()
{
	if (m_Camera == null)
		m_Camera = Camera.main;
	
	// Original contents here
}
```

The AI projectiles store a target transform when fired. If the player character respawns while a projectile is in flight and targeting it, the script will error each frame. This is a more complicated fix due to the number of times the target transform is referenced in the code and so I would advise waiting for a fix from Emerald AI.

#### Location Based Damage
Emerald AI characters are hard coded to disable all colliders in their character heirarchy on initialisation in order to prevent attached ragdolls causing issues. Unfortunately this means that, without modifications, the Emerald AI characters cannot have location specific damage handlers such as head or eye crits and less vulnerable armoured sections. The code that performs this can be found in the file **EmeraldAIInitializer.cs** in the method `DisableRagdoll()`.

I have requested that this method and the equivalent `EnableRagdoll()` be made virtual so that a custom derived initialiser can be used instead that separates the ragdoll from the damage colliders.

## Future Work
This integration will be updated along with any major updates to Emerald AI, and when new features are added to NeoFPS. For example, the damage handlers will be altered to enable blocking when the additional NeoFPS melee features are added.

Keep an eye on this repo for updates and fixes.

Enjoy!