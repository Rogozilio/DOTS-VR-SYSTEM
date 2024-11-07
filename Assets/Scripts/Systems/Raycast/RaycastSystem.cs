using Aspects;
using Static;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Systems.Raycast
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct RaycastSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
                
            EntityQuery singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            
            foreach (var force in SystemAPI.Query<ForceHandAspect>())
            {
                RaycastInput input = new RaycastInput()
                {
                    Start = force.StartPoint,
                    End = force.EndPoint,
                    Filter = new CollisionFilter()
                    {
                        //BelongsTo = force.BelongTo.Value,
                        //CollidesWith = force.CollidesWith.Value,// all 1s, so all layers, collide with everything
                        GroupIndex = 0
                    }
                };
                
                var raycastHit = new RaycastHit();
                RaycastBurst.SingleRayCast(collisionWorld, input, ref raycastHit);
                
                Debug.Log(state.EntityManager.GetName(raycastHit.Entity));
            }
        }
    }
}