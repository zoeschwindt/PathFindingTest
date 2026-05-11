using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class JaguarSaltoOffMesh : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Salto")]
    [SerializeField] private float duracionSalto = 0.6f;
    [SerializeField] private float alturaSalto = 1.5f;

    [Header("Animación")]
    [SerializeField] private string parametroVelocidad = "Speed";

    private bool saltando;

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
        if (agent == null) return;

        if (agent.isOnOffMeshLink && !saltando)
        {
            StartCoroutine(SaltarLink());
        }
    }

    private IEnumerator SaltarLink()
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

            Vector3 direccion = final - inicio;
            direccion.y = 0;

            if (direccion.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10f * Time.deltaTime);
            }

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
}