using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    public readonly partial struct AnimationAspect : IAspect
    {
        private readonly AnimationAssetAspect _animationAssetAspect;
        private readonly HandAspect _handAspect;

        private void Blend(string namePose, float inputValue)
        {
            for (var i = 1; i < _handAspect.GetLenghtJoints; i++)
            {
                var lerp = math.nlerp(_animationAssetAspect.defaultPose.joints[i],
                    _animationAssetAspect.GetPose(namePose).joints[i], inputValue);
                var delta = math.mul(lerp, math.inverse(_animationAssetAspect.defaultPose.joints[i]));
                _handAspect.SetJoint(i,math.mul(delta, _handAspect.GetJoint(i)));
            }
        }

        public void Play(string namePose, float inputValue)
        {
            ref var pose = ref _animationAssetAspect.GetPose(namePose);

            for (var i = 1; i < _handAspect.GetLenghtJoints; i++)
            {
                _handAspect.SetJoint(i,math.nlerp(_animationAssetAspect.defaultPose.joints[i],
                    pose.joints[i], inputValue));
            } 
        }

        public void PlayHands()
        {
            if (_handAspect.EntityNearHand == Entity.Null)
            {
                Play("grip", _handAspect.input.ValueRO.gripValue);
                Blend("trigger", _handAspect.input.ValueRO.triggerValue);
            }
            else
            {
                Play(_handAspect.GetNextPose, _handAspect.input.ValueRO.gripValue);
            }
        }
    }
}