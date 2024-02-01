using NeoFPS;
using NeoFPS.ModularFirearms;
using EmeraldAI.SoundDetection;
using UnityEngine;
using NeoFPS.CharacterMotion;
using System;
using NeoFPS.CharacterMotion.Parameters;

namespace NeoFPS.EmeraldAI
{
    [RequireComponent(typeof(AttractModifier))]
    public class NeoFpsEmeraldAI_FootstepAttractor : MonoBehaviour
    {
        [SerializeField, RequiredObjectProperty, Tooltip("The NeoFPS character's motion controller. This is what tracks the character's footsteps and sends them to Emerald's AttractModifier")]
        private MotionController m_MotionController = null;

        [SerializeField, Tooltip("The name of a switch parameter on the character's motion graph which is set to true while crouching. This prevents attracting AI while crouching")]
        private string m_CrouchingKey = "isCrouching";

        private AttractModifier m_AttractModifier = null;
        private SwitchParameter m_CrouchingSwitch = null;

        private void Awake()
        {
            if (m_MotionController != null)
            {
                m_AttractModifier = GetComponent<AttractModifier>();
                m_AttractModifier.TriggerType = TriggerTypes.OnCustomCall;

                m_MotionController.onStep += OnStep;

                if (!string.IsNullOrEmpty(m_CrouchingKey))
                    m_CrouchingSwitch = m_MotionController.motionGraph.GetSwitchProperty(m_CrouchingKey);
            }
        }

        private void OnStep()
        {
            if (m_CrouchingSwitch == null || !m_CrouchingSwitch.on)
                m_AttractModifier.ActivateAttraction();
        }
    }
}