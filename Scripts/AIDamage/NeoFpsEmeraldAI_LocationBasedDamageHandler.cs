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
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_LocationBasedDamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField, Min(0.1f), Tooltip("A multiplier applied to the damage to calculate impact force.")]
        private float m_ForceMultiplier = 10f;

        [SerializeField, Tooltip("Does the damage count as critical. Used to change the feedback for the damage taker and dealer.")]
        private bool m_Critical = false;

        private EmeraldSystem m_EmeraldAISystem;
        private LocationBasedDamageArea m_LocationBasedDamage;

        protected bool isCritical
        {
            get { return m_Critical; }
        }

        public IHealthManager healthManager
        {
            get { return null; }
        }

        protected void Start()
        {
            m_EmeraldAISystem = GetComponentInParent<EmeraldSystem>();

            // LocationBasedDamageArea added by emerald's LocationBasedDamage in Awake() 
            m_LocationBasedDamage = GetComponent<LocationBasedDamageArea>();
        }

        protected virtual void OnPlayerKilledAI()
        {
            //Debug.Log("AI was killed by player");
        }

        public void GetDamageValues(float inDamage, out int intDamage, out int reportedDamage, out int force)
        {
            intDamage = Mathf.CeilToInt(inDamage);

            if (m_LocationBasedDamage != null)
                reportedDamage = Mathf.RoundToInt(intDamage * m_LocationBasedDamage.DamageMultiplier);
            else
                reportedDamage = intDamage;

            force = (int)(intDamage * m_ForceMultiplier);
        }

        #region IDamageHandler implementation

		private DamageFilter m_InDamageFilter = DamageFilter.AllDamageAllTeams;
        public DamageFilter inDamageFilter
        {
            get { return m_InDamageFilter; }
            set { m_InDamageFilter = value; }
        }

        public DamageResult AddDamage(float damage)
        {
            // Get the modified damage values
            GetDamageValues(damage, out int scaledDamage, out int reportedDamage, out int force);

            // Show the damage text
            if (CombatTextSystem.Instance != null)
                CombatTextSystem.Instance.CreateCombatTextAI(reportedDamage, transform.position, m_Critical, false);

            // Apply the damage
            ApplyDamage(scaledDamage, force, null);

            return m_Critical ? DamageResult.Critical : DamageResult.Standard;
        }

        public DamageResult AddDamage(float damage, IDamageSource source)
        {
            bool alreadyDead = m_EmeraldAISystem.HealthComponent.Health <= 0;

            if (source == null || source.controller == null)
                return AddDamage(damage);

            // Apply damage
			if (inDamageFilter.CollidesWith(source.outDamageFilter, false))
            {
                // Get the modified damage values
                GetDamageValues(damage, out int intDamage, out int reportedDamage, out int force);

                // Show the damage text
                if (CombatTextSystem.Instance != null)
					CombatTextSystem.Instance.CreateCombatTextAI(reportedDamage, transform.position, m_Critical, false);

                // Was the damage source a character controller
                if (source.controller != null)
                {
                    // Apply the damage
                    ApplyDamage(intDamage, force, source.controller.currentCharacter.transform);

                    // Report killed
                    if (!alreadyDead && m_EmeraldAISystem.HealthComponent.Health <= 0 && source.controller.isPlayer)
                        OnPlayerKilledAI();
                }
                else
                {
                    // Apply the damage
                    ApplyDamage(intDamage, force, source.controller.currentCharacter.transform);
                }

                var result = m_Critical ? DamageResult.Critical : DamageResult.Standard;

                // Report damage dealt
                if (damage > 0f)
                    DamageEvents.ReportDamageHandlerHit(this, source, transform.position, result, damage);

                return result;
            }
			else
                return DamageResult.Ignored;
        }

        void ApplyDamage(int intDamage, int force, Transform sourceTransform)
        {
            if (m_LocationBasedDamage)
                m_LocationBasedDamage.DamageArea(intDamage, sourceTransform, force, m_Critical);
            else
                m_EmeraldAISystem.HealthComponent.Damage(intDamage, sourceTransform, force, m_Critical);
        }

        public DamageResult AddDamage(float damage, RaycastHit hit)
        {
            return AddDamage(damage);
        }

        public DamageResult AddDamage(float damage, RaycastHit hit, IDamageSource source)
        {
            return AddDamage(damage, source);
        }

        #endregion
    }
}