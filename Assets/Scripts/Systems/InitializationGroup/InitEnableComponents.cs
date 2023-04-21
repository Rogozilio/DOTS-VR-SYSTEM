using Components;
using EnableComponents;
using Enums;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems.InitializationGroup
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitEnableComponents : ISystem, ISystemStartStop
    {
        private bool _isCreateEnableStaticComponent;
        private bool _isCreateEnableDynamicComponent;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InteractiveObject>();
            state.RequireForUpdate<Hand>();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            foreach (var interactiveObject in SystemAPI.Query<RefRO<InteractiveObject>>())
            {
                switch (interactiveObject.ValueRO.interactiveType)
                {
                    case InteractiveType.Static:
                        _isCreateEnableStaticComponent = true;
                        break;
                    case InteractiveType.Dynamic:
                        _isCreateEnableDynamicComponent = true;
                        break;
                }
            }

            if (_isCreateEnableStaticComponent)
            {
                var entities = state.GetEntityQuery(typeof(Hand));
                state.EntityManager.AddComponent<EnableStaticState>(entities);
                state.CompleteDependency();
                entities = state.GetEntityQuery(typeof(EnableStaticState));
                state.EntityManager.SetComponentEnabled<EnableStaticState>(entities, false);
            }
            if (_isCreateEnableDynamicComponent)
            {
                var entities = state.GetEntityQuery(typeof(Hand));
                state.EntityManager.AddComponent<EnableDynamicState>(entities);
                state.CompleteDependency();
                entities = state.GetEntityQuery(typeof(EnableDynamicState));
                state.EntityManager.SetComponentEnabled<EnableDynamicState>(entities, false);
            }

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
        }
    }
}