using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CambioCamaraCinemachine : MonoBehaviour
{
    [Header("Camaras Cinemachine")]
    [SerializeField] private CinemachineCamera camPrimeraPersona;
    [SerializeField] private CinemachineCamera camTopDown;

    [Header("Centro de la isla")]
    [SerializeField] private Transform centroIsla;

    [Header("Configuracion Top Down")]
    [SerializeField] private float radio = 35f;
    [SerializeField] private float altura = 25f;

    [Header("Controles Top Down")]
    [SerializeField] private float velocidadGiro = 60f;
    [SerializeField] private float velocidadAltura = 15f;
    [SerializeField] private float alturaMinima = 10f;
    [SerializeField] private float alturaMaxima = 60f;

    [Header("Zoom Top Down")]
    [SerializeField] private float velocidadZoom = 50f;
    [SerializeField] private float radioMinimo = 10f;
    [SerializeField] private float radioMaximo = 80f;

    private bool usandoTopDown;
    private float anguloActual;

    private void Start()
    {
        ActivarPrimeraPersona();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            CambiarCamara();
        }

        if (usandoTopDown)
        {
            ControlarCamaraTopDown();
        }
    }

    private void CambiarCamara()
    {
        if (usandoTopDown)
        {
            ActivarPrimeraPersona();
        }
        else
        {
            ActivarTopDown();
        }
    }

    private void ActivarPrimeraPersona()
    {
        usandoTopDown = false;

        camPrimeraPersona.Priority = 20;
        camTopDown.Priority = 0;
    }

    private void ActivarTopDown()
    {
        usandoTopDown = true;

        camPrimeraPersona.Priority = 0;
        camTopDown.Priority = 20;

        ActualizarCamaraTopDown();
    }

    private void ControlarCamaraTopDown()
    {
        if (centroIsla == null) return;

        // A gira hacia la izquierda
        if (Keyboard.current.aKey.isPressed)
        {
            anguloActual += velocidadGiro * Time.deltaTime;
        }

        // D gira hacia la derecha
        if (Keyboard.current.dKey.isPressed)
        {
            anguloActual -= velocidadGiro * Time.deltaTime;
        }

        // W sube la cámara
        if (Keyboard.current.wKey.isPressed)
        {
            altura += velocidadAltura * Time.deltaTime;
        }

        // S baja la cámara
        if (Keyboard.current.sKey.isPressed)
        {
            altura -= velocidadAltura * Time.deltaTime;
        }

        altura = Mathf.Clamp(altura, alturaMinima, alturaMaxima);

        // Ruedita del mouse para zoom
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll != 0)
            {
                radio -= scroll * velocidadZoom * Time.deltaTime;
                radio = Mathf.Clamp(radio, radioMinimo, radioMaximo);
            }
        }

        ActualizarCamaraTopDown();
    }

    private void ActualizarCamaraTopDown()
    {
        if (centroIsla == null) return;

        float anguloRad = anguloActual * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Sin(anguloRad) * radio,
            altura,
            Mathf.Cos(anguloRad) * radio
        );

        camTopDown.transform.position = centroIsla.position + offset;

        Vector3 direccionAlCentro = centroIsla.position - camTopDown.transform.position;
        camTopDown.transform.rotation = Quaternion.LookRotation(direccionAlCentro);
    }
}