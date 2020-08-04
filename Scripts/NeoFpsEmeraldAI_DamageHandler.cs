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
    public class NeoFpsEmeraldAI_DamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField, Tooltip("The value to multiply any incoming damage by. Use to reduce damage to areas like feet, or raise it for areas like the head.")]
        private float m_Multiplier = 0.1f;

        [SerializeField, Tooltip("Does the damage count as critical. Used to change the feedback for the damage taker and dealer.")]
        private bool m_Critical = false;

        private EmeraldAISystem m_EmeraldAISystem;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (m_Multiplier < 0f)
                m_Multiplier = 0f;
        }
#endif

        void Awake()
        {
            m_EmeraldAISystem = GetComponentInParent<EmeraldAISystem>();
        }

        #region IDamageHandler implementation

        public DamageFilter inDamageFilter
        {
            get { return DamageFilter.AllDamageAllTeams; ; }
            set { }
        }

        public DamageResult AddDamage(float damage)
        {
            if (m_Multiplier > 0f)
            {
                int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);

                if (CombatTextSystem.Instance != null)
                    CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

                m_EmeraldAISystem.Damage(scaledDamage);

                return m_Critical ? DamageResult.Critical : DamageResult.Standard;
            }
            else
                return DamageResult.Ignored;
        }

        public DamageResult AddDamage(float damage, IDamageSource source)
        {
            if (source == null || source.controller == null)
                return AddDamage(damage);

            // Apply damage
            if (m_Multiplier > 0f)
            {
                int scaledDamage = Mathf.CeilToInt(damage * m_Multiplier);

                if (CombatTextSystem.Instance != null)
                    CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

                m_EmeraldAISystem.Damage(
                    scaledDamage,
                    source.controller.isPlayer ? EmeraldAISystem.TargetType.Player : EmeraldAISystem.TargetType.AI,
                    source.controller.currentCharacter.transform
                    );

                return m_Critical ? DamageResult.Critical : DamageResult.Standard;
            }
            else
                return DamageResult.Ignored;
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