using System;
using EmeraldAI;
using NeoFPS.ModularFirearms;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    /// <summary>
    /// This component will connect an Emerald AI to NeoFPS weapons to automatically enable sound detection without
    /// having to edit NeoFPS code.
    /// 
    /// To use add this component and `SoloPlayerCharacterEventWatcher` to your AI character.
    /// </summary>
    [RequireComponent(typeof(EmeraldAISystem))]
    [RequireComponent(typeof(SoloPlayerCharacterEventWatcher))]
    public class NeoFpsEmeraldAI_AISoundDetection : MonoBehaviour, IPlayerCharacterSubscriber
    {
        [SerializeField, Tooltip("The radius within which the AI may detect sound.")]
        float m_Radius = 25;

        private SoloPlayerCharacterEventWatcher m_Watcher;
        private BaseShooterBehaviour m_Shooter;
        private FpsInventoryBase m_Inventory;
        private Transform m_PlayerTransform;
        private EmeraldAISystem m_Ai;
        private float m_TimeOfNextCheck;
        private float m_ResetSoundAwarenessAfter = 2;

        protected void Awake()
        {
            m_Watcher = GetComponent<SoloPlayerCharacterEventWatcher>();
            m_Ai = GetComponent<EmeraldAISystem>();
        }

        protected void OnEnable()
        {
            m_Watcher.AttachSubscriber(this);
            if (m_Shooter != null)
                m_Shooter.onShoot += DetectSound;
        }

        private void OnDisable()
        {
            m_Watcher.ReleaseSubscriber(this);
            if (m_Shooter != null)
                m_Shooter.onShoot -= DetectSound;
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (m_Inventory)
            {
                m_Inventory.onSelectionChanged -= OnWieldableSelectionChanged;
                OnWieldableSelectionChanged(0, null);
            }

            if (character as Component != null)
            {
                m_Inventory = character.GetComponent<FpsInventoryBase>();
                m_PlayerTransform = character.transform;
            }
            else
            {
                m_Inventory = null;
                m_PlayerTransform = null;
            }

            if (m_Inventory != null)
            {
                m_Inventory.onSelectionChanged += OnWieldableSelectionChanged;
                OnWieldableSelectionChanged(0, m_Inventory.selected);
            }
        }

        private void OnWieldableSelectionChanged(int index, IQuickSlotItem item)
        {
            if (m_Shooter != null)
            {
                m_Shooter.onShoot -= DetectSound;
                m_Shooter = null;
            }
            
            if (item != null)
            {
                if (item.GetComponent<BaseShooterBehaviour>())
                {
                    m_Shooter = item.GetComponent<BaseShooterBehaviour>();
                    if (m_Shooter)
                    {
                        m_Shooter.onShoot += DetectSound;
                    }
                }
            }
        }

        private void DetectSound(IModularFirearm source)
        {
            if (Time.timeSinceLevelLoad < m_TimeOfNextCheck)
                return;

            m_TimeOfNextCheck += m_ResetSoundAwarenessAfter;
            Check();
              
        }

        private void Check()
        {
            if (m_Ai.CurrentTarget == null && m_PlayerTransform != null && Vector3.Distance(transform.position, m_PlayerTransform.position) < m_Radius)
                m_Ai.CurrentTarget = m_PlayerTransform;
            // Perform additional checks here, like raycasts maybe
        }
    }
}