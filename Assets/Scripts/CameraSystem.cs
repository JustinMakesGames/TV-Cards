using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public enum CameraKind
{
    LeftGameCamera = 0,
    RightGameCamera = 1,
    MiddleGameCamera = 2,
    RedCamera = 3,
    BlueCamera = 4,
    GreenCamera = 5,
    YellowCamera = 6
}
public class CameraSystem : MonoBehaviour
{

    [SerializeField] private List<GameObject> cameras = new List<GameObject>();
    private CinemachineBrain _camBrain;
    private GameObject _currentCamera;




    private void Awake()
    {
        if (Camera.main.TryGetComponent(out CinemachineBrain brain))
        {
            _camBrain = brain;

        }

        _currentCamera = cameras[0];
    }

    public void ChangeCamera(CameraKind cameraKind)
    {
        GameObject camera = cameras[(int)cameraKind];
        ChangeCamera(camera);  
    }

    public void ChangeCamera(GameObject camera)
    {
        _currentCamera.SetActive(false);
        camera.SetActive(true);
        _currentCamera = camera;
    }

    public void ChangeCameraInterval(int cameraInterval)
    {
        
    } 

}
