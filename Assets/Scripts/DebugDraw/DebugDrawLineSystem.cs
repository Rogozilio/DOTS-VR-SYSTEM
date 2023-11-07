using Aspects;
using Unity.Entities;
using UnityEngine;

namespace DebugDraw
{
    public partial class DebugDrawLineSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var debugDrawLine = DebugDrawLineSingleton.Instance;

            if (debugDrawLine == null)
            {
                Debug.LogError("DebugDrawLine singleton not found");
                return;
            }

            Entities.ForEach((int entityInQueryIndex, ForceHandAspect force) =>
            {
                debugDrawLine.SetLine(entityInQueryIndex, force.StartPoint, force.EndPoint);
            }).WithoutBurst().Run();
        }
    }
}