using UnityEngine;
using UnityEngine.AI;

public class AnimalMovimientoArea : MonoBehaviour
{
    [Header("Zona de movimiento")]
    [SerializeField] private Transform centroZona;
    [SerializeField] private float radioMovimiento = 20f;

    [Header("Zonas prohibidas")]
    [SerializeField] private Transform[] zonasProhibidas;
    [SerializeField] private float[] radiosZonasProhibidas;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float velocidadRotacion = 4f;
    [SerializeField] private float distanciaParaNuevoDestino = 1.5f;
    [SerializeField] private int intentosParaBuscarDestino = 30;

    [Header("Altura fija")]
    [SerializeField] private float alturaFija = 0.2f;

    [Header("Pausas")]
    [SerializeField] private float tiempoMinPausa = 1f;
    [SerializeField] private float tiempoMaxPausa = 3f;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parametroVelocidad = "Speed";

    [Header("Matar Jaguar")]
    [SerializeField] private string tagJaguar = "Jaguar";
    [SerializeField] private Transform puntoRespawnJaguar;

    private Vector3 destino;
    private bool pausado;
    private float tiempoPausa;

    private void Start()
    {
        if (centroZona == null)
            centroZona = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        Vector3 posicion = transform.position;
        posicion.y = alturaFija;
        transform.position = posicion;

        ElegirNuevoDestinoSeguro();
    }

    private void Update()
    {
        if (pausado)
        {
            tiempoPausa -= Time.deltaTime;

            if (tiempoPausa <= 0)
            {
                pausado = false;
                ElegirNuevoDestinoSeguro();
            }

            ActualizarAnimacion(0);
            MantenerAltura();
            return;
        }

        MoverHaciaDestino();
        MantenerAltura();
    }

    private void MoverHaciaDestino()
    {
        Vector3 posicionActual = transform.position;

        Vector3 direccion = destino - posicionActual;
        direccion.y = 0;

        if (direccion.magnitude <= distanciaParaNuevoDestino)
        {
            pausado = true;
            tiempoPausa = Random.Range(tiempoMinPausa, tiempoMaxPausa);
            ActualizarAnimacion(0);
            return;
        }

        Vector3 nuevaPosicion = posicionActual + direccion.normalized * velocidad * Time.deltaTime;
        nuevaPosicion.y = alturaFija;

        if (!EsPuntoValido(nuevaPosicion))
        {
            ElegirNuevoDestinoSeguro();
            return;
        }

        transform.position = nuevaPosicion;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }

        ActualizarAnimacion(velocidad);
    }

    private void ElegirNuevoDestinoSeguro()
    {
        for (int i = 0; i < intentosParaBuscarDestino; i++)
        {
            Vector2 puntoRandom = Random.insideUnitCircle * radioMovimiento;
            Vector3 centro = centroZona.position;

            Vector3 posibleDestino = new Vector3(
                centro.x + puntoRandom.x,
                alturaFija,
                centro.z + puntoRandom.y
            );

            if (EsPuntoValido(posibleDestino))
            {
                destino = posibleDestino;
                return;
            }
        }

        destino = transform.position;
        destino.y = alturaFija;
    }

    private bool EsPuntoValido(Vector3 punto)
    {
        if (centroZona == null)
            return false;

        Vector3 centro = centroZona.position;

        float distanciaAlCentro = Vector3.Distance(
            new Vector3(punto.x, 0, punto.z),
            new Vector3(centro.x, 0, centro.z)
        );

        if (distanciaAlCentro > radioMovimiento)
            return false;

        for (int i = 0; i < zonasProhibidas.Length; i++)
        {
            if (zonasProhibidas[i] == null)
                continue;

            float radioZona = 3f;

            if (i < radiosZonasProhibidas.Length)
                radioZona = radiosZonasProhibidas[i];

            Vector3 centroZonaProhibida = zonasProhibidas[i].position;

            float distanciaZona = Vector3.Distance(
                new Vector3(punto.x, 0, punto.z),
                new Vector3(centroZonaProhibida.x, 0, centroZonaProhibida.z)
            );

            if (distanciaZona < radioZona)
                return false;
        }

        return true;
    }

    private void MantenerAltura()
    {
        Vector3 posicion = transform.position;
        posicion.y = alturaFija;
        transform.position = posicion;
    }

    private void ActualizarAnimacion(float velocidadActual)
    {
        if (animator != null && !string.IsNullOrEmpty(parametroVelocidad))
        {
            animator.SetFloat(parametroVelocidad, velocidadActual);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RevisarSiEsJaguar(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        RevisarSiEsJaguar(collision.collider);
    }

    private void RevisarSiEsJaguar(Collider other)
    {
        GameObject jaguar = BuscarJaguarEnPadres(other.transform);

        if (jaguar != null)
        {
            RespawnearJaguar(jaguar);
        }
    }

    private GameObject BuscarJaguarEnPadres(Transform t)
    {
        Transform actual = t;

        while (actual != null)
        {
            if (actual.CompareTag(tagJaguar))
            {
                return actual.gameObject;
            }

            actual = actual.parent;
        }

        return null;
    }

    private void RespawnearJaguar(GameObject jaguar)
    {
        if (puntoRespawnJaguar == null)
        {
            Debug.LogWarning("Falta asignar el Punto Respawn Jaguar.");
            return;
        }

        NavMeshAgent agent = jaguar.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.ResetPath();
            agent.Warp(puntoRespawnJaguar.position);
        }
        else
        {
            CharacterController controller = jaguar.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                jaguar.transform.position = puntoRespawnJaguar.position;
                controller.enabled = true;
            }
            else
            {
                jaguar.transform.position = puntoRespawnJaguar.position;
            }
        }

        Debug.Log("El jaguar murió y volvió al respawn.");
    }

    private void OnDrawGizmosSelected()
    {
        if (centroZona != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centroZona.position, radioMovimiento);
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < zonasProhibidas.Length; i++)
        {
            if (zonasProhibidas[i] == null)
                continue;

            float radioZona = 3f;

            if (i < radiosZonasProhibidas.Length)
                radioZona = radiosZonasProhibidas[i];

            Gizmos.DrawWireSphere(zonasProhibidas[i].position, radioZona);
        }
    }
}