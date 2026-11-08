using UnityEngine;

public class CameraWatcher : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(_camera.transform);
        transform.Rotate(0, 180, 0);
    }
}
