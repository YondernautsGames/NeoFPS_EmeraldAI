using NeoFPS;
using NeoFPS.ModularFirearms;
using EmeraldAI.SoundDetection;
using UnityEngine;
using EmeraldAI;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_StunExplosion : NeoFpsEmeraldAI_PooledExplosion
    {
        [Header("Emerald AI Stun")]

        [SerializeField, Min(1f), Tooltip("The amount of time to stun the AI for")]
        private float m_StunDuration = 5f;

        private EmeraldCombat m_Combat = null;
        private bool m_Checked = false;

        public override void Explode(float maxDamage, float maxForce, IDamageSource source = null, Transform ignoreRoot = null)
        {
            base.Explode(maxDamage, maxForce, source, ignoreRoot);

            // Apply stun
            if (m_Combat != null )
            {
                m_Combat.TriggerStun(m_StunDuration);
                m_Combat = null;
            }

            m_Checked = false;
        }

        protected override void ApplyExplosionDamageEffect(DamageHandlerInfo info)
        {
            base.ApplyExplosionDamageEffect(info);

            // Check for an Emerald AI combat module
            if (!m_Checked)
            {
                m_Combat = GetComponentInParent<EmeraldCombat>();
                m_Checked = true;
            }
        }
    }
}