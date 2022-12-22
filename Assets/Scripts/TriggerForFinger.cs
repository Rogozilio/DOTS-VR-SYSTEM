using System;
using UnityEngine;

namespace Scripts
{
    [ExecuteInEditMode]
    public class TriggerForFinger : MonoBehaviour
    {
        private void Update()
        {
            Physics.simulationMode = SimulationMode.Script;
            if (!Application.isPlaying)
            {
                Physics.Simulate(1f / 30f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            other.enabled = false;
        }
    }
}