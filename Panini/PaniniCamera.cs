using System;
using UnityEngine;

namespace PaniniPlugin.Panini;

public class PaniniCamera : MonoBehaviour
{
    private GameObject _player;
    private GameObject _mainCamera; // Child
    private CubeCamera _cubeCamera;
    
    private Shader _shader;
    private Material _mat;
    private RenderTexture _cubeCameraRT;
    
    private int _iCameraCube;
    private int _iHudRT;
    
    private int _iFacing;
    private int _iFOV;
    private int _iWarp;

    private Vector4 _facing = Vector4.zero;

    private void Awake()
    {
        _shader = Panini.Resources.Assets.Value.LoadAsset<Shader>("PaniniShader");
        _cubeCameraRT = Panini.Resources.Assets.Value.LoadAsset<RenderTexture>("CameraCube");
        
        _mat = new Material(_shader);
        
        _iCameraCube = Shader.PropertyToID("_CameraCube");
        _iHudRT = Shader.PropertyToID("_HudRT");
        
        _iFacing = Shader.PropertyToID("_Facing");
        _iFOV = Shader.PropertyToID("_FOV");
        _iWarp = Shader.PropertyToID("_Warp");
    }
    
    private void Start()
    {
        _player = gameObject;
        foreach (Transform tr in transform)
        {
            if (!tr.gameObject.CompareTag("MainCamera")) continue;
            
            _mainCamera = tr.gameObject;
            break;
        }

        _cubeCamera = _mainCamera.GetComponent<CubeCamera>();
    }
    
    private void UpdateFacing()
    {
        var yaw = _player.transform.rotation.eulerAngles.y;
        var pitch = _mainCamera.transform.rotation.eulerAngles.x;
        var roll = _mainCamera.transform.rotation.eulerAngles.z;

        _facing.Set(pitch, yaw, roll, 0f);
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateFacing();
        
        _mat.SetVector(_iFacing, _facing);
        _mat.SetFloat(_iFOV, _cubeCamera.CameraController.cam.fieldOfView);
        _mat.SetFloat(_iWarp, Plugin.ConfigWarp.Value / 100f);
        
        _mat.SetTexture(_iCameraCube, _cubeCameraRT);

        var hudRT = _cubeCamera.HudCamera.targetTexture;
        if (hudRT == null) Debug.LogError("HudRT is null!");
        _mat.SetTexture(_iHudRT, hudRT);
        
        Graphics.Blit(source, destination, _mat);
    }
}