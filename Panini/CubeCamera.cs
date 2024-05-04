using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace PaniniPlugin.Panini;

public class CubeCamera : MonoBehaviour
{
    private Camera _cam;
    
    public CameraController CameraController { get; private set; }
    public RenderTexture RenderTexture { get; private set; }
    public Camera HudCamera { get; private set; }

    private void Awake()
    {
        RenderTexture = Panini.Resources.Assets.Value.LoadAsset<RenderTexture>("CameraCube");
        CameraController = this.GetComponent<CameraController>();
    }

    private void Start()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null) throw new Exception("This object is not a camera!");
        
        foreach (Transform tr in gameObject.transform)
        {
            if (tr.gameObject.name != "HUD Camera") continue;
            
            HudCamera = tr.gameObject.GetComponent<Camera>();
            break;
        }
    }

    private void LateUpdate()
    {
        _cam.RenderToCubemap(RenderTexture, 0b111111);
    }
}