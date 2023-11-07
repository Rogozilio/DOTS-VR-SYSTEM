using System.Collections.Generic;
using UnityEngine;

namespace DebugDraw
{
    public class DebugDrawLineSingleton : MonoBehaviour
    {
        public static DebugDrawLineSingleton Instance;
        
        public List<LineRenderer> lineRenderer;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void SetLine(int index, params Vector3[] point)
        {
            lineRenderer[index].positionCount = point.Length;
            lineRenderer[index].SetPositions(point);
        }
    }
}