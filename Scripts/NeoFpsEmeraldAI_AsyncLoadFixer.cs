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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NeoFPS.EmeraldAI
{
    public class NeoFpsEmeraldAI_AsyncLoadFixer : MonoBehaviour
    {
        public IEnumerator Start()
        {
            yield return null;

            // Emerald AI creates a number of objects from within Awake() functions.
            // When using async load, this will create objects in the active scene.
            // NeoFPS uses async load behind a loading screen, which means the new objects are created in the loading screen scene, not the AI's scene
            // This can be fixed by setting the new objects' scene on creation to gameObject.scene
            // This behaviour recreates the objects that are destroyed when the loading scene unloads

            // Create object pool if destroyed
            if (EmeraldAISystem.ObjectPool == null)
            {
                EmeraldAISystem.ObjectPool = new GameObject();
                EmeraldAISystem.ObjectPool.name = "Emerald Object Pool";
            }

            // Set up combat text system if destroyed
            if (CombatTextSystem.Instance == null)
            {
                GameObject m_CombatTextSystem = Instantiate((GameObject)Resources.Load("Combat Text System") as GameObject, Vector3.zero, Quaternion.identity);
                m_CombatTextSystem.name = "Combat Text System";
                GameObject m_CombatTextCanvas = Instantiate((GameObject)Resources.Load("Combat Text Canvas") as GameObject, Vector3.zero, Quaternion.identity);
                m_CombatTextCanvas.name = "Combat Text Canvas";
                EmeraldAISystem.CombatTextSystemObject = m_CombatTextCanvas;
                CombatTextSystem.Instance.CombatTextCanvas = m_CombatTextCanvas;
                CombatTextSystem.Instance.Initialize();
            }
        }
    }
}
