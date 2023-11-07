using Components;
using Singletons;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Systems.InitializationGroup
{
    public partial struct InitCollisionFilterValue : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollisionFilterSettings>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionFilterSettings = SystemAPI.GetSingleton<CollisionFilterSettings>();
            
            foreach (var collider in SystemAPI.Query<PhysicsCollider>().WithAll<InteractiveObject>())
            {
                collider.Value.Value.SetCollisionFilter(collisionFilterSettings.itemCollisionFilter);
            }
            
            foreach (var collider in SystemAPI.Query<PhysicsCollider>().WithNone<InteractiveObject>())
            {
                collider.Value.Value.SetCollisionFilter(collisionFilterSettings.otherCollisionFilter);
            }
            
            state.Enabled = false;
        }
    }
}