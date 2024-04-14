/*
 * Copyright 2020 Yondernauts Games Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
using EmeraldAI;
using EmeraldAI.Utility;
using UnityEngine;

// This class is a legacy workaround to enable location based damage (eg headshots) to Emerald AI characters.

// NOTE: THIS CLASS IS NO LONGER REQUIRED WITH THE LATEST VERSIONS OF EMERALD AI 3, SINCE IT ADDS AN OFFICIAL WAY TO HANDLE LIMB COLLIDERS AND RAGDOLL

// Using this class requires modifying the Emerald AI source code, so it is sensitive to changes in the API.
// To use it you will need to perform the following steps:
// 1 - In the EmeraldAIInitializer.cs script, make the EmeraldComponent protected
// 2 - Make the EnableRagdoll() and DisableRagdoll() methods virtual
// 3 - Delete the /* and */ comment start and end below to uncomment the methods in this class
// 4 - Replace the Emerald AI Initializer component on your AI with this class
// 5 - Add any ragdoll colliders to the m_RagdollColliders array in the inspector
// 6 - Move the root object of the Emerald AI character (only the root object) to the CharacterControllers layer
// 7 - Remove the NeoFpsEmeraldAI_DamageHandler component from the root object of the character
// 8 - Add damage handlers to the character's skeleton (eg, limbs, head and torso)
//   a - Create a new child object of the relevant bone and set it to the CharacterPhysics layer
//   b - Add a collider that is roughly the shape required
//   c - Add a NeoFpsEmeraldAI_DamageHandler component and set it up with the desired values (note that the Emerald demo characters have 10 health, so a multiplier of less than 1 is used in most cases)

namespace NeoFPS.EmeraldAI
{

    public class NeoFpsEmeraldAI_InitializerOverride : EmeraldAIInitializer
    {
        [SerializeField]
        private Collider[] m_RagdollColliders = { };

        /*

        public override void DisableRagdoll()
        {
            foreach (Rigidbody R in transform.GetComponentsInChildren<Rigidbody>())
            {
                R.isKinematic = true;
            }

            foreach (Collider C in m_RagdollColliders)
            {
				if (C != null)
					C.enabled = false;
            }

            GetComponent<BoxCollider>().enabled = true;
        }

        public override void EnableRagdoll()
        {
            EmeraldComponent.AIBoxCollider.enabled = false;
            EmeraldComponent.AIAnimator.enabled = false;

            foreach (Collider C in m_RagdollColliders)
            {
                if (C != null && C.transform != this.transform)
                {
                    C.tag = EmeraldComponent.RagdollTag;
                    C.enabled = true;
                }
            }

            foreach (Rigidbody R in transform.GetComponentsInChildren<Rigidbody>())
            {
                R.isKinematic = false;
            }

            if (EmeraldComponent.UseDroppableWeapon == EmeraldAISystem.YesOrNo.Yes)
            {
                EmeraldComponent.EmeraldEventsManagerComponent.CreateDroppableWeapon();
            }
        }
	
        */
    }
}
