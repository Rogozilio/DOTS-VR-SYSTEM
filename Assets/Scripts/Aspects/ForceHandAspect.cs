using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct ForceHandAspect : IAspect
    {
        private readonly RefRO<Raycast> _raycast;
        private readonly RefRO<LocalToWorld> _localToWorld;

        public float3 StartPoint => _localToWorld.ValueRO.Position;

        public float3 EndPoint =>
            _localToWorld.ValueRO.Position + _localToWorld.ValueRO.Value[(int)_raycast.ValueRO.directionType].xyz *
            _raycast.ValueRO.lenght;
    }
}