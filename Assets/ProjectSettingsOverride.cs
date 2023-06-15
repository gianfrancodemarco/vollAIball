using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ProjectSettingsOverride : MonoBehaviour
{
// Original values
        Vector3 m_OriginalGravity;

        [Tooltip("Increase or decrease the scene gravity. Use ~3x to make things less floaty")]
        public float gravityMultiplier = 1.0f;

        public void Awake()
        {
            // Override
            Physics.gravity *= gravityMultiplier;
            
            // Make sure the Academy singleton is initialized first, since it will create the SideChannels.
            Academy.Instance.EnvironmentParameters.RegisterCallback("gravity", f => { Physics.gravity = new Vector3(0, -f, 0); });
        }

        public void OnDestroy()
        {
            Physics.gravity = m_OriginalGravity;
        }
}
