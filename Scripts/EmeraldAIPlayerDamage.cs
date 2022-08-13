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

using System.Collections.Generic;
using UnityEngine;
using NeoFPS;

namespace EmeraldAI
{
    public class EmeraldAIPlayerDamage : MonoBehaviour, IDamageSource
    {
        public List<string> ActiveEffects = new List<string>();

        const float k_KickDistance = 0.02f;
        const float k_KickRotation = 5f;
        const float k_KickDuration = 0.5f;

        private EmeraldAISystem m_AI = null;
        private IDamageHandler m_DamageHandler = null;
        private IHealthManager m_HealthManager = null;
        private AdditiveKicker m_HeadKicker = null;
        private bool m_Initialised = false;

        public bool IsDead
        {
            get
            {
                if (m_HealthManager != null)
                    return !m_HealthManager.isAlive;
                else
                    return true;
            }
        }

        public DamageFilter outDamageFilter
        {
            get { return DamageFilter.AllDamageAllTeams; }
            set { }
        }

        public IController controller
        {
            get { return null; }
        }

        public Transform damageSourceTransform
        {
            get { return m_AI.transform; }
        }

        public string description
        {
            get { return m_AI.AIName; }
        }

        void Awake()
        {
            if (!m_Initialised)
                Initialise();
        }

        void Initialise()
        {
            // Get health and damage handlers
			m_DamageHandler = GetComponent<IDamageHandler>();
            m_HealthManager = GetComponentInParent<IHealthManager>();

            // Get character head kicker
            var character = GetComponent<ICharacter>();
            if (character != null)
                m_HeadKicker = character.headTransformHandler.GetComponent<AdditiveKicker>();

            m_Initialised = true;
        }

        public void SendPlayerDamage(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent, bool CriticalHit = false)
        {
            if (!m_Initialised)
                Initialise();
			
            // Store the AI
            m_AI = EmeraldComponent;

            // Apply damage
			if (m_DamageHandler != null)
				m_DamageHandler.AddDamage(DamageAmount, this);
			else
			{
				if (m_HealthManager != null)
					m_HealthManager.AddDamage(DamageAmount, CriticalHit, this);
			}

            // Kick the camera position & rotation
            if (m_HeadKicker != null)
            {
                // Get direction of attack
                var direction = transform.position - EmeraldComponent.transform.position;
                direction.y = 0;
                direction.Normalize();

                m_HeadKicker.KickPosition(direction * k_KickDistance, k_KickDuration);
                m_HeadKicker.KickRotation(Quaternion.AngleAxis(k_KickRotation, Vector3.Cross(direction, Vector3.up)), k_KickDuration);
            }
        }
    }
}
