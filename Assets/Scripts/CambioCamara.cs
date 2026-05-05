using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CambioCamara : MonoBehaviour
{
    public CinemachineCamera camPrimera;
    public CinemachineCamera camTop;

    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            if (camPrimera.Priority > camTop.Priority)
            {
                camPrimera.Priority = 0;
                camTop.Priority = 10;
            }
            else
            {
                camPrimera.Priority = 10;
                camTop.Priority = 0;
            }
        }
    }
}