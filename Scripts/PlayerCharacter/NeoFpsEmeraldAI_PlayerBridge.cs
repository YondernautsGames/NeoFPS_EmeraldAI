using EmeraldAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    [RequireComponent(typeof(NeoFpsEmeraldAI_TargetPositionModifier))]
    [RequireComponent(typeof(FactionExtension))]
    public class NeoFpsEmeraldAI_PlayerBridge : EmeraldPlayerBridge
    {
        [SerializeField, Tooltip("The player character's damage handler that emerald AI damage will be sent to. It doesn't seem to have the ability to target specific transforms.")]
        private Transform m_DefaultDamageHandler = null;

        private IDamageHandler m_TargetDamageHandler = null;
        private IHealthManager m_HealthManager = null;
        private IQuickSlots m_QuickSlots = null;
        private IMeleeWeapon m_MeleeWeapon = null;

        private bool m_Attacking = false;
        private bool m_Blocking = false;

        public override void Awake()
        {
            base.Awake();

            m_HealthManager = GetComponent<IHealthManager>();
            if (m_HealthManager != null)
            {
                m_HealthManager.onHealthChanged += OnHealthChanged;
                m_HealthManager.onIsAliveChanged += OnIsAliveChanged;
            }

            m_QuickSlots = GetComponent<IQuickSlots>();
            if (m_QuickSlots != null)
            {
                m_QuickSlots.onSelectionChanged += OnWieldableSelectionChanged;
            }

            // Get the default damage handler, or root damage handler
            if (m_DefaultDamageHandler != null)
                m_TargetDamageHandler = m_DefaultDamageHandler.GetComponent<IDamageHandler>();
            if (m_TargetDamageHandler == null)
                m_TargetDamageHandler = GetComponent<IDamageHandler>();
        }

        public override void Start()
        {
            base.Start();

            if (m_HealthManager != null)
                StartHealth = Mathf.RoundToInt(m_HealthManager.health);
        }

        public override void DamageCharacterController(int DamageAmount, Transform Target)
        {
            if (m_TargetDamageHandler != null)
                m_TargetDamageHandler.AddDamage(DamageAmount);
            else
                m_HealthManager.AddDamage(DamageAmount);
        }

        public override bool IsAttacking()
        {
            return m_Attacking;
        }

        public override bool IsBlocking()
        {
            return m_Blocking;
        }

        public override bool IsDodging()
        {
            return base.IsDodging();
        }

        // TODO: Stun

        private void OnHealthChanged(float from, float to, bool critical, IDamageSource source)
        {
            Health = Mathf.RoundToInt(to);
        }

        private void OnIsAliveChanged(bool alive)
        {
            // ???
        }

        private void OnWieldableSelectionChanged(int index, IQuickSlotItem item)
        {
            if (m_MeleeWeapon != null)
            {
                m_MeleeWeapon.onAttackingChange -= OnMeleeAttackingChanged;
                m_MeleeWeapon.onBlockStateChange -= OnMeleeBlockingChanged;
                m_MeleeWeapon = null;
                m_Attacking = false;
                m_Blocking = false;
            }

            if (item != null)
                m_MeleeWeapon = item.wieldable as IMeleeWeapon;

            if (m_MeleeWeapon != null)
            {

            }
        }

        private void OnMeleeAttackingChanged(bool attacking)
        {
            m_Attacking = attacking;
        }

        private void OnMeleeBlockingChanged(bool blocking)
        {
            m_Blocking = blocking;
        }
    }
}