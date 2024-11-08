﻿using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;

namespace Static
{
    public class RaycastBurst
    {
        [BurstCompile]
        public struct RaycastJob : IJobParallelFor
        {
            [ReadOnly] public CollisionWorld world;
            [ReadOnly] public NativeArray<RaycastInput> inputs;
            public NativeArray<RaycastHit> results;

            public void Execute(int index)
            {
                RaycastHit hit;
                world.CastRay(inputs[index], out hit);
                results[index] = hit;
            }
        }

        public static JobHandle ScheduleBatchRayCast(CollisionWorld world,
            NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results)
        {
            JobHandle rcj = new RaycastJob
            {
                inputs = inputs,
                results = results,
                world = world

            }.Schedule(inputs.Length, 4);
            return rcj;
        }
        
        public static void SingleRayCast(CollisionWorld world, RaycastInput input,
            ref RaycastHit result)
        {
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            rayCommands[0] = input;
            var handle = ScheduleBatchRayCast(world, rayCommands, rayResults);
            handle.Complete();
            result = rayResults[0];
            rayCommands.Dispose();
            rayResults.Dispose();
        }
    }
}