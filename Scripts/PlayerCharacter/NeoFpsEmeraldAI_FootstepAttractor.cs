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

        [SerializeField, Tooltip("Should footsteps attract enemy AI when crouching?")]
        private bool m_CrouchSneak = true;

        private AttractModifier m_AttractModifier = null;

        private void Awake()
        {
            if (m_MotionController != null)
            {
                m_AttractModifier = GetComponent<AttractModifier>();
                m_AttractModifier.TriggerType = TriggerTypes.OnCustomCall;

                m_MotionController.onStep += OnStep;
            }
        }

        private void OnStep()
        {
            if (m_CrouchSneak && m_MotionController.currentHeightNormalised > 0.75f)
                m_AttractModifier.ActivateAttraction();            
        }
    }
}