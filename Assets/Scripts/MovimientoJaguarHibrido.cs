using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class MovimientoJaguarHibrido : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera camara;

    [Header("Movimiento con click")]
    [SerializeField] private float distanciaBusquedaNavMesh = 3f;

    [Header("Movimiento WASD")]
    [SerializeField] private float velocidadCaminar = 5f;
    [SerializeField] private float velocidadCorrer = 9f;
    [SerializeField] private float velocidadRotacion = 12f;

    [Header("Gravedad")]
    [SerializeField] private float gravedad = -25f;
    [SerializeField] private float fuerzaSalto = 6f;

    [Header("Agua")]
    [SerializeField] private string tagAgua = "Agua";
    [SerializeField] private bool estaEnAgua;
    [SerializeField] private float velocidadEnAgua = 3f;
    [SerializeField] private float gravedadEnAgua = -8f;

    [Header("Animación")]
    [SerializeField] private string parametroVelocidad = "Speed";

    private float velocidadVertical;
    private bool modoManual;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (camara == null)
            camara = Camera.main;
    }

    private void Start()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
    }

    private void Update()
    {
        bool usandoWASD = HayInputMovimiento();

        if (usandoWASD)
        {
            ActivarModoManual();
            MoverConWASD();
        }
        else
        {
            LeerClick();
            ActualizarAnimacionNavMesh();
        }

        AplicarGravedad();
    }

    private bool HayInputMovimiento()
    {
        if (Keyboard.current == null) return false;

        return Keyboard.current.wKey.isPressed ||
               Keyboard.current.sKey.isPressed ||
               Keyboard.current.aKey.isPressed ||
               Keyboard.current.dKey.isPressed;
    }

    private void LeerClick()
    {
        if (Mouse.current == null || camara == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = camara.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, distanciaBusquedaNavMesh, NavMesh.AllAreas))
                {
                    ActivarModoNavMesh(navHit.position);
                }
            }
        }
    }

    private void ActivarModoNavMesh(Vector3 destino)
    {
        modoManual = false;

        if (!agent.enabled)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        agent.isStopped = false;
        agent.SetDestination(destino);
    }

    private void ActivarModoManual()
    {
        if (modoManual) return;

        modoManual = true;

        if (agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    private void MoverConWASD()
    {
        if (Keyboard.current == null) return;

        Vector3 direccion = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            direccion += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            direccion -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            direccion -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            direccion += transform.right;

        direccion.y = 0f;

        if (direccion.magnitude > 1f)
            direccion.Normalize();

        bool corriendo = Keyboard.current.leftShiftKey.isPressed;

        float velocidadActual = estaEnAgua
            ? velocidadEnAgua
            : corriendo ? velocidadCorrer : velocidadCaminar;

        controller.Move(direccion * velocidadActual * Time.deltaTime);

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }

        ActualizarAnimacion(direccion.magnitude * velocidadActual);
    }

    private void AplicarGravedad()
    {
        if (agent.enabled && !modoManual)
            return;

        if (controller.isGrounded && velocidadVertical < 0f)
        {
            velocidadVertical = -3f;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (controller.isGrounded || estaEnAgua)
            {
                velocidadVertical = fuerzaSalto;
            }
        }

        float gravedadActual = estaEnAgua ? gravedadEnAgua : gravedad;
        velocidadVertical += gravedadActual * Time.deltaTime;

        Vector3 movimientoVertical = Vector3.up * velocidadVertical;
        controller.Move(movimientoVertical * Time.deltaTime);
    }

    private void ActualizarAnimacionNavMesh()
    {
        if (agent != null && agent.enabled)
        {
            ActualizarAnimacion(agent.velocity.magnitude);
        }
    }

    private void ActualizarAnimacion(float velocidad)
    {
        if (animator != null && !string.IsNullOrEmpty(parametroVelocidad))
        {
            animator.SetFloat(parametroVelocidad, velocidad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagAgua))
        {
            estaEnAgua = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagAgua))
        {
            estaEnAgua = false;
        }
    }
}