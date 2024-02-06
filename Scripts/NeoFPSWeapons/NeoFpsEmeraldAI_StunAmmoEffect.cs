using EmeraldAI;
using NeoFPS.ModularFirearms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_StunAmmoEffect : BaseAmmoEffect
    {
        [SerializeField, Min(1f), Tooltip("The amount of time to stun the AI for")]
        private float m_StunDuration = 5f;

        [SerializeField, NeoPrefabField, Tooltip("The object to spawn at the impact location if the bullet hits a stunnable target. If this is not assigned then it will show the impact effect instead")]
        private ParticleImpactEffect m_StunEffect = null;

        [SerializeField, NeoPrefabField, Tooltip("The object to spawn at the impact location if the object has not hit a stunnable target")]
        private ParticleImpactEffect m_ImpactEffect = null;

        private PooledObject m_ImpactEffectPrototype = null;
        private PooledObject m_StunEffectPrototype = null;

        protected void Awake()
        {
            if (m_StunEffect != null)
                m_StunEffectPrototype = m_StunEffect.GetComponent<PooledObject>();
            if (m_ImpactEffect != null)
                m_ImpactEffectPrototype = m_ImpactEffect.GetComponent<PooledObject>();
        }

        public override void Hit(RaycastHit hit, Vector3 rayDirection, float totalDistance, float speed, IDamageSource damageSource)
        {
            bool stunEffect = false;
            if (hit.collider.CompareTag("AI"))
            {
                // Stun the AI
                var combat = hit.collider.GetComponentInParent<EmeraldCombat>();
                if (combat != null)
                {
                    combat.TriggerStun(m_StunDuration);

                    // Spawn impact particle effect
                    if (m_StunEffectPrototype != null)
                    {
                        PoolManager.GetPooledObject<ParticleImpactEffect>(m_StunEffectPrototype, hit.point, Quaternion.LookRotation(hit.normal));
                        stunEffect = true;
                    }
                }
            }

            if (!stunEffect)
            {
                // Spawn impact particle effect
                if (m_ImpactEffectPrototype != null)
                    PoolManager.GetPooledObject<ParticleImpactEffect>(m_ImpactEffectPrototype, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}