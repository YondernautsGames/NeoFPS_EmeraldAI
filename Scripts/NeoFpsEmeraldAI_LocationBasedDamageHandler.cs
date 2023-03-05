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
        [SerializeField, Tooltip("The value to multiply any incoming damage by. Use to reduce damage to areas like feet, or raise it for areas like the head.")]
        private float m_Multiplier = 0.1f;

        [SerializeField, Tooltip("Does the damage count as critical. Used to change the feedback for the damage taker and dealer.")]
        private bool m_Critical = false;

        private LocationBasedDamageArea m_LocationBasedDamage;

        protected bool isCritical
        {
            get { return m_Critical; }
        }

        public IHealthManager healthManager
        {
            get { return null; }
        }

        void Start()
        {
            m_LocationBasedDamage = GetComponent<LocationBasedDamageArea>();
        }

        protected virtual void OnPlayerKilledAI()
        {
            //Debug.Log("AI was killed by player");
        }

        #region IDamageHandler implementation

        public DamageFilter inDamageFilter
        {
            get { return DamageFilter.AllDamageAllTeams; ; }
            set { }
        }

        public DamageResult AddDamage(float damage)
        {
            int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);

            if (CombatTextSystem.Instance != null)
                CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

            m_LocationBasedDamage.DamageArea(scaledDamage);
               
            return m_Critical ? DamageResult.Critical : DamageResult.Standard;
        }

        public DamageResult AddDamage(float damage, IDamageSource source)
        {
            bool isDead = m_LocationBasedDamage.EmeraldComponent.IsDead;

            if (source == null || source.controller == null)
                return AddDamage(damage);

            // Apply damage
            int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);

            if (CombatTextSystem.Instance != null)
                CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

            m_LocationBasedDamage.DamageArea(
                scaledDamage,
                source.controller.isPlayer ? EmeraldAISystem.TargetType.Player : EmeraldAISystem.TargetType.AI,
                source.controller.currentCharacter.transform
                );
					
            // Report damage dealt
            if (damage > 0f && source != null && source.controller != null)
                source.controller.currentCharacter.ReportTargetHit(m_Critical);

            // Report killed
            if (!isDead && m_LocationBasedDamage.EmeraldComponent.IsDead && source.controller.isPlayer)
                OnPlayerKilledAI();

            return m_Critical ? DamageResult.Critical : DamageResult.Standard;
        }

        public DamageResult AddDamage(float damage, RaycastHit hit)
        {
            // UHG
            if (m_LocationBasedDamage)
                return AddLocationBasedDamage(damage, hit);
            else
                return AddDamage(damage);

        }

        public DamageResult AddDamage(float damage, RaycastHit hit, IDamageSource source)
        {
            // UHG
            if (m_LocationBasedDamage)
                return AddLocationBasedDamage(damage, hit, source);
            else
                return AddDamage(damage, source);
        }

        // UHG handle LocationBasedDamage
        private DamageResult AddLocationBasedDamage(float damage, RaycastHit hit, IDamageSource source)
        {
            bool isDead = m_LocationBasedDamage.EmeraldComponent.IsDead;

            if (source == null || source.controller == null)
                return AddDamage(damage);

            // Apply damage
            int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);
            float multiplier = m_LocationBasedDamage.DamageMultiplier;

            if (CombatTextSystem.Instance != null)
                CombatTextSystem.Instance.CreateCombatTextAI( (int)(scaledDamage * multiplier), transform.position, m_Critical, false);

            m_LocationBasedDamage.DamageArea(
                scaledDamage,
                source.controller.isPlayer ? EmeraldAISystem.TargetType.Player : EmeraldAISystem.TargetType.AI,
                source.controller.currentCharacter.transform
            );               

            // Report damage dealt
            if (damage > 0f && source != null && source.controller != null)
                source.controller.currentCharacter.ReportTargetHit(m_Critical);

            // Report killed
            if (!isDead && m_LocationBasedDamage.EmeraldComponent.IsDead && source.controller.isPlayer)
                OnPlayerKilledAI();

            return m_Critical ? DamageResult.Critical : DamageResult.Standard;
        }

        // UHG handle LocationBasedDamage
        private DamageResult AddLocationBasedDamage(float damage, RaycastHit hit)
        {
            int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);
            float multiplier = m_LocationBasedDamage.DamageMultiplier;

            if (CombatTextSystem.Instance != null)
                CombatTextSystem.Instance.CreateCombatTextAI((int)(scaledDamage * multiplier), transform.position, m_Critical, false);

            m_LocationBasedDamage.DamageArea(scaledDamage);

            return m_Critical ? DamageResult.Critical : DamageResult.Standard;
        }

        #endregion
    }
}