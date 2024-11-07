using Components;
using SystemGroups;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial class CameraFollowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var inputCamera = SystemAPI.GetSingleton<InputCamera>();
            CameraSingleton.Instance.SetPosition = inputCamera.position;
            CameraSingleton.Instance.SetRotation = inputCamera.rotation;
        }
    }
}