using EmeraldAI;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_DamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField, Tooltip("The value to multiply any incoming damage by. Use to reduce damage to areas like feet, or raise it for areas like the head.")]
        private float m_Multiplier = 1f;

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