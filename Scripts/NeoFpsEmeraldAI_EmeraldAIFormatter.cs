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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI;

namespace NeoSaveGames.Serialization.Formatters
{
    public class EmeraldAISystemFormatter : NeoSerializationFormatter<EmeraldAISystem>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        static void Register()
        {
            NeoSerializationFormatters.RegisterFormatter(new EmeraldAISystemFormatter());
        }
        
        private static readonly NeoSerializationKey k_ActiveEffectsKey = new NeoSerializationKey("activeEffects");
        private static readonly NeoSerializationKey k_CurrentHealthKey = new NeoSerializationKey("currentHealth");
        private static readonly NeoSerializationKey k_CurrentDamageKey = new NeoSerializationKey("currentDamage");
        private static readonly NeoSerializationKey k_IsDeadKey = new NeoSerializationKey("isDead");
        private static readonly NeoSerializationKey k_CurrentAggroHitsKey = new NeoSerializationKey("currentAggroHits");
        private static readonly NeoSerializationKey k_CurrentTargetKey = new NeoSerializationKey("currentTarget");
        private static readonly NeoSerializationKey k_CurrentFollowKey = new NeoSerializationKey("currentFollow");

        protected override void WriteProperties(INeoSerializer writer, EmeraldAISystem from, NeoSerializedGameObject nsgo)
        {
            writer.WriteValue(k_CurrentHealthKey, from.CurrentHealth);
            writer.WriteValue(k_CurrentDamageKey, from.CurrentDamageAmount);
            writer.WriteValue(k_CurrentAggroHitsKey, from.CurrentAggroHits);
            writer.WriteValue(k_IsDeadKey, from.IsDead);

            if (from.ActiveEffects.Count > 0)
                writer.WriteValues(k_ActiveEffectsKey, from.ActiveEffects);

            writer.WriteTransformReference(k_CurrentTargetKey, from.CurrentTarget, nsgo);
            writer.WriteTransformReference(k_CurrentFollowKey, from.CurrentFollowTarget , nsgo);
        }

