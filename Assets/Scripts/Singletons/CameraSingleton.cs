using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Vector3 SetPosition
    {
        set => transform.position = value;
    }
    public Quaternion SetRotation
    {
        set => transform.rotation = value;
    }
}