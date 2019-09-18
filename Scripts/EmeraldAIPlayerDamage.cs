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

        public void SendPlayerDamage(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent)
        {
            DamageNeoFpsPlayer(DamageAmount, Target, EmeraldComponent);
        }

        void DamageNeoFpsPlayer(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent)
        {        
            // Damage the player health
            var health = GetComponent<NeoFPS.IHealthManager>();
            if (health == null)
                return;

            m_AI = EmeraldComponent;
            health.AddDamage(DamageAmount, false, this);
            
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
