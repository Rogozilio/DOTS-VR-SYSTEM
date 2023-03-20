using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    public readonly partial struct AnimationAspect : IAspect
    {
        private readonly AnimationAssetAspect animationAssetAspect;
        private readonly HandAspect handAspect;

        private void Blend(string namePose, float inputValue)
        {
            for (var i = 1; i < handAspect.GetLenghtJoints; i++)
            {
                var lerp = math.nlerp(animationAssetAspect.defaultPose.joints[i],
                    animationAssetAspect.GetPose(namePose).joints[i], inputValue);
                var delta = math.mul(lerp, math.inverse(animationAssetAspect.defaultPose.joints[i]));
                handAspect.SetJoint(i,math.mul(delta, handAspect.GetJoint(i)));
            }
        }

        public void Play(string namePose, float inputValue)
        {
            ref var pose = ref animationAssetAspect.GetPose(namePose);

            for (var i = 1; i < handAspect.GetLenghtJoints; i++)
            {
                handAspect.SetJoint(i,math.nlerp(animationAssetAspect.defaultPose.joints[i],
                    pose.joints[i], inputValue));
            }
        }

        public void PlayHands()
        {
            if (handAspect.EntityInHand == Entity.Null)
            {
                Play("grip", handAspect.input.ValueRO.gripValue);
                Blend("trigger", handAspect.input.ValueRO.triggerValue);
            }
            else
            {
                Play(handAspect.GetNextPose, handAspect.input.ValueRO.gripValue);
            }
        }
    }
}