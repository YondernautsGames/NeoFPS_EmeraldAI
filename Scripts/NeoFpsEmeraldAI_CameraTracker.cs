using EmeraldAI;
using EmeraldAI.Utility;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public static class NeoFpsEmeraldAI_CameraTracker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AttachCameraTracker()
        {
            FirstPersonCameraBase.onCurrentCameraChanged += OnFirstPersonCameraChanged;
        }

        private static bool m_Set = false;

        private static void OnFirstPersonCameraChanged(FirstPersonCameraBase cam)
        {
            if (cam != null)
            {
                if (cam is FirstPersonCamera)
                {
                    EmeraldAIHealthBar.m_Camera = cam.unityCamera;
                    m_Set = true;
                }
            }
            else
            {
                if (m_Set)
                {
                    EmeraldAIHealthBar.m_Camera = Camera.main;
                    m_Set = false;
                }
            }
        }
    }
}
