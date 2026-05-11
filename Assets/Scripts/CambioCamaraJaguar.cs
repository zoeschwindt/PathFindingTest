using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CambioCamaraJaguar : MonoBehaviour
{
    [Header("Camaras Cinemachine")]
    [SerializeField] private CinemachineCamera camTerceraPersona;
    [SerializeField] private CinemachineCamera camPrimeraPersona;

    [Header("Referencias")]
    [SerializeField] private Transform jaguar;
    [SerializeField] private Transform puntoCamaraTercera;
    [SerializeField] private Transform puntoCamaraPrimera;

    [Header("Tercera persona")]
    [SerializeField] private float distanciaTercera = 7f;
    [SerializeField] private float alturaTercera = 2.5f;
    [SerializeField] private float sensibilidadTercera = 0.15f;
    [SerializeField] private float pitchMinTercera = -10f;
    [SerializeField] private float pitchMaxTercera = 55f;

    [Header("Primera persona")]
    [SerializeField] private float sensibilidadPrimera = 0.12f;
    [SerializeField] private float limiteHorizontalPrimera = 90f;
    [SerializeField] private float pitchMinPrimera = -25f;
    [SerializeField] private float pitchMaxPrimera = 35f;

    [Header("Cursor")]
    [SerializeField] private bool bloquearCursor = true;

    private bool usandoPrimeraPersona;

    private float yawTercera;
    private float pitchTercera = 20f;

    private float yawPrimera;
    private float pitchPrimera;

    private void Start()
    {
        ActivarTerceraPersona();

        if (bloquearCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            CambiarCamara();
        }

        LeerMouse();
    }

    private void LateUpdate()
    {
        if (usandoPrimeraPersona)
        {
            ActualizarPrimeraPersona();
        }
        else
        {
            ActualizarTerceraPersona();
        }
    }

    private void CambiarCamara()
    {
        if (usandoPrimeraPersona)
        {
            ActivarTerceraPersona();
        }
        else
        {
            ActivarPrimeraPersona();
        }
    }

    private void ActivarTerceraPersona()
    {
        usandoPrimeraPersona = false;

        camTerceraPersona.Priority = 20;
        camPrimeraPersona.Priority = 0;
    }

    private void ActivarPrimeraPersona()
    {
        usandoPrimeraPersona = true;

        camTerceraPersona.Priority = 0;
        camPrimeraPersona.Priority = 20;

        yawPrimera = 0f;
        pitchPrimera = 0f;
    }

    private void LeerMouse()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        if (usandoPrimeraPersona)
        {
            yawPrimera += mouseDelta.x * sensibilidadPrimera;
            pitchPrimera -= mouseDelta.y * sensibilidadPrimera;

            yawPrimera = Mathf.Clamp(yawPrimera, -limiteHorizontalPrimera, limiteHorizontalPrimera);
            pitchPrimera = Mathf.Clamp(pitchPrimera, pitchMinPrimera, pitchMaxPrimera);
        }
        else
        {
            yawTercera += mouseDelta.x * sensibilidadTercera;
            pitchTercera -= mouseDelta.y * sensibilidadTercera;

            pitchTercera = Mathf.Clamp(pitchTercera, pitchMinTercera, pitchMaxTercera);
        }
    }

    private void ActualizarTerceraPersona()
    {
        if (jaguar == null || camTerceraPersona == null) return;

        Transform objetivo = puntoCamaraTercera != null ? puntoCamaraTercera : jaguar;

        Quaternion rotacion = Quaternion.Euler(pitchTercera, yawTercera, 0f);

        Vector3 offset = rotacion * new Vector3(0f, alturaTercera, -distanciaTercera);

        camTerceraPersona.transform.position = objetivo.position + offset;

        Vector3 puntoMirar = objetivo.position + Vector3.up * 1f;
        camTerceraPersona.transform.rotation = Quaternion.LookRotation(puntoMirar - camTerceraPersona.transform.position);
    }

    private void ActualizarPrimeraPersona()
    {
        if (jaguar == null || camPrimeraPersona == null) return;

        Transform objetivo = puntoCamaraPrimera != null ? puntoCamaraPrimera : jaguar;

        camPrimeraPersona.transform.position = objetivo.position;

        Quaternion rotacionBase = jaguar.rotation;
        Quaternion rotacionMouse = Quaternion.Euler(pitchPrimera, yawPrimera, 0f);

        camPrimeraPersona.transform.rotation = rotacionBase * rotacionMouse;
    }
}