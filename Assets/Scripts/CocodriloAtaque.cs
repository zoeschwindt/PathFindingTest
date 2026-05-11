using UnityEngine;
using UnityEngine.AI;

public class CocodriloAtaque : MonoBehaviour
{
    [Header("Movimiento de ataque")]
    [SerializeField] private float velocidadAtaque = 8f;
    [SerializeField] private float velocidadRotacion = 8f;
    [SerializeField] private float distanciaMatar = 2f;

    [Header("Altura del agua")]
    [SerializeField] private float alturaAgua = 9.55f;

    [Header("Respawn del jaguar")]
    [SerializeField] private bool respawnearJaguar = true;
    [SerializeField] private Transform puntoRespawnJaguar;

    [Header("Tags")]
    [SerializeField] private string tagJaguar = "Jaguar";
    [SerializeField] private string tagCazador = "Cazador";

    [Header("Movimiento libre")]
    [SerializeField] private MonoBehaviour movimientoLibre;

    private GameObject objetivo;
    private bool atacando;

    private void Awake()
    {
        if (movimientoLibre == null)
        {
            movimientoLibre = GetComponent<CocodriloMovimiento>();
        }
    }

    private void Update()
    {
        if (!atacando)
            return;

        if (objetivo == null)
        {
            TerminarAtaque();
            return;
        }

        PerseguirObjetivo();
    }

    public void AtacarObjetivo(GameObject nuevoObjetivo)
    {
        if (nuevoObjetivo == null)
            return;

        objetivo = nuevoObjetivo;
        atacando = true;

        if (movimientoLibre != null)
            movimientoLibre.enabled = false;

        Debug.Log(name + " va a atacar a " + objetivo.name);
    }

    private void PerseguirObjetivo()
    {
        Vector3 posicionObjetivo = objetivo.transform.position;
        posicionObjetivo.y = alturaAgua;

        Vector3 direccion = posicionObjetivo - transform.position;
        direccion.y = 0f;

        if (direccion.magnitude <= distanciaMatar)
        {
            MatarObjetivo();
            return;
        }

        transform.position += direccion.normalized * velocidadAtaque * Time.deltaTime;

        Vector3 posicion = transform.position;
        posicion.y = alturaAgua;
        transform.position = posicion;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }
    }

    private void MatarObjetivo()
    {
        if (objetivo == null)
        {
            TerminarAtaque();
            return;
        }

        Debug.Log(name + " mató a " + objetivo.name);

        if (objetivo.CompareTag(tagCazador))
        {
            Destroy(objetivo);
        }
        else if (objetivo.CompareTag(tagJaguar))
        {
            if (respawnearJaguar)
            {
                RespawnearJaguar();
            }
            else
            {
                Destroy(objetivo);
            }
        }

        TerminarAtaque();
    }

    private void RespawnearJaguar()
    {
        if (puntoRespawnJaguar == null)
        {
            Debug.LogWarning("Falta asignar Punto Respawn Jaguar.");
            return;
        }

        NavMeshAgent agent = objetivo.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.ResetPath();
            agent.Warp(puntoRespawnJaguar.position);
        }
        else
        {
            objetivo.transform.position = puntoRespawnJaguar.position;
        }
    }

    private void TerminarAtaque()
    {
        objetivo = null;
        atacando = false;

        if (movimientoLibre != null)
            movimientoLibre.enabled = true;
    }
}