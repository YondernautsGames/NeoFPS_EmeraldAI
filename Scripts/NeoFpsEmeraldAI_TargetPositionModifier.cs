/*
 * Copyright 2020 Yondernauts Games Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using EmeraldAI;
using EmeraldAI.Utility;
using NeoCC;
using System;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    [RequireComponent(typeof (NeoCharacterController))]
    public class NeoFpsEmeraldAI_TargetPositionModifier : TargetPositionModifier
    {
        [SerializeField, Tooltip("The distance down from the top of the character collider to aim for. This means that AI will look and aim at the right position when the NeoFPS character crouches.")]
        private float m_OffsetFromTop = 0.25f;

        // Set nicer default values
        protected void Reset()
        {
            PositionModifier = 1.25f;
            GizmoRadius = 0.375f;
        }

        protected void Start()
        {
            if (!Application.isPlaying)
                return;

            // Connect to the NeoCharacterController height changed event
            var ncc = GetComponent<NeoCharacterController>();
            if (ncc != null)
            {
                ncc.onHeightChanged += OnHeightChanged;
                OnHeightChanged(ncc.height, 0f);
            }

        }

        private void OnHeightChanged(float newHeight, float rootOffset)
        {
            // Set the position modifier to the character height minus the offset
            PositionModifier = newHeight - m_OffsetFromTop;
        }
    }
}