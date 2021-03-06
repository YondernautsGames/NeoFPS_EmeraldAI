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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI.Example;

namespace EmeraldAI
{
    public class EmeraldAIPlayerDamage : MonoBehaviour, NeoFPS.IDamageSource
    {
        public List<string> ActiveEffects = new List<string>();

        const float k_KickDistance = 0.02f;
        const float k_KickRotation = 5f;
        const float k_KickDuration = 0.5f;

        private EmeraldAISystem m_AI;

        public NeoFPS.DamageFilter outDamageFilter
        {
            get { return NeoFPS.DamageFilter.AllDamageAllTeams; }
            set { }
        }

        public NeoFPS.IController controller
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

        public void SendPlayerDamage(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent, bool CriticalHit = false)
        {
            DamageNeoFpsPlayer(DamageAmount, Target, EmeraldComponent, CriticalHit);

            //Creates damage text on the player's position, if enabled.
            //CombatTextSystem.Instance.CreateCombatText(DamageAmount, transform.position, CriticalHit, false, true);
        }

        void DamageNeoFpsPlayer(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent, bool critical)
        {
            // Damage the player health
            var health = GetComponent<NeoFPS.IHealthManager>();
            if (health == null)
                return;

            m_AI = EmeraldComponent;
            health.AddDamage(DamageAmount, critical, this);

            // Get character head kicker
            var character = GetComponent<NeoFPS.ICharacter>();
            if (character == null)
                return;
            var kicker = character.headTransformHandler.GetComponent<NeoFPS.AdditiveKicker>();
            if (kicker == null)
                return;

            // Get direction of attack
            var direction = transform.position - EmeraldComponent.transform.position;
            direction.y = 0;
            direction.Normalize();

            // Kick the camera position & rotation
            kicker.KickPosition(direction * k_KickDistance, k_KickDuration);
            kicker.KickRotation(Quaternion.AngleAxis(k_KickRotation, Vector3.Cross(direction, Vector3.up)), k_KickDuration);
        }
    }
}