        protected override void ReadProperties(INeoDeserializer reader, EmeraldAISystem to, NeoSerializedGameObject nsgo)
        {
            reader.TryReadValue(k_CurrentHealthKey, out to.CurrentHealth, to.CurrentHealth);
            reader.TryReadValue(k_CurrentDamageKey, out to.CurrentDamageAmount, to.CurrentDamageAmount);
            reader.TryReadValue(k_CurrentAggroHitsKey, out to.CurrentAggroHits, to.CurrentAggroHits);
            reader.TryReadValue(k_IsDeadKey, out to.IsDead, to.IsDead);

            reader.TryReadValues(k_ActiveEffectsKey, to.ActiveEffects);

            reader.TryReadTransformReference(k_CurrentTargetKey, out to.CurrentTarget, nsgo);
            reader.TryReadTransformReference(k_CurrentFollowKey, out to.CurrentFollowTarget, nsgo);
            
            /*
             * Properties to investigate. Current save is very basic
             * 
            
            if (to.MeleeAttacks.Count > 0)
            {
                reader.TryReadValue("", out to.MeleeAttackIndex, to.MeleeAttackIndex);
                reader.TryReadValue("", out to.MeleeAttacksListIndex, to.MeleeAttacksListIndex);
                reader.TryReadValue("", out to.CurrentAnimationIndex, to.CurrentAnimationIndex);
                reader.TryReadValue("", out to.AttackTimer, to.AttackTimer);
            }

            if (to.MeleeRunAttacks.Count > 0)
            {
                reader.TryReadValue("", out to.MeleeRunAttackIndex, to.MeleeRunAttackIndex);
                reader.TryReadValue("", out to.MeleeRunAttacksListIndex, to.MeleeRunAttacksListIndex);
                reader.TryReadValue("", out to.CurrentRunAttackAnimationIndex, to.CurrentRunAttackAnimationIndex);
                reader.TryReadValue("", out to.RunAttackTimer, to.RunAttackTimer);
                reader.TryReadValue("", out to.RunAttackSpeed, to.RunAttackSpeed);
            }

            if (to.OffensiveAbilities.Count > 0)
            {
                reader.TryReadValue("", out to.OffensiveAbilityIndex, to.OffensiveAbilityIndex);
            }

            if (to.SupportAbilities.Count > 0)
            {
                reader.TryReadValue("", out to.SupportAbilityIndex, to.SupportAbilityIndex);
            }

            if (to.SummoningAbilities.Count > 0)
            {
                reader.TryReadValue("", out to.SummoningAbilityIndex, to.SummoningAbilityIndex);
                reader.TryReadValue("", out to.TotalSummonedAI, to.TotalSummonedAI);

                //CurrentSummoner ?? EmeraldAI Ref
            }

            reader.TryReadValue("", out to.HealingCooldownTimer, to.HealingCooldownTimer); // Private
            reader.TryReadValue("", out to.m_AbilityPicked, to.m_AbilityPicked);
            reader.TryReadValue("", out to.m_InitialTargetPosition, to.m_InitialTargetPosition);
            reader.TryReadValue("", out to.ObstructionTimer, to.ObstructionTimer);
            reader.TryReadValue("", out to.m_ObstructedTimer, to.m_ObstructedTimer); // Private
            reader.TryReadValue("", out to.m_ProjectileCollisionPoint, to.m_ProjectileCollisionPoint);

            reader.TryReadValue("", out to.ReturnToStationaryPosition, to.ReturnToStationaryPosition);
            reader.TryReadValue("", out to.SurfaceNormal, to.SurfaceNormal);
            reader.TryReadValue("", out to.NormalRotation, to.NormalRotation); // Private
            reader.TryReadValue("", out to.AngleCheckTimer, to.AngleCheckTimer); // Private

            reader.TryReadValue("", out to.CurrentMovementState, to.CurrentMovementState); // Private
            reader.TryReadValue("", out to.CurrentBlockingState, to.CurrentBlockingState); // Private

            reader.TryReadValue("", out to.BackingUpTimer, to.BackingUpTimer);
            reader.TryReadValue("", out to.BackingUpSeconds, to.BackingUpSeconds);
            reader.TryReadValue("", out to.BackupDestination, to.BackupDestination);

            reader.TryReadValue("", out to.Attacking, to.Attacking);
            reader.TryReadValue("", out to.GettingHit, to.GettingHit);

            reader.TryReadValue("", out to.m_SwitchingWeaponTypes, to.m_SwitchingWeaponTypes);
            reader.TryReadValue("", out to.m_WeaponTypeSwitchDelay, to.m_WeaponTypeSwitchDelay); // Private

            reader.TryReadValue("", out to.AngleToTurn, to.AngleToTurn);
            reader.TryReadValue("", out to.AlignmentSpeed, to.AlignmentSpeed);

            reader.TryReadValue("", out to.IdleAnimationTimer, to.IdleAnimationTimer); // Private

            if (to.IsMoving)
            {
                reader.TryReadValue("", out to.WaypointTimer, to.WaypointTimer);
                reader.TryReadValue("", out to.WaypointIndex, to.WaypointIndex);
                reader.TryReadValue("", out to.m_LastWaypointIndex, to.m_LastWaypointIndex); // Private

                reader.TryReadValue("", out to.NewDestination, to.NewDestination);
                reader.TryReadValue("", out to.DistanceFromDestination, to.DistanceFromDestination); // Private
                reader.TryReadValue("", out to.TargetDestination, to.TargetDestination);
                reader.TryReadValue("", out to.DestinationAdjustedAngle, to.DestinationAdjustedAngle);
                reader.TryReadValue("", out to.DestinationDirection, to.DestinationDirection); // Private
                reader.TryReadValue("", out to.SingleDestination, to.SingleDestination);

                reader.TryReadValue("", out to.Velocity, to.Velocity); // Private
                reader.TryReadValue("", out to.AdjustedSpeed, to.AdjustedSpeed); // Private

                reader.TryReadValue("", out to.TargetQ, to.TargetQ); // Private
                reader.TryReadValue("", out to.GroundQ, to.GroundQ); // Private

                reader.TryReadValue("", out to.PreviousAngle, to.PreviousAngle); // Private
            }
            */
        }
    }
}