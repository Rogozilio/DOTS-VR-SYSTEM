using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct PhysicsHandAspect : IAspect
    {
        private readonly RefRO<LocalTransform> _localTransform;

        public float3 CalculateVelocityLinear(float3 handPosition, float deltaTime)
        {
            var speed = 50f;
            var direction = DeltaPosition(handPosition);
            var distanceToMove = speed * deltaTime;
            return math.select(direction * distanceToMove, float3.zero,
                math.distance(_localTransform.ValueRO.Position, direction) <= 0.01f);
        }

        private float3 DeltaPosition(float3 handPosition)
        {
            return math.normalize(handPosition - _localTransform.ValueRO.Position);
        }

        public float3 CalculateVelocityAngle(quaternion hand, float deltaTime)
        {
            var deltaRotation = DeltaRotation(hand);
            CalculateAngleAxis(deltaRotation, out var angle, out var axis);
            float angularVelocityMagnitude = math.radians(angle) / deltaTime;
            return math.normalize(axis) * angularVelocityMagnitude;
        }

        private quaternion DeltaRotation(quaternion handRotation)
        {
            return math.mul(math.inverse(_localTransform.ValueRO.Rotation), handRotation);
        }

        private void CalculateAngleAxis(quaternion q, out float angle, out float3 axis)
        {
            // Делаем нормализацию кватерниона на всякий случай
            q = math.normalize(q);

            angle = 2.0f * math.acos(q.value.w); // Угол в радианах
            float s = math.sqrt(1.0f - q.value.w * q.value.w);

            // Ось вращения
            if (s < 0.001f) // Возможно, это приближается к 0, смотрим на случай, когда ось неопределена
            {
                axis = new float3(1.0f, 0.0f, 0.0f); // Возьмем произвольную ось
            }
            else
            {
                axis = new float3(q.value.x, q.value.y, q.value.z) / s; // Нормализация происходит здесь
            }

            // Преобразуем угол в градусы (если нужно)
            angle = math.degrees(angle);
        }
    }
}