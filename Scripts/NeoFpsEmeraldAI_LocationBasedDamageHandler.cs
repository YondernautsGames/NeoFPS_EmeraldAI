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
        [SerializeField, Min(0f), Tooltip("The value to multiply any incoming damage by. Use to reduce damage to areas like feet, or raise it for areas like the head.")]
        private float m_Multiplier = 0.1f;

        [SerializeField, Tooltip("Does the damage count as critical. Used to change the feedback for the damage taker and dealer.")]
        private bool m_Critical = false;

        private EmeraldAISystem m_EmeraldAISystem;
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
            m_EmeraldAISystem = GetComponentInParent<EmeraldAISystem>();

            // LocationBasedDamageArea added by emerald's LocationBasedDamage in Awake() 
            m_LocationBasedDamage = GetComponent<LocationBasedDamageArea>();
        }

        protected virtual void OnPlayerKilledAI()
        {
            //Debug.Log("AI was killed by player");
        }

        public void GetDamageValues(float inDamage, out int scaledDamage, out int reportedDamage)
        {
            scaledDamage = Mathf.CeilToInt(inDamage * m_Multiplier);

            if (m_LocationBasedDamage != null)
                reportedDamage = Mathf.RoundToInt(scaledDamage * m_LocationBasedDamage.DamageMultiplier);
            else
                reportedDamage = scaledDamage;
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
            if (m_Multiplier > 0f)
            {
                // Get the modified damage values
                GetDamageValues(damage, out int scaledDamage, out int reportedDamage);

                // Show the damage text
                if (CombatTextSystem.Instance != null)
                    CombatTextSystem.Instance.CreateCombatTextAI(reportedDamage, transform.position, m_Critical, false);

                // Apply the damage
                if (m_LocationBasedDamage)
                    m_LocationBasedDamage.DamageArea(scaledDamage);
                else
                    m_EmeraldAISystem.Damage(scaledDamage);

                return m_Critical ? DamageResult.Critical : DamageResult.Standard;
            }
            else
                return DamageResult.Ignored;
        }

        public DamageResult AddDamage(float damage, IDamageSource source)
        {
            bool isDead = m_LocationBasedDamage.EmeraldComponent.IsDead;

            if (source == null || source.controller == null)
                return AddDamage(damage);

            // Apply damage
			if (inDamageFilter.CollidesWith(source.outDamageFilter, false))
            {
                // Get the modified damage values
                GetDamageValues(damage, out int scaledDamage, out int reportedDamage);

                // Show the damage text
                if (CombatTextSystem.Instance != null)
					CombatTextSystem.Instance.CreateCombatTextAI(reportedDamage, transform.position, m_Critical, false);

                // Was the damage source a character controller
                if (source.controller != null)
                {
                    // Apply the damage
                    ApplyDamage(scaledDamage, source.controller.isPlayer, source.controller.currentCharacter.transform);

                    // Report damage dealt
                    if (damage > 0f && source != null && source.controller != null)
                        source.controller.currentCharacter.ReportTargetHit(m_Critical);

                    // Report killed
                    if (!isDead && m_EmeraldAISystem.IsDead && source.controller.isPlayer)
                        OnPlayerKilledAI();
                }
                else
                {
                    // Apply the damage
                    ApplyDamage(scaledDamage, source.controller.isPlayer, source.controller.currentCharacter.transform);
                }

				return m_Critical ? DamageResult.Critical : DamageResult.Standard;
			}
			else
                return DamageResult.Ignored;
        }

        void ApplyDamage(int scaledDamage, bool isPlayer, Transform sourceTransform)
        {
            var targetType = isPlayer ? EmeraldAISystem.TargetType.Player : EmeraldAISystem.TargetType.NonAITarget;
            if (m_LocationBasedDamage)
                m_LocationBasedDamage.DamageArea(scaledDamage, targetType, sourceTransform);
            else
                m_EmeraldAISystem.Damage(scaledDamage, targetType, sourceTransform);
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