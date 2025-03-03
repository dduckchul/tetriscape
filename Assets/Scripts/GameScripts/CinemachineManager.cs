using Cinemachine;
using UnityEngine;

public class CinemachineManager : MonoBehaviour
{
    private Camera _camera;
    private Camera _blockCamera;

    public CinemachineClearShot clearShot;
    public CinemachineVirtualCamera ceremonyCamera;
    
    int _blockLayerMask = 7;
    
    private void Awake()
    {
        _camera = Camera.main;
        _blockCamera = _camera?.transform.GetChild(0).GetComponent<Camera>();
    }
    
    // 2D 가상 카메라로 변경
    public void Change2DView()
    {
        clearShot.Priority = 9;
        _camera.orthographic = true;
        _camera.cullingMask &= ~(1 << _blockLayerMask);
        _blockCamera.gameObject.SetActive(true);
    }

    // 3D 가상 카메라로 변경
    public void Change3DView()
    {
        clearShot.Priority = 11;
        _camera.orthographic = false;
        _camera.cullingMask |= (1 << _blockLayerMask);
        _blockCamera.gameObject.SetActive(false);
    }

    public void ChangeToCeremony()
    {
        ceremonyCamera.Priority = 12;
        _camera.orthographic = false;
        _camera.cullingMask |= (1 << _blockLayerMask);
        _blockCamera.gameObject.SetActive(false);        
    }
}
