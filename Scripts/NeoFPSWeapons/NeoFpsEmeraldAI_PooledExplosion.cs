using NeoFPS;
using NeoFPS.ModularFirearms;
using EmeraldAI.SoundDetection;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    [RequireComponent(typeof(AttractModifier))]
    public class NeoFpsEmeraldAI_PooledExplosion : PooledExplosion
    {
        private AttractModifier m_AttractModifier = null;

        public override void Explode(float maxDamage, float maxForce, IDamageSource source = null, Transform ignoreRoot = null)
        {
            base.Explode(maxDamage, maxForce, source, ignoreRoot);

            if (m_AttractModifier == null)
            {
                m_AttractModifier = GetComponent<AttractModifier>();
                m_AttractModifier.TriggerType = TriggerTypes.OnCustomCall;
            }

            m_AttractModifier.ActivateAttraction();
        }
    }
}