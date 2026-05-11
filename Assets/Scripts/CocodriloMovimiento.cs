using UnityEngine;

public class CocodriloMovimiento : MonoBehaviour
{
    [Header("Zona de movimiento")]
    [SerializeField] private Transform centroLago;
    [SerializeField] private float radioMovimiento = 40f;

    [Header("Zonas prohibidas - tierra/islas")]
    [SerializeField] private Transform[] zonasProhibidas;
    [SerializeField] private float[] radiosZonasProhibidas;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float velocidadRotacion = 3f;
    [SerializeField] private float distanciaParaNuevoDestino = 2f;
    [SerializeField] private int intentosParaBuscarDestino = 30;

    [Header("Altura del agua")]
    [SerializeField] private float alturaSuperficieAgua = 0.9f;
    [SerializeField] private float alturaSumergido = -0.8f;
    [SerializeField] private float movimientoVerticalSuave = 0.08f;
    [SerializeField] private float velocidadFlotar = 2f;

    [Header("Sumergirse")]
    [SerializeField] private float tiempoMinParaSumergirse = 5f;
    [SerializeField] private float tiempoMaxParaSumergirse = 12f;
    [SerializeField] private float tiempoSumergido = 3f;
    [SerializeField] private float velocidadSubirBajar = 2f;

    [Header("Pausas")]
    [SerializeField] private float tiempoMinPausa = 1f;
    [SerializeField] private float tiempoMaxPausa = 3f;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parametroVelocidad = "Speed";

    private Vector3 destino;
    private bool pausado;
    private float tiempoPausa;

    private bool sumergido;
    private float temporizadorSumergirse;
    private float temporizadorSumergido;
    private float alturaObjetivo;

    private void Start()
    {
        if (centroLago == null)
            centroLago = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        alturaObjetivo = alturaSuperficieAgua;

        ReiniciarTiempoParaSumergirse();
        ElegirNuevoDestinoSeguro();
    }

    private void Update()
    {
        ControlarSumergirse();

        if (pausado)
        {
            tiempoPausa -= Time.deltaTime;

            if (tiempoPausa <= 0)
            {
                pausado = false;
                ElegirNuevoDestinoSeguro();
            }

            ActualizarAltura();
            ActualizarAnimacion(0);
            return;
        }

        MoverHaciaDestino();
        ActualizarAltura();
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

        // Si el próximo paso tocaría tierra o isla, busca otro destino.
        if (!EsPuntoValido(nuevaPosicion))
        {
            ElegirNuevoDestinoSeguro();
            return;
        }

        transform.position = nuevaPosicion;

        if (direccion.normalized != Vector3.zero)
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
            Vector3 centro = centroLago.position;

            Vector3 posibleDestino = new Vector3(
                centro.x + puntoRandom.x,
                transform.position.y,
                centro.z + puntoRandom.y
            );

            if (EsPuntoValido(posibleDestino))
            {
                destino = posibleDestino;
                return;
            }
        }

        // Si no encontró punto válido, se queda donde está.
        destino = transform.position;
    }

    private bool EsPuntoValido(Vector3 punto)
    {
        if (centroLago == null)
            return false;

        Vector3 centro = centroLago.position;

        float distanciaAlCentro = Vector3.Distance(
            new Vector3(punto.x, 0, punto.z),
            new Vector3(centro.x, 0, centro.z)
        );

        // No puede salir del radio general del lago.
        if (distanciaAlCentro > radioMovimiento)
            return false;

        // No puede entrar en zonas prohibidas: isla, tierra, costa, etc.
        for (int i = 0; i < zonasProhibidas.Length; i++)
        {
            if (zonasProhibidas[i] == null)
                continue;

            float radioZona = 3f;

            if (i < radiosZonasProhibidas.Length)
                radioZona = radiosZonasProhibidas[i];

            Vector3 centroZona = zonasProhibidas[i].position;

            float distanciaZona = Vector3.Distance(
                new Vector3(punto.x, 0, punto.z),
                new Vector3(centroZona.x, 0, centroZona.z)
            );

            if (distanciaZona < radioZona)
                return false;
        }

        return true;
    }

    private void ControlarSumergirse()
    {
        if (!sumergido)
        {
            temporizadorSumergirse -= Time.deltaTime;

            if (temporizadorSumergirse <= 0)
            {
                sumergido = true;
                temporizadorSumergido = tiempoSumergido;
                alturaObjetivo = alturaSumergido;
            }
        }
        else
        {
            temporizadorSumergido -= Time.deltaTime;

            if (temporizadorSumergido <= 0)
            {
                sumergido = false;
                alturaObjetivo = alturaSuperficieAgua;
                ReiniciarTiempoParaSumergirse();
            }
        }
    }

    private void ActualizarAltura()
    {
        Vector3 posicion = transform.position;

        float flotacion = 0f;

        if (!sumergido)
        {
            flotacion = Mathf.Sin(Time.time * velocidadFlotar) * movimientoVerticalSuave;
        }

        float alturaFinal = alturaObjetivo + flotacion;

        posicion.y = Mathf.Lerp(
            posicion.y,
            alturaFinal,
            velocidadSubirBajar * Time.deltaTime
        );

        transform.position = posicion;
    }

    private void ReiniciarTiempoParaSumergirse()
    {
        temporizadorSumergirse = Random.Range(
            tiempoMinParaSumergirse,
            tiempoMaxParaSumergirse
        );
    }

    private void ActualizarAnimacion(float velocidadActual)
    {
        if (animator != null)
        {
            animator.SetFloat(parametroVelocidad, velocidadActual);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (centroLago != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centroLago.position, radioMovimiento);
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