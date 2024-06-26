﻿/*
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

using System;
using EmeraldAI;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_DamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField, Min(0.01f), Tooltip("The value to multiply any incoming damage by. Use to reduce damage to areas like feet, or raise it for areas like the head.")]
        private float m_DamageMultiplier = 0.1f;

        [SerializeField, Min(0.1f), Tooltip("A multiplier applied to the damage to calculate impact force.")]
        private float m_ForceMultiplier = 10f;

        [SerializeField, Tooltip("Does the damage count as critical. Used to change the feedback for the damage taker and dealer.")]
        private bool m_Critical = false;

        private EmeraldSystem m_EmeraldAISystem;
		
		public IHealthManager healthManager
		{
			get { return null; }
		}

#if UNITY_EDITOR
        void OnValidate()
        {
            if (m_DamageMultiplier < 0f)
                m_DamageMultiplier = 0f;
        }
#endif

        protected bool isCritical
        {
            get { return m_Critical; }
        }

        void Awake()
        {
            m_EmeraldAISystem = GetComponentInParent<EmeraldSystem>();
        }

        protected virtual void OnPlayerKilledAI()
        {
            //Debug.Log("AI was killed by player");
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
            if (m_DamageMultiplier > 0f)
            {
                int scaledDamage = Mathf.CeilToInt(damage * m_DamageMultiplier);

                if (CombatTextSystem.Instance != null)
                    CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

				try
                {
                    m_EmeraldAISystem.HealthComponent.Damage(
                        scaledDamage,
                        null,
                        (int)(damage * m_ForceMultiplier),
                        m_Critical
                        );
				}
				catch (Exception e)
				{
					Debug.LogError("Emerald AI threw an exception: " + e.Message);
				}

                return m_Critical ? DamageResult.Critical : DamageResult.Standard;
            }
            else
                return DamageResult.Ignored;
        }

        public DamageResult AddDamage(float damage, IDamageSource source)
        {
            bool alreadyDead = m_EmeraldAISystem.HealthComponent.Health <= 0;

            if (source == null)
                return AddDamage(damage);

            // Apply damage
            if (m_DamageMultiplier > 0f && inDamageFilter.CollidesWith(source.outDamageFilter, false))
            {
                int scaledDamage = Mathf.CeilToInt(damage * m_DamageMultiplier);

                if (CombatTextSystem.Instance != null)
                    CombatTextSystem.Instance.CreateCombatTextAI(scaledDamage, transform.position, m_Critical, false);

                if (source.controller != null)
                {
					try
					{
						// Apply damage
						m_EmeraldAISystem.HealthComponent.Damage(
							scaledDamage,
							source.controller.currentCharacter.transform,
                            (int)(damage * m_ForceMultiplier),
                            m_Critical
                            );
					}
					catch (Exception e)
					{
						Debug.LogError("Emerald AI threw an exception: " + e.Message);
					}

                    // Report killed
                    if (!alreadyDead && m_EmeraldAISystem.HealthComponent.Health <= 0 && source.controller.isPlayer)
                        OnPlayerKilledAI();
                }
                else
                {						
					try
					{
                        // Apply damage
                        m_EmeraldAISystem.HealthComponent.Damage(
                            scaledDamage,
                            source.damageSourceTransform,
                            (int)(damage * m_ForceMultiplier),
                            m_Critical
                            );
					}
					catch (Exception e)
					{
						Debug.LogError("Emerald AI threw an exception: " + e.Message);
					}
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