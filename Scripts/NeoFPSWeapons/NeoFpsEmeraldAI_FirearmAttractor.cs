using NeoFPS;
using NeoFPS.ModularFirearms;
using EmeraldAI.SoundDetection;
using UnityEngine;
using System;

namespace NeoFPS.EmeraldAI
{
    [RequireComponent(typeof(AttractModifier))]
    public class NeoFpsEmeraldAI_FirearmAttractor : MonoBehaviour
    {
        private ModularFirearm m_Firearm = null;
        private AttractModifier m_AttractModifier = null;
        private IShooter m_Shooter = null;

        private void Start()
        {
            m_Firearm = GetComponentInParent<ModularFirearm>();
            if (m_Firearm != null)
            {
                m_Firearm.onShooterChange += OnShooterChanged;
                OnShooterChanged(m_Firearm, m_Firearm.shooter);
                m_AttractModifier = GetComponent<AttractModifier>();
                m_AttractModifier.TriggerType = TriggerTypes.OnCustomCall;
            }
        }

        private void OnEnable()
        {
            if (m_Firearm != null)
            {
                m_Firearm.onShooterChange += OnShooterChanged;
                OnShooterChanged(m_Firearm, m_Firearm.shooter);
            }
        }

        private void OnDisable()
        {
            if (m_Firearm != null)
            {
                m_Firearm.onShooterChange -= OnShooterChanged;
                OnShooterChanged(m_Firearm, null);
            }
        }

        private void OnShooterChanged(IModularFirearm firearm, IShooter shooter)
        {
            if (m_Shooter != null)
                m_Shooter.onShoot -= OnShoot;

            m_Shooter = shooter;

            if (m_Shooter != null)
                m_Shooter.onShoot += OnShoot;
        }

        private void OnShoot(IModularFirearm firearm)
        {
            m_AttractModifier.ActivateAttraction();
        }
    }
}