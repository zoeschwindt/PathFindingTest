using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CazadorSaltoOffMesh : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Salto")]
    [SerializeField] private float duracionSalto = 0.8f;
    [SerializeField] private float alturaSalto = 2.5f;

    [Header("Fallo de salto")]
    [SerializeField] private bool puedeFallarse = true;
    [Range(0f, 1f)]
    [SerializeField] private float probabilidadFallar = 0.25f;
    [SerializeField] private float tiempoEnAgua = 3f;
    [SerializeField] private float alturaAgua = 0.5f;

    [Header("Animación")]
    [SerializeField] private string parametroVelocidad = "Speed";

    private bool saltando;
    private bool caido;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (agent != null)
            agent.autoTraverseOffMeshLink = false;
    }

    private void Update()
    {
        if (agent == null || caido) return;

        if (agent.isOnOffMeshLink && !saltando)
        {
            bool falla = puedeFallarse && Random.value < probabilidadFallar;

            if (falla)
                StartCoroutine(FallarSalto());
            else
                StartCoroutine(SaltarCorrectamente());
        }
    }

    private IEnumerator SaltarCorrectamente()
    {
        saltando = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;

        Vector3 inicio = transform.position;
        Vector3 final = data.endPos;

        float tiempo = 0f;

        agent.isStopped = true;

        while (tiempo < duracionSalto)
        {
            float t = tiempo / duracionSalto;

            Vector3 posicion = Vector3.Lerp(inicio, final, t);
            posicion.y += Mathf.Sin(t * Mathf.PI) * alturaSalto;

            transform.position = posicion;

            MirarHacia(final - inicio);

            if (animator != null)
                animator.SetFloat(parametroVelocidad, agent.speed);

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = final;

        agent.CompleteOffMeshLink();

        agent.isStopped = false;
        saltando = false;
    }

    private IEnumerator FallarSalto()
    {
        saltando = true;
        caido = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;

        Vector3 inicio = transform.position;
        Vector3 final = data.endPos;

        Vector3 puntoMedio = Vector3.Lerp(inicio, final, 0.5f);
        puntoMedio.y = alturaAgua;

        float tiempo = 0f;

        agent.isStopped = true;
        agent.enabled = false;

        while (tiempo < duracionSalto)
        {
            float t = tiempo / duracionSalto;

            Vector3 posicion = Vector3.Lerp(inicio, puntoMedio, t);
            posicion.y += Mathf.Sin(t * Mathf.PI) * alturaSalto;

            transform.position = posicion;

            MirarHacia(final - inicio);

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = puntoMedio;

        if (animator != null)
            animator.SetFloat(parametroVelocidad, 0f);

        yield return new WaitForSeconds(tiempoEnAgua);

       
        transform.position = inicio;

        agent.enabled = true;

        if (NavMesh.SamplePosition(inicio, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        agent.isStopped = false;

        saltando = false;
        caido = false;
    }

    private void MirarHacia(Vector3 direccion)
    {
        direccion.y = 0;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10f * Time.deltaTime);
        }
    }
}